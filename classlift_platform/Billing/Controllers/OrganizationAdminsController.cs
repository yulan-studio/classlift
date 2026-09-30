using Billing.Data;
using Billing.Models;
using Billing.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Billing.Controllers;

[Authorize(Roles = "Admin")]
[Route("OrganizationAdmins")]
public sealed class OrganizationAdminsController : Controller
{
    private const string OrganizationAdminRole = "OrganizationAdmin";
    private readonly BillingDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public OrganizationAdminsController(
        BillingDbContext context,
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        return View(await BuildModelAsync(new OrganizationAdminManagementViewModel(), cancellationToken));
    }

    [HttpPost("Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(OrganizationAdminManagementViewModel model, CancellationToken cancellationToken)
    {
        if (model.OrganizationId <= 0)
            ModelState.AddModelError(nameof(model.OrganizationId), "Select an organization.");
        if (string.IsNullOrWhiteSpace(model.Email))
            ModelState.AddModelError(nameof(model.Email), "Email is required.");
        if (string.IsNullOrWhiteSpace(model.Password) || model.Password.Length < 6)
            ModelState.AddModelError(nameof(model.Password), "Password must be at least 6 characters.");

        var organizationExists = await _context.Organizations
            .AnyAsync(o => o.OrganizationId == model.OrganizationId, cancellationToken);
        if (!organizationExists)
            ModelState.AddModelError(nameof(model.OrganizationId), "Organization was not found.");

        if (!ModelState.IsValid)
            return View("Index", await BuildModelAsync(model, cancellationToken));

        var email = model.Email.Trim();
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            user = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true };
            var createResult = await _userManager.CreateAsync(user, model.Password);
            if (!createResult.Succeeded)
            {
                AddIdentityErrors(createResult);
                return View("Index", await BuildModelAsync(model, cancellationToken));
            }
        }

        if (!await _roleManager.RoleExistsAsync(OrganizationAdminRole))
            await _roleManager.CreateAsync(new IdentityRole(OrganizationAdminRole));
        if (!await _userManager.IsInRoleAsync(user, OrganizationAdminRole))
            await _userManager.AddToRoleAsync(user, OrganizationAdminRole);

        var alreadyBound = await _context.OrganizationAdmins.AnyAsync(
            a => a.UserId == user.Id && a.OrganizationId == model.OrganizationId,
            cancellationToken);
        if (!alreadyBound)
        {
            _context.OrganizationAdmins.Add(new OrganizationAdmin
            {
                UserId = user.Id,
                OrganizationId = model.OrganizationId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            });
            await _context.SaveChangesAsync(cancellationToken);
        }
        else
        {
            TempData["Error"] = "This user is already assigned to the selected organization.";
            return RedirectToAction(nameof(Index));
        }

        TempData["Success"] = $"Organization administrator {email} was created and assigned.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("Toggle/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Toggle(int id, CancellationToken cancellationToken)
    {
        var binding = await _context.OrganizationAdmins.FindAsync([id], cancellationToken);
        if (binding == null) return NotFound();

        binding.IsActive = !binding.IsActive;
        await _context.SaveChangesAsync(cancellationToken);
        TempData["Success"] = binding.IsActive ? "Administrator access enabled." : "Administrator access disabled.";
        return RedirectToAction(nameof(Index));
    }

    private async Task<OrganizationAdminManagementViewModel> BuildModelAsync(
        OrganizationAdminManagementViewModel model,
        CancellationToken cancellationToken)
    {
        model.Organizations = await _context.Organizations
            .OrderBy(o => o.OrganizationName)
            .Select(o => new SelectListItem(o.OrganizationName, o.OrganizationId.ToString()))
            .ToListAsync(cancellationToken);

        model.Administrators = await _context.OrganizationAdmins
            .Include(a => a.Organization)
            .OrderBy(a => a.Organization.OrganizationName)
            .ThenBy(a => a.UserId)
            .Select(a => new OrganizationAdminRow(
                a.OrganizationAdminId,
                _context.Users.Where(u => u.Id == a.UserId).Select(u => u.Email).FirstOrDefault() ?? a.UserId,
                _context.Users.Where(u => u.Id == a.UserId).Select(u => u.UserName).FirstOrDefault(),
                a.Organization.OrganizationName,
                a.IsActive,
                a.CreatedAt))
            .ToListAsync(cancellationToken);
        return model;
    }

    private void AddIdentityErrors(IdentityResult result)
    {
        foreach (var error in result.Errors)
            ModelState.AddModelError(string.Empty, error.Description);
    }
}
