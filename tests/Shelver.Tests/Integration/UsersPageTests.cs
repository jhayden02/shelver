/***********************************************************************************************
*
*   UsersPageTests.cs
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

using System.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Shelver.Models;

namespace Shelver.Tests.Integration;

// Each test signs in as an account named "administrator", created by CreateSignedInClientAsync.
public class UsersPageTests
{
    private const string Pin = "975310";

    [Fact]
    public async Task GetIndex_Auditor_RedirectsToAccessDenied()
    {
        using ShelverWebApplicationFactory factory = new ShelverWebApplicationFactory();
        HttpClient client = await factory.CreateSignedInClientAsync(Roles.Auditor, new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

        HttpResponseMessage response = await client.GetAsync("/Users");

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.StartsWith("/Account/AccessDenied", response.Headers.Location?.PathAndQuery);
    }

    [Fact]
    public async Task GetIndex_Accounts_ListsRolesAndStatus()
    {
        using ShelverWebApplicationFactory factory = new ShelverWebApplicationFactory();
        await factory.CreateUserAsync("former", Pin, Roles.Auditor, isActive: false);
        HttpClient client = await factory.CreateSignedInClientAsync(Roles.Administrator);

        string body = await client.GetStringAsync("/Users");

        Assert.Contains("<td>former</td>", body);
        Assert.Contains("Deactivated", body);
        Assert.Contains("<td>administrator</td>", body);
        Assert.Contains("<td>Administrator</td>", body);
    }

    [Fact]
    public async Task PostCreate_ValidForm_CreatesAccountThatCanSignIn()
    {
        using ShelverWebApplicationFactory factory = new ShelverWebApplicationFactory();
        HttpClient client = await factory.CreateSignedInClientAsync(Roles.Administrator);

        await ShelverWebApplicationFactory.PostFormAsync(client, "/Users/Create", "/Users/Create", new Dictionary<string, string> { ["Username"] = "newhire", ["RoleName"] = Roles.Auditor, ["Pin"] = Pin });

        Assert.Equal(Roles.Auditor, await GetRoleAsync(factory, "newhire"));
        Assert.True(await CheckPinAsync(factory, "newhire", Pin));
    }

    [Theory]
    [InlineData("administrator", "975310", "is already taken")]
    [InlineData("newhire", "12345", "The PIN is 6 digits.")]
    [InlineData("new hire", "975310", "is invalid")]
    public async Task PostCreate_InvalidForm_ShowsErrorWithoutCreating(string username, string pin, string expectedError)
    {
        using ShelverWebApplicationFactory factory = new ShelverWebApplicationFactory();
        HttpClient client = await factory.CreateSignedInClientAsync(Roles.Administrator);

        HttpResponseMessage response = await ShelverWebApplicationFactory.PostFormAsync(client, "/Users/Create", "/Users/Create", new Dictionary<string, string> { ["Username"] = username, ["RoleName"] = Roles.Auditor, ["Pin"] = pin });
        string body = await response.Content.ReadAsStringAsync();

        Assert.Contains(expectedError, body);
        Assert.Equal(new[] { "administrator" }, await GetAllUsernamesAsync(factory));
    }

    [Fact]
    public async Task PostRename_NewUsername_RenamesAccount()
    {
        using ShelverWebApplicationFactory factory = new ShelverWebApplicationFactory();
        ShelverUser user = await factory.CreateUserAsync("helper", Pin, Roles.Auditor);
        HttpClient client = await factory.CreateSignedInClientAsync(Roles.Administrator);

        await PostEditFormAsync(client, user.Id, "Rename", new Dictionary<string, string> { ["username"] = "assistant" });

        Assert.Equal(new[] { "administrator", "assistant" }, await GetAllUsernamesAsync(factory));
    }

    [Fact]
    public async Task PostChangeRole_AuditorToAdministrator_ChangesRole()
    {
        using ShelverWebApplicationFactory factory = new ShelverWebApplicationFactory();
        ShelverUser user = await factory.CreateUserAsync("helper", Pin, Roles.Auditor);
        HttpClient client = await factory.CreateSignedInClientAsync(Roles.Administrator);

        await PostEditFormAsync(client, user.Id, "ChangeRole", new Dictionary<string, string> { ["roleName"] = Roles.Administrator });

        Assert.Equal(Roles.Administrator, await GetRoleAsync(factory, "helper"));
    }

    [Fact]
    public async Task PostChangeRole_LastActiveAdministrator_RefusesDemotion()
    {
        using ShelverWebApplicationFactory factory = new ShelverWebApplicationFactory();
        HttpClient client = await factory.CreateSignedInClientAsync(Roles.Administrator);
        string id = await GetIdAsync(factory, "administrator");

        HttpResponseMessage response = await PostEditFormAsync(client, id, "ChangeRole", new Dictionary<string, string> { ["roleName"] = Roles.Auditor });
        string body = await response.Content.ReadAsStringAsync();

        Assert.Contains("is the last active Administrator, so it cannot be demoted", body);
        Assert.Equal(Roles.Administrator, await GetRoleAsync(factory, "administrator"));
    }

    [Fact]
    public async Task PostChangeRole_AnotherActiveAdministratorExists_AllowsDemotion()
    {
        using ShelverWebApplicationFactory factory = new ShelverWebApplicationFactory();
        ShelverUser other = await factory.CreateUserAsync("other", Pin, Roles.Administrator);
        HttpClient client = await factory.CreateSignedInClientAsync(Roles.Administrator);

        await PostEditFormAsync(client, other.Id, "ChangeRole", new Dictionary<string, string> { ["roleName"] = Roles.Auditor });

        Assert.Equal(Roles.Auditor, await GetRoleAsync(factory, "other"));
    }

    [Fact]
    public async Task PostSetActive_LastActiveAdministrator_RefusesDeactivation()
    {
        using ShelverWebApplicationFactory factory = new ShelverWebApplicationFactory();
        await factory.CreateUserAsync("former", Pin, Roles.Administrator, isActive: false);
        HttpClient client = await factory.CreateSignedInClientAsync(Roles.Administrator);
        string id = await GetIdAsync(factory, "administrator");

        HttpResponseMessage response = await PostEditFormAsync(client, id, "SetActive", new Dictionary<string, string> { ["isActive"] = "false" });
        string body = await response.Content.ReadAsStringAsync();

        Assert.Contains("is the last active Administrator, so it cannot be deactivated", body);
        Assert.True((await FindAsync(factory, "administrator")).IsActive);
    }

    [Fact]
    public async Task PostSetActive_Auditor_DeactivatesThenReactivates()
    {
        using ShelverWebApplicationFactory factory = new ShelverWebApplicationFactory();
        ShelverUser user = await factory.CreateUserAsync("helper", Pin, Roles.Auditor);
        HttpClient client = await factory.CreateSignedInClientAsync(Roles.Administrator);

        await PostEditFormAsync(client, user.Id, "SetActive", new Dictionary<string, string> { ["isActive"] = "false" });
        bool isActiveAfterDeactivate = (await FindAsync(factory, "helper")).IsActive;
        await PostEditFormAsync(client, user.Id, "SetActive", new Dictionary<string, string> { ["isActive"] = "true" });
        bool isActiveAfterReactivate = (await FindAsync(factory, "helper")).IsActive;

        Assert.False(isActiveAfterDeactivate);
        Assert.True(isActiveAfterReactivate);
    }

    [Fact]
    public async Task PostResetPin_LockedAccount_ReplacesPinAndUnlocks()
    {
        using ShelverWebApplicationFactory factory = new ShelverWebApplicationFactory();
        ShelverUser user = await factory.CreateUserAsync("helper", Pin, Roles.Auditor);
        using (IServiceScope scope = factory.Services.CreateScope())
        {
            UserManager<ShelverUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<ShelverUser>>();
            await userManager.SetLockoutEndDateAsync((await userManager.FindByNameAsync("helper"))!, DateTimeOffset.UtcNow.AddHours(1));
        }
        HttpClient client = await factory.CreateSignedInClientAsync(Roles.Administrator);

        await PostEditFormAsync(client, user.Id, "ResetPin", new Dictionary<string, string> { ["pin"] = "202020" });

        using IServiceScope checkScope = factory.Services.CreateScope();
        UserManager<ShelverUser> checkUserManager = checkScope.ServiceProvider.GetRequiredService<UserManager<ShelverUser>>();
        ShelverUser updated = (await checkUserManager.FindByNameAsync("helper"))!;
        Assert.True(await checkUserManager.CheckPasswordAsync(updated, "202020"));
        Assert.False(await checkUserManager.IsLockedOutAsync(updated));
    }

    [Fact]
    public async Task PostResetPin_InvalidPin_KeepsOldPin()
    {
        using ShelverWebApplicationFactory factory = new ShelverWebApplicationFactory();
        ShelverUser user = await factory.CreateUserAsync("helper", Pin, Roles.Auditor);
        HttpClient client = await factory.CreateSignedInClientAsync(Roles.Administrator);

        HttpResponseMessage response = await PostEditFormAsync(client, user.Id, "ResetPin", new Dictionary<string, string> { ["pin"] = "12ab56" });
        string body = await response.Content.ReadAsStringAsync();

        Assert.Contains("The PIN must be exactly 6 digits.", body);
        Assert.True(await CheckPinAsync(factory, "helper", Pin));
    }

    private static Task<HttpResponseMessage> PostEditFormAsync(HttpClient client, string id, string action, Dictionary<string, string> fields)
    {
        return ShelverWebApplicationFactory.PostFormAsync(client, $"/Users/Edit/{id}", $"/Users/{action}/{id}", fields);
    }

    private static async Task<ShelverUser> FindAsync(ShelverWebApplicationFactory factory, string username)
    {
        using IServiceScope scope = factory.Services.CreateScope();
        UserManager<ShelverUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<ShelverUser>>();
        return (await userManager.FindByNameAsync(username))!;
    }

    private static async Task<string> GetIdAsync(ShelverWebApplicationFactory factory, string username)
    {
        return (await FindAsync(factory, username)).Id;
    }

    private static async Task<string?> GetRoleAsync(ShelverWebApplicationFactory factory, string username)
    {
        using IServiceScope scope = factory.Services.CreateScope();
        UserManager<ShelverUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<ShelverUser>>();
        return (await userManager.GetRolesAsync((await userManager.FindByNameAsync(username))!)).SingleOrDefault();
    }

    private static async Task<bool> CheckPinAsync(ShelverWebApplicationFactory factory, string username, string pin)
    {
        using IServiceScope scope = factory.Services.CreateScope();
        UserManager<ShelverUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<ShelverUser>>();
        return await userManager.CheckPasswordAsync((await userManager.FindByNameAsync(username))!, pin);
    }

    private static async Task<List<string>> GetAllUsernamesAsync(ShelverWebApplicationFactory factory)
    {
        using IServiceScope scope = factory.Services.CreateScope();
        UserManager<ShelverUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<ShelverUser>>();
        return userManager.Users.Select((ShelverUser user) => user.UserName!).OrderBy((string username) => username).ToList();
    }
}
