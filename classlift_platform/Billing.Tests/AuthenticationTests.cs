using System.Reflection;
using Billing.Configuration;
using Billing.Controllers;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using IdentitySignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace Billing.Tests;

public class AuthenticationTests
{
    [Fact]
    public void Management_policy_requires_an_authenticated_user()
    {
        Assert.Contains(
            ManagementAuthorization.AuthenticatedUserPolicy.Requirements,
            requirement => requirement is DenyAnonymousAuthorizationRequirement);
    }

    [Fact]
    public async Task Successful_login_redirects_to_dashboard()
    {
        var signInManager = new TestSignInManager
        {
            PasswordSignInResult = IdentitySignInResult.Success
        };
        var controller = new AccountController(signInManager);

        var result = await controller.Login("user@example.com", "password");

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
        Assert.Equal("Dashboard", redirect.ControllerName);
    }

    [Fact]
    public async Task Failed_login_returns_login_view_with_error()
    {
        var controller = new AccountController(new TestSignInManager());

        var result = await controller.Login("user@example.com", "wrong-password");

        Assert.IsType<ViewResult>(result);
        Assert.Equal("Invalid login.", controller.ViewBag.Error);
    }

    [Fact]
    public async Task Logout_signs_out_and_redirects_to_login()
    {
        var signInManager = new TestSignInManager();
        var controller = new AccountController(signInManager);

        var result = await controller.Logout();

        Assert.True(signInManager.SignOutCalled);
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Login", redirect.ActionName);
    }

    [Fact]
    public void Logout_requires_post_and_antiforgery_token()
    {
        var method = typeof(AccountController).GetMethod(nameof(AccountController.Logout));

        Assert.NotNull(method);
        Assert.NotNull(method!.GetCustomAttribute<HttpPostAttribute>());
        Assert.NotNull(method.GetCustomAttribute<ValidateAntiForgeryTokenAttribute>());
    }

    private sealed class TestSignInManager : SignInManager<IdentityUser>
    {
        public TestSignInManager()
            : base(
                CreateUserManager(),
                new HttpContextAccessor(),
                new TestClaimsPrincipalFactory(),
                Microsoft.Extensions.Options.Options.Create(new IdentityOptions()),
                NullLogger<SignInManager<IdentityUser>>.Instance,
                new AuthenticationSchemeProvider(Microsoft.Extensions.Options.Options.Create(new AuthenticationOptions())),
                new DefaultUserConfirmation<IdentityUser>())
        {
        }

        public IdentitySignInResult PasswordSignInResult { get; init; } = IdentitySignInResult.Failed;
        public bool SignOutCalled { get; private set; }

        public override Task<IdentitySignInResult> PasswordSignInAsync(
            string userName,
            string password,
            bool isPersistent,
            bool lockoutOnFailure) => Task.FromResult(PasswordSignInResult);

        public override Task SignOutAsync()
        {
            SignOutCalled = true;
            return Task.CompletedTask;
        }

        private static UserManager<IdentityUser> CreateUserManager() => new(
            new TestUserStore(),
            Microsoft.Extensions.Options.Options.Create(new IdentityOptions()),
            new PasswordHasher<IdentityUser>(),
            [],
            [],
            new UpperInvariantLookupNormalizer(),
            new IdentityErrorDescriber(),
            new ServiceCollection().BuildServiceProvider(),
            NullLogger<UserManager<IdentityUser>>.Instance);
    }

    private sealed class TestUserStore : IUserStore<IdentityUser>
    {
        public Task<IdentityResult> CreateAsync(IdentityUser user, CancellationToken cancellationToken) =>
            Task.FromResult(IdentityResult.Success);

        public Task<IdentityResult> DeleteAsync(IdentityUser user, CancellationToken cancellationToken) =>
            Task.FromResult(IdentityResult.Success);

        public void Dispose()
        {
        }

        public Task<IdentityUser?> FindByIdAsync(string userId, CancellationToken cancellationToken) =>
            Task.FromResult<IdentityUser?>(null);

        public Task<IdentityUser?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken) =>
            Task.FromResult<IdentityUser?>(null);

        public Task<string?> GetNormalizedUserNameAsync(IdentityUser user, CancellationToken cancellationToken) =>
            Task.FromResult(user.NormalizedUserName);

        public Task<string> GetUserIdAsync(IdentityUser user, CancellationToken cancellationToken) =>
            Task.FromResult(user.Id);

        public Task<string?> GetUserNameAsync(IdentityUser user, CancellationToken cancellationToken) =>
            Task.FromResult(user.UserName);

        public Task SetNormalizedUserNameAsync(IdentityUser user, string? normalizedName, CancellationToken cancellationToken)
        {
            user.NormalizedUserName = normalizedName;
            return Task.CompletedTask;
        }

        public Task SetUserNameAsync(IdentityUser user, string? userName, CancellationToken cancellationToken)
        {
            user.UserName = userName;
            return Task.CompletedTask;
        }

        public Task<IdentityResult> UpdateAsync(IdentityUser user, CancellationToken cancellationToken) =>
            Task.FromResult(IdentityResult.Success);
    }

    private sealed class TestClaimsPrincipalFactory : IUserClaimsPrincipalFactory<IdentityUser>
    {
        public Task<System.Security.Claims.ClaimsPrincipal> CreateAsync(IdentityUser user) =>
            Task.FromResult(new System.Security.Claims.ClaimsPrincipal());
    }

}
