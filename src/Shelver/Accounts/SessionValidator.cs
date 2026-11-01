/***********************************************************************************************
*
*   SessionValidator.cs
*
*   Copyright (c) 2026 Josh Hayden (@jhayden02)
*
*   shelver is free software: you can redistribute it and/or modify
*   it under the terms of the GNU General Public License v3.0 as published
*   by the Free Software Foundation.
*
*   shelver is distributed in the hope that it will be useful,
*   but WITHOUT ANY WARRANTY; without even the implied warranty of
*   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
*   GNU General Public License for more details.
*
*   You should have received a copy of the GNU General Public License
*   along with this program.  If not, see <https://www.gnu.org/licenses/>.
*
***********************************************************************************************/

using System.Globalization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Shelver.Models;

namespace Shelver.Accounts;

// The sign-in cookie is checked on every request. The cookie's own issue time is reset each
// time it is renewed, so the sign-in time and the last request time are stored in it separately.
public static class SessionValidator
{
    private const string SignedInAtKey = "shelver.signed-in-at";
    private const string LastActivityKey = "shelver.last-activity";

    public static Task StampSignInAsync(CookieSigningInContext context)
    {
        string now = FormatTime(context.HttpContext.RequestServices.GetRequiredService<TimeProvider>().GetUtcNow());
        context.Properties.Items[SignedInAtKey] = now;
        context.Properties.Items[LastActivityKey] = now;
        return Task.CompletedTask;
    }

    // This replaces Identity's own check, which compares the security stamp only every 30
    // minutes. Checking every request makes deactivation and role changes apply immediately.
    public static async Task ValidateAsync(CookieValidatePrincipalContext context)
    {
        IServiceProvider services = context.HttpContext.RequestServices;
        SignInManager<ShelverUser> signInManager = services.GetRequiredService<SignInManager<ShelverUser>>();
        SignOutLimitsOptions limits = services.GetRequiredService<IOptions<SignOutLimitsOptions>>().Value;
        DateTimeOffset now = services.GetRequiredService<TimeProvider>().GetUtcNow();

        ShelverUser? user = context.Principal == null ? null : await signInManager.ValidateSecurityStampAsync(context.Principal);
        if (user == null || !user.IsActive || !TryReadTime(context.Properties, SignedInAtKey, out DateTimeOffset signedInAt) || !TryReadTime(context.Properties, LastActivityKey, out DateTimeOffset lastActivity))
        {
            await RejectAsync(context);
            return;
        }

        IList<string> roleNames = await signInManager.UserManager.GetRolesAsync(user);
        RoleSignOutLimits roleLimits = limits.ForRoles(roleNames);
        if (now - lastActivity > roleLimits.Idle || now - signedInAt > roleLimits.Absolute)
        {
            await RejectAsync(context);
            return;
        }

        // The cookie holds the roles from sign-in, so it is rebuilt when they have changed since.
        HashSet<string> cookieRoleNames = context.Principal!.FindAll(ClaimTypes.Role).Select((Claim claim) => claim.Value).ToHashSet();
        if (!cookieRoleNames.SetEquals(roleNames))
        {
            context.ReplacePrincipal(await signInManager.CreateUserPrincipalAsync(user));
        }

        context.Properties.Items[LastActivityKey] = FormatTime(now);
        context.ShouldRenew = true;
    }

    private static async Task RejectAsync(CookieValidatePrincipalContext context)
    {
        context.RejectPrincipal();
        await context.HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
    }

    private static string FormatTime(DateTimeOffset time)
    {
        return time.ToString("O", CultureInfo.InvariantCulture);
    }

    private static bool TryReadTime(AuthenticationProperties properties, string key, out DateTimeOffset time)
    {
        time = default;
        return properties.Items.TryGetValue(key, out string? text) && DateTimeOffset.TryParse(text, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out time);
    }
}
