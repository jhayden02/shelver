/***********************************************************************************************
*
*   SessionTests.cs
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

// These use the default limits from appsettings.json: 15 minutes idle for Auditors, an hour
// for Administrators, and 12 hours since sign-in for both.
public class SessionTests
{
    private static readonly WebApplicationFactoryClientOptions NoRedirects = new WebApplicationFactoryClientOptions { AllowAutoRedirect = false };

    [Theory]
    [InlineData(Roles.Auditor, 14, true)]
    [InlineData(Roles.Auditor, 16, false)]
    [InlineData(Roles.Administrator, 59, true)]
    [InlineData(Roles.Administrator, 61, false)]
    public async Task GetHome_AfterIdleMinutes_StaysSignedInOnlyWithinRoleLimit(string roleName, int idleMinutes, bool staysSignedIn)
    {
        using ShelverWebApplicationFactory factory = new ShelverWebApplicationFactory();
        HttpClient client = await factory.CreateSignedInClientAsync(roleName, NoRedirects);

        factory.Clock.Advance(TimeSpan.FromMinutes(idleMinutes));
        HttpResponseMessage response = await client.GetAsync("/");

        Assert.Equal(staysSignedIn, response.StatusCode == HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetHome_ActiveForOverTwelveHours_SignsOut()
    {
        using ShelverWebApplicationFactory factory = new ShelverWebApplicationFactory();
        HttpClient client = await factory.CreateSignedInClientAsync(Roles.Auditor, NoRedirects);

        // A request every 10 minutes keeps the idle limit from ending the session first.
        for (int elapsedMinutes = 10; elapsedMinutes <= 11 * 60 + 50; elapsedMinutes += 10)
        {
            factory.Clock.Advance(TimeSpan.FromMinutes(10));
            Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/")).StatusCode);
        }
        factory.Clock.Advance(TimeSpan.FromMinutes(11));
        HttpResponseMessage response = await client.GetAsync("/");

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.StartsWith("/Account/SignIn", response.Headers.Location?.PathAndQuery);
    }

    [Fact]
    public async Task GetHome_AccountDeactivatedWhileSignedIn_SignsOut()
    {
        using ShelverWebApplicationFactory factory = new ShelverWebApplicationFactory();
        HttpClient client = await factory.CreateSignedInClientAsync(Roles.Auditor, NoRedirects);

        using (IServiceScope scope = factory.Services.CreateScope())
        {
            UserManager<ShelverUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<ShelverUser>>();
            ShelverUser user = (await userManager.FindByNameAsync("auditor"))!;
            user.IsActive = false;
            await userManager.UpdateAsync(user);
        }
        HttpResponseMessage response = await client.GetAsync("/");

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.StartsWith("/Account/SignIn", response.Headers.Location?.PathAndQuery);
    }

    [Fact]
    public async Task GetFloorPlans_AdministratorDemotedWhileSignedIn_DeniesAccess()
    {
        using ShelverWebApplicationFactory factory = new ShelverWebApplicationFactory();
        HttpClient client = await factory.CreateSignedInClientAsync(Roles.Administrator, NoRedirects);
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/FloorPlans")).StatusCode);

        using (IServiceScope scope = factory.Services.CreateScope())
        {
            UserManager<ShelverUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<ShelverUser>>();
            ShelverUser user = (await userManager.FindByNameAsync("administrator"))!;
            await userManager.RemoveFromRoleAsync(user, Roles.Administrator);
            await userManager.AddToRoleAsync(user, Roles.Auditor);
        }
        HttpResponseMessage response = await client.GetAsync("/FloorPlans");

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.StartsWith("/Account/AccessDenied", response.Headers.Location?.PathAndQuery);
    }
}
