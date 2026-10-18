using Billing.Data;
using Billing.Configuration;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Billing.Controllers.Api;

[ApiController]
[Route("api/organizations/{organizationId:int}/feature-settings")]
[AllowAnonymous]
public sealed class OrganizationFeatureSettingsController : ControllerBase
{
    private readonly BillingDbContext _context;
    private readonly IConfiguration _configuration;

    public OrganizationFeatureSettingsController(BillingDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<OrganizationFeatureSettingDto>>> Get(int organizationId)
    {
        if (!await CanAccessAsync(organizationId))
            return Unauthorized();

        if (!await IsEnabledOrganizationAsync(organizationId))
            return NotFound();

        var features = await _context.Features
            .Where(f => f.IsOrganizationConfigurable)
            .ToListAsync();

        var featureIds = features.Select(f => f.FeatureId).ToList();
        var settings = await _context.OrganizationFeatureSettings
            .Where(s => s.OrganizationId == organizationId && featureIds.Contains(s.FeatureId))
            .ToDictionaryAsync(s => s.FeatureId, s => s.IsEnabled);

        var result = features
            .Select(f => new OrganizationFeatureSettingDto(
                f.FeatureKey,
                f.FeatureName,
                !settings.TryGetValue(f.FeatureId, out var isEnabled) || isEnabled))
            .OrderBy(x => x.FeatureName)
            .ToList();

        return Ok(result);
    }

    [HttpPut("{featureKey}")]
    public async Task<IActionResult> Update(int organizationId, string featureKey, UpdateOrganizationFeatureSettingRequest request)
    {
        if (!await CanAccessAsync(organizationId))
            return Unauthorized();

        if (!await IsEnabledOrganizationAsync(organizationId))
            return NotFound();

        var feature = await _context.Features
            .SingleOrDefaultAsync(f => f.FeatureKey == featureKey && f.IsOrganizationConfigurable);
        if (feature == null)
            return NotFound();

        var setting = await _context.OrganizationFeatureSettings
            .SingleOrDefaultAsync(s => s.OrganizationId == organizationId && s.FeatureId == feature.FeatureId);
        if (setting == null)
        {
            _context.OrganizationFeatureSettings.Add(new Billing.Models.OrganizationFeatureSetting
            {
                OrganizationId = organizationId,
                FeatureId = feature.FeatureId,
                IsEnabled = request.IsEnabled
            });
        }
        else
        {
            setting.IsEnabled = request.IsEnabled;
        }

        await _context.SaveChangesAsync();
        return NoContent();
    }

    private async Task<bool> IsEnabledOrganizationAsync(int organizationId) =>
        await _context.Organizations.AnyAsync(o => o.OrganizationId == organizationId && o.IsActive == true);

    private async Task<bool> CanAccessAsync(int organizationId)
    {
        var configuredApiKey = _configuration["ManagementApi:ApiKey"];
        var suppliedApiKey = Request.Headers["X-Platform-Api-Key"].FirstOrDefault();

        if (!string.IsNullOrEmpty(configuredApiKey) &&
            !string.IsNullOrEmpty(suppliedApiKey) &&
            FixedTimeEquals(suppliedApiKey, configuredApiKey))
            return true;

        if (!string.IsNullOrEmpty(suppliedApiKey))
            return false;

        // Browser callers retain the existing management authorization model.
        return User.Identity?.IsAuthenticated == true &&
               await OrganizationAuthorization.HasAccessAsync(_context, User, organizationId);
    }

    private static bool FixedTimeEquals(string left, string right)
    {
        var leftBytes = Encoding.UTF8.GetBytes(left);
        var rightBytes = Encoding.UTF8.GetBytes(right);
        return leftBytes.Length == rightBytes.Length &&
               CryptographicOperations.FixedTimeEquals(leftBytes, rightBytes);
    }
}

public sealed record OrganizationFeatureSettingDto(string FeatureKey, string FeatureName, bool IsEnabled);
public sealed record UpdateOrganizationFeatureSettingRequest(bool IsEnabled);
