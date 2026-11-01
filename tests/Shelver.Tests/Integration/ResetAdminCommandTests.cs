/***********************************************************************************************
*
*   ResetAdminCommandTests.cs
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
using Shelver.Accounts;
using Shelver.Models;

namespace Shelver.Tests.Integration;

// The command is run against the test app's services, the same ones the real command-line
// mode builds, so it sees the same Identity rules.
public class ResetAdminCommandTests
{
    private const string Pin = "112233";
    private const string NewPin = "445566";

    [Fact]
    public async Task Run_NewUsername_CreatesAdministratorWhoCanSignIn()
    {
        using ShelverWebApplicationFactory factory = new ShelverWebApplicationFactory();

        int exitCode = await RunAsync(factory, Pin, "boss");
        HttpClient client = factory.CreateClient();
        await ShelverWebApplicationFactory.PostPinAsync(client, "boss", Pin);
        string floorPlansPage = await client.GetStringAsync("/FloorPlans");

        Assert.Equal(ResetAdminCommand.Succeeded, exitCode);
        Assert.Contains("Create floor plan", floorPlansPage);
    }

    [Fact]
    public async Task Run_ExistingAdministrator_ReplacesPin()
    {
        using ShelverWebApplicationFactory factory = new ShelverWebApplicationFactory();
        await factory.CreateUserAsync("boss", Pin, Roles.Administrator);

        int exitCode = await RunAsync(factory, NewPin, "boss");

        Assert.Equal(ResetAdminCommand.Succeeded, exitCode);
        Assert.False(await CheckPinAsync(factory, "boss", Pin));
        Assert.True(await CheckPinAsync(factory, "boss", NewPin));
    }

    [Fact]
    public async Task Run_DeactivatedAndLockedAdministrator_ReactivatesAndUnlocks()
    {
        using ShelverWebApplicationFactory factory = new ShelverWebApplicationFactory();
        await factory.CreateUserAsync("boss", Pin, Roles.Administrator, isActive: false);
        using (IServiceScope scope = factory.Services.CreateScope())
        {
            UserManager<ShelverUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<ShelverUser>>();
            await userManager.SetLockoutEndDateAsync((await userManager.FindByNameAsync("boss"))!, DateTimeOffset.UtcNow.AddHours(1));
        }

        int exitCode = await RunAsync(factory, NewPin, "boss");

        using IServiceScope checkScope = factory.Services.CreateScope();
        UserManager<ShelverUser> checkUserManager = checkScope.ServiceProvider.GetRequiredService<UserManager<ShelverUser>>();
        ShelverUser user = (await checkUserManager.FindByNameAsync("boss"))!;
        Assert.Equal(ResetAdminCommand.Succeeded, exitCode);
        Assert.True(user.IsActive);
        Assert.False(await checkUserManager.IsLockedOutAsync(user));
    }

    [Fact]
    public async Task Run_ExistingAuditor_RefusesAndLeavesAccountUnchanged()
    {
        using ShelverWebApplicationFactory factory = new ShelverWebApplicationFactory();
        await factory.CreateUserAsync("helper", Pin, Roles.Auditor);

        int exitCode = await RunAsync(factory, NewPin, "helper");

        Assert.Equal(ResetAdminCommand.Failed, exitCode);
        Assert.True(await CheckPinAsync(factory, "helper", Pin));
    }

    [Theory]
    [InlineData("12345")]
    [InlineData("abcdef")]
    [InlineData("")]
    public async Task Run_InvalidPin_FailsWithoutCreatingAccount(string pin)
    {
        using ShelverWebApplicationFactory factory = new ShelverWebApplicationFactory();

        int exitCode = await RunAsync(factory, pin, "boss");

        using IServiceScope scope = factory.Services.CreateScope();
        UserManager<ShelverUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<ShelverUser>>();
        Assert.Equal(ResetAdminCommand.Failed, exitCode);
        Assert.Null(await userManager.FindByNameAsync("boss"));
    }

    [Theory]
    [InlineData()]
    [InlineData("boss", "extra")]
    public async Task Run_WrongArgumentCount_ReturnsUsageError(params string[] arguments)
    {
        using ShelverWebApplicationFactory factory = new ShelverWebApplicationFactory();

        int exitCode = await RunAsync(factory, Pin, arguments);

        Assert.Equal(ResetAdminCommand.UsageError, exitCode);
    }

    private static async Task<int> RunAsync(ShelverWebApplicationFactory factory, string pin, params string[] arguments)
    {
        return await ResetAdminCommand.RunAsync(factory.Services, arguments, new StringReader($"{pin}\n"), new StringWriter(), new StringWriter());
    }

    private static async Task<bool> CheckPinAsync(ShelverWebApplicationFactory factory, string username, string pin)
    {
        using IServiceScope scope = factory.Services.CreateScope();
        UserManager<ShelverUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<ShelverUser>>();
        return await userManager.CheckPasswordAsync((await userManager.FindByNameAsync(username))!, pin);
    }
}
