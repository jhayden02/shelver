/***********************************************************************************************
*
*   PinValidatorTests.cs
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
using Shelver.Accounts;
using Shelver.Models;

namespace Shelver.Tests.Accounts;

public class PinValidatorTests
{
    // The validator never uses the manager, so none is needed to call it.
    [Theory]
    [InlineData("123456", true)]
    [InlineData("000000", true)]
    [InlineData("12345", false)]
    [InlineData("1234567", false)]
    [InlineData("12345a", false)]
    [InlineData("12 456", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    public async Task ValidateAsync_Pin_AcceptsOnlySixDigits(string? pin, bool isValid)
    {
        PinValidator validator = new PinValidator();

        IdentityResult result = await validator.ValidateAsync(null!, new ShelverUser(), pin);

        Assert.Equal(isValid, result.Succeeded);
    }
}
