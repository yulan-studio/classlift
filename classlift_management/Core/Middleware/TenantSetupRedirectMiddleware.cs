using Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Middleware
{
    public class TenantSetupRedirectMiddleware
    {
        private readonly RequestDelegate _next;

        public TenantSetupRedirectMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(
            HttpContext context,
            UserManager<User> userManager)
        {
            var path = context.Request.Path.Value?.ToLower() ?? "";

            if (path.StartsWith("/setup") ||
                path.StartsWith("/css") ||
                path.StartsWith("/js") ||
                path.StartsWith("/lib") ||
                path.StartsWith("/images"))
            {
                await _next(context);
                return;
            }

            var admins = await userManager.GetUsersInRoleAsync("Admin");

            if (!admins.Any())
            {
                context.Response.Redirect("/Setup/CreateAdmin");
                return;
            }

            await _next(context);
        }
    }
}
