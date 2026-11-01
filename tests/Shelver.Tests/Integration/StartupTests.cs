/***********************************************************************************************
*
*   StartupTests.cs
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
using Microsoft.Extensions.DependencyInjection;
using Shelver.Models;

namespace Shelver.Tests.Integration;

public class StartupTests
{
    [Theory]
    [InlineData(Roles.Auditor)]
    [InlineData(Roles.Administrator)]
    public async Task Startup_EmptyDatabase_CreatesRole(string roleName)
    {
        using ShelverWebApplicationFactory factory = new ShelverWebApplicationFactory();

        // Reading Services starts the app, which runs the same startup code as a real launch.
        using IServiceScope scope = factory.Services.CreateScope();
        RoleManager<IdentityRole> roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        Assert.True(await roleManager.RoleExistsAsync(roleName));
    }
}
