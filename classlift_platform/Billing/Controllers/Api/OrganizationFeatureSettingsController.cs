using Billing.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Billing.Controllers.Api;

[ApiController]
[Route("api/organizations/{organizationId:int}/feature-settings")]
public sealed class OrganizationFeatureSettingsController : ControllerBase
{
    private readonly BillingDbContext _context;

    public OrganizationFeatureSettingsController(BillingDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<OrganizationFeatureSettingDto>>> Get(int organizationId)
    {
        var settings = await _context.Features
            .Where(f => f.IsOrganizationConfigurable)
            .GroupJoin(
                _context.OrganizationFeatureSettings.Where(s => s.OrganizationId == organizationId),
                f => f.FeatureId,
                s => s.FeatureId,
                (feature, values) => new { feature, setting = values.FirstOrDefault() })
            .Select(x => new OrganizationFeatureSettingDto(
                x.feature.FeatureKey,
                x.feature.FeatureName,
                x.setting == null || x.setting.IsEnabled))
            .OrderBy(x => x.FeatureName)
            .ToListAsync();

        return Ok(settings);
    }

    [HttpPut("{featureKey}")]
    public async Task<IActionResult> Update(int organizationId, string featureKey, UpdateOrganizationFeatureSettingRequest request)
    {
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
}

public sealed record OrganizationFeatureSettingDto(string FeatureKey, string FeatureName, bool IsEnabled);
public sealed record UpdateOrganizationFeatureSettingRequest(bool IsEnabled);
