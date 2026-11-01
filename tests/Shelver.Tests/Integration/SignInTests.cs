/***********************************************************************************************
*
*   SignInTests.cs
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
using Microsoft.AspNetCore.Mvc.Testing;
using Shelver.Models;

namespace Shelver.Tests.Integration;

public class SignInTests
{
    private const string Pin = "135790";

    [Fact]
    public async Task GetSignIn_ActiveAndDeactivatedAccounts_ListsOnlyActive()
    {
        using ShelverWebApplicationFactory factory = new ShelverWebApplicationFactory();
        await factory.CreateUserAsync("active", Pin, Roles.Auditor);
        await factory.CreateUserAsync("former", Pin, Roles.Auditor, isActive: false);
        HttpClient client = factory.CreateClient();

        string body = await client.GetStringAsync("/Account/SignIn");

        Assert.Contains("/Account/SignIn/active", body);
        Assert.DoesNotContain("former", body);
    }

    [Fact]
    public async Task PostPin_CorrectPin_SignsInAndRedirectsHome()
    {
        using ShelverWebApplicationFactory factory = new ShelverWebApplicationFactory();
        await factory.CreateUserAsync("auditor", Pin, Roles.Auditor);
        HttpClient client = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

        HttpResponseMessage response = await ShelverWebApplicationFactory.PostPinAsync(client, "auditor", Pin);
        string homePage = await client.GetStringAsync("/");

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Equal("/", response.Headers.Location?.OriginalString);
        Assert.Contains("Sign out", homePage);
    }

    [Fact]
    public async Task PostPin_CorrectPin_IssuesSessionCookie()
    {
        using ShelverWebApplicationFactory factory = new ShelverWebApplicationFactory();
        await factory.CreateUserAsync("auditor", Pin, Roles.Auditor);
        HttpClient client = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

        HttpResponseMessage response = await ShelverWebApplicationFactory.PostPinAsync(client, "auditor", Pin);
        string authenticationCookie = response.Headers.GetValues("Set-Cookie").Single((string cookie) => cookie.StartsWith(".AspNetCore.Identity.Application="));

        // A cookie without an expiry date is dropped when the browser closes.
        Assert.DoesNotContain("expires=", authenticationCookie, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("max-age=", authenticationCookie, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task PostPin_LocalReturnUrl_RedirectsThere()
    {
        using ShelverWebApplicationFactory factory = new ShelverWebApplicationFactory();
        await factory.CreateUserAsync("auditor", Pin, Roles.Auditor);
        HttpClient client = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

        HttpResponseMessage response = await ShelverWebApplicationFactory.PostPinAsync(client, "auditor", Pin, "/FloorPlans");

        Assert.Equal("/FloorPlans", response.Headers.Location?.OriginalString);
    }

    [Fact]
    public async Task PostPin_ExternalReturnUrl_RedirectsHome()
    {
        using ShelverWebApplicationFactory factory = new ShelverWebApplicationFactory();
        await factory.CreateUserAsync("auditor", Pin, Roles.Auditor);
        HttpClient client = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

        HttpResponseMessage response = await ShelverWebApplicationFactory.PostPinAsync(client, "auditor", Pin, "https://example.com/");

        Assert.Equal("/", response.Headers.Location?.OriginalString);
    }

    [Fact]
    public async Task PostPin_WrongPin_ShowsErrorAndStaysSignedOut()
    {
        using ShelverWebApplicationFactory factory = new ShelverWebApplicationFactory();
        await factory.CreateUserAsync("auditor", Pin, Roles.Auditor);
        HttpClient client = factory.CreateClient();

        HttpResponseMessage response = await ShelverWebApplicationFactory.PostPinAsync(client, "auditor", "000000");
        string body = await response.Content.ReadAsStringAsync();
        string homePage = await client.GetStringAsync("/");

        Assert.Contains("Wrong PIN.", body);
        Assert.DoesNotContain("Sign out", homePage);
    }

    [Fact]
    public async Task PostPin_FiveWrongPins_LocksAccountEvenForCorrectPin()
    {
        using ShelverWebApplicationFactory factory = new ShelverWebApplicationFactory();
        await factory.CreateUserAsync("auditor", Pin, Roles.Auditor);
        HttpClient client = factory.CreateClient();

        for (int attempt = 0; attempt < 5; attempt++)
        {
            await ShelverWebApplicationFactory.PostPinAsync(client, "auditor", "000000");
        }
        HttpResponseMessage response = await ShelverWebApplicationFactory.PostPinAsync(client, "auditor", Pin);
        string body = await response.Content.ReadAsStringAsync();

        Assert.Contains("Too many wrong PINs. Try again in 5 minutes.", body);
    }

    [Fact]
    public async Task PostPin_DeactivatedAccount_RedirectsToListWithoutSigningIn()
    {
        using ShelverWebApplicationFactory factory = new ShelverWebApplicationFactory();
        await factory.CreateUserAsync("auditor", Pin, Roles.Auditor);
        await factory.CreateUserAsync("former", Pin, Roles.Auditor, isActive: false);
        HttpClient client = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

        // The form is loaded from an active account's page, since a deactivated one has none.
        string pinPage = await client.GetStringAsync("/Account/SignIn/auditor");
        Dictionary<string, string> fields = new Dictionary<string, string> { ["Pin"] = Pin, ["__RequestVerificationToken"] = ShelverWebApplicationFactory.ReadAntiforgeryToken(pinPage) };
        HttpResponseMessage response = await client.PostAsync("/Account/SignIn/former", new FormUrlEncodedContent(fields));
        string homePage = await client.GetStringAsync("/");

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.StartsWith("/Account/SignIn", response.Headers.Location?.OriginalString);
        Assert.DoesNotContain("Sign out", homePage);
    }

    [Fact]
    public async Task PostSignOut_SignedIn_EndsSession()
    {
        using ShelverWebApplicationFactory factory = new ShelverWebApplicationFactory();
        await factory.CreateUserAsync("auditor", Pin, Roles.Auditor);
        HttpClient client = factory.CreateClient();
        await ShelverWebApplicationFactory.PostPinAsync(client, "auditor", Pin);

        string homePage = await client.GetStringAsync("/");
        Dictionary<string, string> fields = new Dictionary<string, string> { ["__RequestVerificationToken"] = ShelverWebApplicationFactory.ReadAntiforgeryToken(homePage) };
        await client.PostAsync("/Account/SignOut", new FormUrlEncodedContent(fields));
        string homePageAfter = await client.GetStringAsync("/");

        Assert.Contains("Sign out", homePage);
        Assert.DoesNotContain("Sign out", homePageAfter);
    }
}
