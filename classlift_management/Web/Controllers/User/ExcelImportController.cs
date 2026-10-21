using ClosedXML.Excel;
using Core.Contexts;
using Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Web.Controllers.User;

[Authorize(Roles = "Staff")]
[Route("User/ExcelImport")]
public sealed class ExcelImportController : Controller
{
    private const string DefaultImportPassword = "hello123!";
    private readonly AppDbContext _db;
    private readonly UserManager<Core.Models.User> _userManager;
    private readonly CurrentTenant _currentTenant;

    public ExcelImportController(AppDbContext db, UserManager<Core.Models.User> userManager, CurrentTenant currentTenant)
    {
        _db = db;
        _userManager = userManager;
        _currentTenant = currentTenant;
    }

    [HttpGet]
    public IActionResult Index() => View();

    [HttpGet("/Coach/Import")]
    public IActionResult CoachImport() { ViewBag.ImportType = "Coach"; return View("Index"); }

    [HttpGet("/Child/Import")]
    public IActionResult ChildImport() { ViewBag.ImportType = "Child"; return View("Index"); }

    [HttpGet("Template/Coach")]
    public IActionResult DownloadCoachTemplate() => Template(_currentTenant.Terminology.ProviderSingular, new[] { "Name", "Email", "Gender", "Phone", "Province", "City", "Specialty" });

    [HttpGet("Template/Child")]
    public IActionResult DownloadChildTemplate() => Template(_currentTenant.Terminology.ParticipantSingular, new[] { "Name", "Email", "BirthDate", "Gender", "Province", "City" });

    [HttpPost("Coach")]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> ImportCoach(IFormFile file) => ImportAsync(file, ImportType.Coach);

    [HttpPost("Child")]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> ImportChild(IFormFile file) => ImportAsync(file, ImportType.Child);

    [HttpPost("Preview/Coach")]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> PreviewCoach(IFormFile file) => PreviewAsync(file, ImportType.Coach);

    [HttpPost("Preview/Child")]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> PreviewChild(IFormFile file) => PreviewAsync(file, ImportType.Child);

    [HttpPost("Confirm")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Confirm()
    {
        var typeText = HttpContext.Session.GetString("ExcelImportType");
        var data = HttpContext.Session.GetString("ExcelImportFile");
        if (!Enum.TryParse<ImportType>(typeText, out var type) || string.IsNullOrWhiteSpace(data))
            return BadRequest("The preview has expired. Please upload the file again.");

        HttpContext.Session.Remove("ExcelImportType");
        HttpContext.Session.Remove("ExcelImportFile");
        await using var stream = new MemoryStream(Convert.FromBase64String(data));
        var file = new FormFile(stream, 0, stream.Length, "file", $"{type}-import.xlsx");
        return await ImportAsync(file, type, true);
    }

    [HttpPost("Cancel")]
    [ValidateAntiForgeryToken]
    public IActionResult Cancel()
    {
        HttpContext.Session.Remove("ExcelImportType");
        HttpContext.Session.Remove("ExcelImportFile");
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("Result")]
    public IActionResult DownloadResult()
    {
        var data = HttpContext.Session.GetString("ExcelImportResult");
        if (string.IsNullOrWhiteSpace(data)) return NotFound("The import result has expired.");
        HttpContext.Session.Remove("ExcelImportResult");
        TempData.Remove("ImportResultAvailable");
        return File(Convert.FromBase64String(data), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "import-result.xlsx");
    }

    private async Task<IActionResult> PreviewAsync(IFormFile file, ImportType type)
    {
        if (file == null || file.Length == 0) return BadRequest("Please select an Excel file.");
        if (!Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase)) return BadRequest("Only .xlsx files are supported.");
        await using var buffer = new MemoryStream();
        await file.CopyToAsync(buffer);
        var bytes = buffer.ToArray();
        using var workbook = new XLWorkbook(new MemoryStream(bytes));
        var sheet = workbook.Worksheets.FirstOrDefault();
        if (sheet == null) return BadRequest("The workbook does not contain a worksheet.");
        var rows = sheet.RowsUsed().ToList();
        if (rows.Count < 2) return BadRequest("The worksheet must contain a header row and at least one data row.");
        var headers = rows[0].CellsUsed().ToDictionary(c => Normalize(c.GetString()), c => c.Address.ColumnNumber);
        var emailColumn = headers.GetValueOrDefault("email");
        var emails = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var duplicate = 0;
        foreach (var row in rows.Skip(1))
        {
            var email = emailColumn > 0 ? row.Cell(emailColumn).GetString().Trim() : string.Empty;
            if (!string.IsNullOrWhiteSpace(email) && !emails.Add(email)) duplicate++;
        }
        var staff = await _userManager.GetUserAsync(User);
        var existing = 0;
        foreach (var email in emails)
            if (await _userManager.FindByEmailAsync(email) != null) existing++;

        HttpContext.Session.SetString("ExcelImportType", type.ToString());
        HttpContext.Session.SetString("ExcelImportFile", Convert.ToBase64String(bytes));
        ViewBag.Preview = true;
        ViewBag.ImportType = type.ToString();
        ViewBag.TotalRows = rows.Count - 1;
        ViewBag.DuplicateEmails = duplicate;
        ViewBag.ExistingEmails = existing;
        ViewBag.Warnings = 0;
        ViewBag.Errors = headers.ContainsKey("email") ? 0 : rows.Count - 1;
        return View("Index");
    }

    private async Task<IActionResult> ImportAsync(IFormFile file, ImportType type, bool redirectAfterImport = false)
    {
        if (file == null || file.Length == 0)
            return BadRequest("Please select an Excel file.");
        if (!Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
            return BadRequest("Only .xlsx files are supported.");

        var staff = await _userManager.GetUserAsync(User);
        if (staff == null) return Unauthorized();

        using var input = file.OpenReadStream();
        using var workbook = new XLWorkbook(input);
        var sheet = workbook.Worksheets.FirstOrDefault();
        if (sheet == null) return BadRequest("The workbook does not contain a worksheet.");

        var rows = sheet.RowsUsed().ToList();
        if (rows.Count < 2) return BadRequest("The worksheet must contain a header row and at least one data row.");
        var headers = rows[0].CellsUsed().ToDictionary(c => Normalize(c.GetString()), c => c.Address.ColumnNumber);

        var required = type == ImportType.Coach
            ? new[] { "name", "email", "gender", "phone", "province", "city", "specialty" }
            : new[] { "name", "email", "birthdate", "gender", "province", "city" };
        var missing = required.Where(h => !headers.ContainsKey(h)).ToArray();
        if (missing.Length > 0) return BadRequest("Missing columns: " + string.Join(", ", missing));

        var cities = await _db.Cities.AsNoTracking().ToListAsync();
        var unknownCity = cities.FirstOrDefault(c => Normalize(c.Name) == "unknown");
        if (unknownCity == null) return BadRequest("The Unknown city has not been configured.");
        var provinces = await _db.Provinces.AsNoTracking().ToListAsync();
        var unknownProvince = provinces.FirstOrDefault(p => Normalize(p.Name) == "unknown");
        if (unknownProvince == null) return BadRequest("The Unknown province has not been configured.");
        var specialties = await _db.Specialties.AsNoTracking().ToListAsync();
        var errors = new List<string>();
        var imported = 0;
        var fileEmails = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var row in rows.Skip(1))
        {
            var rowNumber = row.RowNumber();
            await using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var email = Get(row, headers, "email");
                if (string.IsNullOrWhiteSpace(email)) throw new InvalidOperationException("Email is required.");
                if (!fileEmails.Add(email)) throw new InvalidOperationException("Email is duplicated in this Excel file; row skipped.");
                if (await _userManager.FindByEmailAsync(email) != null)
                    throw new InvalidOperationException("Email already exists; row skipped.");

                var name = Get(row, headers, "name");
                var password = DefaultImportPassword;
                if (string.IsNullOrWhiteSpace(name))
                    throw new InvalidOperationException("Name is required.");

                var cityName = Get(row, headers, "city");
                var city = cities.FirstOrDefault(c => Normalize(c.Name) == Normalize(cityName));
                if (city == null)
                {
                    city = unknownCity;
                    if (!string.IsNullOrWhiteSpace(cityName) && headers.TryGetValue("city", out var cityColumn))
                        row.Cell(cityColumn).Style.Fill.BackgroundColor = XLColor.LightYellow;
                }
                var provinceName = Get(row, headers, "province");
                var province = provinces.FirstOrDefault(p => Normalize(p.Name) == Normalize(provinceName));
                if (!string.IsNullOrWhiteSpace(provinceName) && province == null)
                    throw new InvalidOperationException("Province: the province does not exist.");
                if (province != null && city.CityID != unknownCity.CityID && city.ProvinceID != province.ProvinceID)
                {
                    province = unknownProvince;
                    city = unknownCity;
                    if (headers.TryGetValue("province", out var provinceColumn))
                    {
                        row.Cell(provinceColumn).Style.Fill.BackgroundColor = XLColor.LightYellow;
                        row.Cell(provinceColumn).CreateComment().AddText("Province and City do not match. Unknown Province was used.");
                    }
                    if (headers.TryGetValue("city", out var cityColumn))
                    {
                        row.Cell(cityColumn).Style.Fill.BackgroundColor = XLColor.LightYellow;
                        row.Cell(cityColumn).CreateComment().AddText("Province and City do not match. Unknown City was used.");
                    }
                }
                _db.Attach(city);
                var user = new Core.Models.User { UserName = email, Email = email, Role = type.ToString(), CreatedBy = staff.Id };
                var create = await _userManager.CreateAsync(user, password);
                if (!create.Succeeded) throw new InvalidOperationException(string.Join("; ", create.Errors.Select(e => e.Description)));
                await _userManager.AddToRoleAsync(user, type.ToString());

                if (type == ImportType.Coach)
                {
                    var specialtyNames = Get(row, headers, "specialty").Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    var specialtyIds = specialties.Where(s => specialtyNames.Any(n => Normalize(n) == Normalize(s.Title))).Select(s => s.SpecialtyID).ToList();
                    if (specialtyNames.Length > 0 && specialtyIds.Count == 0)
                        throw new InvalidOperationException("Specialty: no matching specialty was found.");
                    var unmatchedSpecialties = specialtyNames
                        .Where(name => !specialties.Any(s => Normalize(s.Title) == Normalize(name)))
                        .ToArray();
                    if (unmatchedSpecialties.Length > 0 && headers.TryGetValue("specialty", out var specialtyColumn))
                    {
                        row.Cell(specialtyColumn).Style.Fill.BackgroundColor = XLColor.LightYellow;
                        row.Cell(specialtyColumn).CreateComment().AddText("Unmatched Specialty names: " + string.Join(", ", unmatchedSpecialties));
                    }
                    _db.Coaches.Add(new Coach { UserID = user.Id, Name = name, Gender = Get(row, headers, "gender"), Phone = Get(row, headers, "phone"), CityID = city.CityID, City = city, Status = "Active", CoachSpecialties = specialtyIds.Select(id => new CoachSpecialty { SpecialtyID = id }).ToList() });
                }
                else
                {
                    DateTime? birthDate = null;
                    var birth = Get(row, headers, "birthdate");
                    if (!string.IsNullOrWhiteSpace(birth))
                    {
                        if (!DateTime.TryParse(birth, out var parsedBirthDate))
                            throw new InvalidOperationException("BirthDate is invalid.");
                        birthDate = parsedBirthDate;
                    }
                    _db.Children.Add(new Child { UserID = user.Id, User = user, Name = name, BirthDate = birthDate, Gender = Get(row, headers, "gender"), CityID = city.CityID, City = city, HasOAP = false });
                }
                await _db.SaveChangesAsync();
                await transaction.CommitAsync();
                imported++;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                foreach (var entry in _db.ChangeTracker.Entries().Where(e => e.State != EntityState.Unchanged))
                    entry.State = EntityState.Detached;
                errors.Add($"Row {rowNumber}: {ex.Message}");
                var field = ex.Message.StartsWith("ProvinceCity:", StringComparison.OrdinalIgnoreCase) ? "provincecity" :
                    ex.Message.StartsWith("Province:", StringComparison.OrdinalIgnoreCase) ? "province" :
                    ex.Message.StartsWith("Specialty:", StringComparison.OrdinalIgnoreCase) ? "specialty" :
                    ex.Message.StartsWith("City:", StringComparison.OrdinalIgnoreCase) ? "city" : null;
                if (field == "provincecity")
                {
                    if (headers.TryGetValue("province", out var provinceColumn))
                        row.Cell(provinceColumn).Style.Fill.BackgroundColor = XLColor.LightPink;
                    if (headers.TryGetValue("city", out var cityColumn))
                        row.Cell(cityColumn).Style.Fill.BackgroundColor = XLColor.LightPink;
                }
                else if (field != null && headers.TryGetValue(field, out var errorColumn))
                    row.Cell(errorColumn).Style.Fill.BackgroundColor = XLColor.LightPink;
                else
                    row.Style.Fill.BackgroundColor = XLColor.LightPink;
                row.Cell(1).CreateComment().AddText("Import error: " + ex.Message);
            }
        }

        var result = workbook;
        var resultSheet = result.AddWorksheet("Import result");
        resultSheet.Cell(1, 1).Value = "Imported rows";
        resultSheet.Cell(1, 2).Value = imported;
        resultSheet.Cell(2, 1).Value = "Errors";
        resultSheet.Cell(2, 2).Value = errors.Count;
        for (var i = 0; i < errors.Count; i++) resultSheet.Cell(i + 4, 1).Value = errors[i];
        foreach (var worksheet in result.Worksheets)
        {
            worksheet.ColumnsUsed().AdjustToContents();
            foreach (var column in worksheet.ColumnsUsed())
                column.Width = Math.Clamp(column.Width, 18, 50);
        }
        using var output = new MemoryStream();
        result.SaveAs(output);
        if (redirectAfterImport)
        {
            using var resultStream = new MemoryStream();
            result.SaveAs(resultStream);
            HttpContext.Session.SetString("ExcelImportResult", Convert.ToBase64String(resultStream.ToArray()));
            TempData["ImportSummary"] = $"Imported {imported} record(s). Errors: {errors.Count}.";
            TempData["ImportResultAvailable"] = true;
            return Redirect(type == ImportType.Coach ? "/Coach/List" : "/Child/List");
        }
        return File(output.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "import-result.xlsx");
    }

    private static string Get(IXLRow row, IReadOnlyDictionary<string, int> headers, string name) => headers.TryGetValue(name, out var column) ? row.Cell(column).GetString().Trim() : string.Empty;
    private static string Normalize(string? value) => (value ?? string.Empty).Trim().ToLowerInvariant();
    private static bool ParseBool(string value) => value.Equals("true", StringComparison.OrdinalIgnoreCase) || value.Equals("yes", StringComparison.OrdinalIgnoreCase) || value == "1";
    private IActionResult Template(string type, string[] headers)
    {
        using var workbook = new XLWorkbook();
        var sheet = workbook.AddWorksheet(type);
        for (var i = 0; i < headers.Length; i++)
        {
            sheet.Cell(1, i + 1).Value = headers[i];
            sheet.Cell(1, i + 1).Style.Font.Bold = true;
        }
        sheet.Row(1).Style.Fill.BackgroundColor = XLColor.LightBlue;
        sheet.Columns().AdjustToContents();
        foreach (var column in sheet.ColumnsUsed())
            column.Width = Math.Clamp(column.Width, 18, 32);
        using var output = new MemoryStream();
        workbook.SaveAs(output);
        var safeName = string.Concat(type.Where(ch => !Path.GetInvalidFileNameChars().Contains(ch))).Trim();
        if (string.IsNullOrWhiteSpace(safeName)) safeName = type;
        return File(output.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{safeName}-import-template.xlsx");
    }
    private enum ImportType { Coach, Child }
}
