/***********************************************************************************************
*
*   PinValidator.cs
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

using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity;
using Shelver.Models;

namespace Shelver.Accounts;

// Identity calls an account's credential a password. In shelver it is always a PIN of exactly
// six digits, so this runs alongside Identity's own validator, whose rules are all turned off.
public partial class PinValidator : IPasswordValidator<ShelverUser>
{
    public const int PinLength = 6;

    public Task<IdentityResult> ValidateAsync(UserManager<ShelverUser> manager, ShelverUser user, string? password)
    {
        if (password != null && SixDigits().IsMatch(password))
        {
            return Task.FromResult(IdentityResult.Success);
        }
        return Task.FromResult(IdentityResult.Failed(new IdentityError { Code = "PinFormat", Description = $"The PIN must be exactly {PinLength} digits." }));
    }

    [GeneratedRegex(@"^[0-9]{6}$")]
    private static partial Regex SixDigits();
}
