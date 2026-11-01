/***********************************************************************************************
*
*   ResetAdminCommand.cs
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

using Microsoft.AspNetCore.Identity;
using Shelver.Models;

namespace Shelver.Accounts;

// "reset-admin <username>" creates an Administrator, or resets an existing one's PIN and makes
// it able to sign in again. The PIN is read from standard input rather than the arguments,
// because arguments are visible to other users in the process list.
public static class ResetAdminCommand
{
    public const string Name = "reset-admin";

    public const int Succeeded = 0;
    public const int Failed = 1;
    public const int UsageError = 2;

    public static async Task<int> RunAsync(IServiceProvider services, IReadOnlyList<string> arguments, TextReader input, TextWriter output, TextWriter error)
    {
        if (arguments.Count != 1)
        {
            await error.WriteLineAsync($"Usage: {Name} <username>, with the PIN on standard input.");
            return UsageError;
        }
        string username = arguments[0];
        string pin = (await input.ReadLineAsync() ?? "").Trim();

        using IServiceScope scope = services.CreateScope();
        UserManager<ShelverUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<ShelverUser>>();

        // The PIN is checked before anything changes, so a bad PIN never leaves an account
        // half reset.
        List<string> pinErrors = new List<string>();
        foreach (IPasswordValidator<ShelverUser> validator in userManager.PasswordValidators)
        {
            IdentityResult validation = await validator.ValidateAsync(userManager, new ShelverUser { UserName = username }, pin);
            pinErrors.AddRange(validation.Errors.Select((IdentityError identityError) => identityError.Description));
        }
        if (pinErrors.Count > 0)
        {
            await error.WriteLineAsync(string.Join(" ", pinErrors.Distinct()));
            return Failed;
        }

        ShelverUser? user = await userManager.FindByNameAsync(username);
        if (user == null)
        {
            ShelverUser newUser = new ShelverUser { UserName = username };
            if (!await SucceedsAsync(error, userManager.CreateAsync(newUser, pin)) || !await SucceedsAsync(error, userManager.AddToRoleAsync(newUser, Roles.Administrator)))
            {
                return Failed;
            }
            await output.WriteLineAsync($"Created Administrator {newUser.UserName}.");
            return Succeeded;
        }

        if (!await userManager.IsInRoleAsync(user, Roles.Administrator))
        {
            await error.WriteLineAsync($"{user.UserName} is not an Administrator. Choose another username.");
            return Failed;
        }

        // Replacing the PIN also updates the security stamp, which signs out every session.
        user.IsActive = true;
        bool reset = await SucceedsAsync(error, userManager.UpdateAsync(user)) && await SucceedsAsync(error, userManager.RemovePasswordAsync(user)) && await SucceedsAsync(error, userManager.AddPasswordAsync(user, pin)) && await SucceedsAsync(error, userManager.SetLockoutEndDateAsync(user, null)) && await SucceedsAsync(error, userManager.ResetAccessFailedCountAsync(user));
        if (!reset)
        {
            return Failed;
        }
        await output.WriteLineAsync($"Reset the PIN of Administrator {user.UserName}.");
        return Succeeded;
    }

    private static async Task<bool> SucceedsAsync(TextWriter error, Task<IdentityResult> operation)
    {
        IdentityResult result = await operation;
        if (!result.Succeeded)
        {
            await error.WriteLineAsync(string.Join(" ", result.Errors.Select((IdentityError identityError) => identityError.Description)));
        }
        return result.Succeeded;
    }
}
