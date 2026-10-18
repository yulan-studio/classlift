using System.Net;
using System.Net.Http.Json;
using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Core.Services;

public sealed class OrganizationFeatureSettingsService : IOrganizationFeatureSettingsService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly CurrentTenant _currentTenant;
    private readonly IConfiguration _configuration;

    public OrganizationFeatureSettingsService(
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor,
        CurrentTenant currentTenant,
        IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _httpContextAccessor = httpContextAccessor;
        _currentTenant = currentTenant;
        _configuration = configuration;
    }

    public async Task<IReadOnlyList<OrganizationFeatureSettingDto>> GetAsync(CancellationToken cancellationToken = default)
    {
        var response = await SendAsync(HttpMethod.Get, string.Empty, null, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<OrganizationFeatureSettingDto>>(cancellationToken: cancellationToken)
            ?? [];
    }

    public async Task UpdateAsync(string featureKey, bool isEnabled, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(featureKey) || featureKey.Contains('/') || featureKey.Contains('\\'))
            throw new ArgumentException("Invalid feature key.", nameof(featureKey));

        var response = await SendAsync(
            HttpMethod.Put,
            $"feature-settings/{Uri.EscapeDataString(featureKey)}",
            new UpdateOrganizationFeatureSettingRequest { IsEnabled = isEnabled },
            cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    private async Task<HttpResponseMessage> SendAsync(
        HttpMethod method,
        string suffix,
        object? body,
        CancellationToken cancellationToken)
    {
        if (!_currentTenant.OrganizationId.HasValue)
            throw new InvalidOperationException("The organization could not be resolved.");

        var client = _httpClientFactory.CreateClient("PlatformApi");
        using var request = new HttpRequestMessage(
            method,
            $"api/organizations/{_currentTenant.OrganizationId.Value}/feature-settings{(string.IsNullOrEmpty(suffix) ? string.Empty : "/" + suffix)}");

        if (body != null)
            request.Content = JsonContent.Create(body);

        var context = _httpContextAccessor.HttpContext;
        var apiKey = _configuration["PlatformApi:ApiKey"];
        if (!string.IsNullOrWhiteSpace(apiKey))
            request.Headers.TryAddWithoutValidation("X-Platform-Api-Key", apiKey);

        if (context?.Request.Headers.Authorization.Count > 0)
            request.Headers.TryAddWithoutValidation("Authorization", context.Request.Headers.Authorization.ToString());

        return await client.SendAsync(request, cancellationToken);
    }
}
