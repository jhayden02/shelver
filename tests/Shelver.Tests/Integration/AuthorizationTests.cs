/***********************************************************************************************
*
*   AuthorizationTests.cs
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

public class AuthorizationTests
{
    [Theory]
    [InlineData("/")]
    [InlineData("/FloorPlans")]
    public async Task GetPage_SignedOut_RedirectsToSignInWithReturnUrl(string path)
    {
        using ShelverWebApplicationFactory factory = new ShelverWebApplicationFactory();
        HttpClient client = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

        HttpResponseMessage response = await client.GetAsync(path);

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Equal($"/Account/SignIn?ReturnUrl={Uri.EscapeDataString(path)}", response.Headers.Location?.PathAndQuery);
    }

    [Theory]
    [InlineData("/Account/SignIn")]
    [InlineData("/Home/Error")]
    [InlineData("/css/site.css")]
    public async Task GetPublicPath_SignedOut_ReturnsOk(string path)
    {
        using ShelverWebApplicationFactory factory = new ShelverWebApplicationFactory();
        HttpClient client = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

        HttpResponseMessage response = await client.GetAsync(path);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetFloorPlans_Auditor_RedirectsToAccessDenied()
    {
        using ShelverWebApplicationFactory factory = new ShelverWebApplicationFactory();
        HttpClient client = await factory.CreateSignedInClientAsync(Roles.Auditor, new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

        HttpResponseMessage response = await client.GetAsync("/FloorPlans");

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.StartsWith("/Account/AccessDenied", response.Headers.Location?.PathAndQuery);
    }

    [Fact]
    public async Task GetFloorPlans_Administrator_ReturnsOk()
    {
        using ShelverWebApplicationFactory factory = new ShelverWebApplicationFactory();
        HttpClient client = await factory.CreateSignedInClientAsync(Roles.Administrator, new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

        HttpResponseMessage response = await client.GetAsync("/FloorPlans");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Theory]
    [InlineData(Roles.Auditor, false)]
    [InlineData(Roles.Administrator, true)]
    public async Task GetHome_Role_ShowsFloorPlansLinkOnlyToAdministrators(string roleName, bool showsLink)
    {
        using ShelverWebApplicationFactory factory = new ShelverWebApplicationFactory();
        HttpClient client = await factory.CreateSignedInClientAsync(roleName);

        string body = await client.GetStringAsync("/");

        Assert.Equal(showsLink, body.Contains("href=\"/FloorPlans\""));
    }
}
