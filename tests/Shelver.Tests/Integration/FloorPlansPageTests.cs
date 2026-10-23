/***********************************************************************************************
*
*   FloorPlansPageTests.cs
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
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shelver.Data;
using Shelver.Models;

namespace Shelver.Tests.Integration;

// Each test starts the whole app with its own empty database, so tests never see each
// other's floor plans.
public class FloorPlansPageTests
{
    [Fact]
    public async Task GetIndex_NoFloorPlans_ShowsOnlyCreateButton()
    {
        using ShelverWebApplicationFactory factory = new ShelverWebApplicationFactory();
        HttpClient client = factory.CreateClient();

        HttpResponseMessage response = await client.GetAsync("/FloorPlans");
        string body = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("floor-plan-empty", body);
        Assert.Contains("Create floor plan", body);
        Assert.DoesNotContain("<table", body);
    }

    [Fact]
    public async Task PostCreate_ValidForm_RedirectsToEdit()
    {
        using ShelverWebApplicationFactory factory = new ShelverWebApplicationFactory();
        HttpClient client = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

        HttpResponseMessage response = await PostCreateAsync(client, "Warehouse", "2000", "1200");

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Matches(@"^/FloorPlans/Edit/\d+$", response.Headers.Location?.OriginalString);
    }

    [Fact]
    public async Task PostCreate_ValidForm_SavesFloorPlanInSetupState()
    {
        using ShelverWebApplicationFactory factory = new ShelverWebApplicationFactory();
        HttpClient client = factory.CreateClient();

        await PostCreateAsync(client, "Warehouse", "2000", "1200");

        using IServiceScope scope = factory.Services.CreateScope();
        ShelverDbContext database = scope.ServiceProvider.GetRequiredService<ShelverDbContext>();
        FloorPlan plan = await database.FloorPlans.SingleAsync();
        Assert.Equal("Warehouse", plan.Name);
        Assert.Equal(2000, plan.WidthCm);
        Assert.Equal(1200, plan.HeightCm);
        Assert.Equal(FloorPlanState.Setup, plan.State);
    }

    [Fact]
    public async Task PostCreate_BlankName_ShowsValidationError()
    {
        using ShelverWebApplicationFactory factory = new ShelverWebApplicationFactory();
        HttpClient client = factory.CreateClient();

        HttpResponseMessage response = await PostCreateAsync(client, "", "2000", "1200");
        string body = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("The Name field is required.", body);
    }

    [Fact]
    public async Task PostCreate_WidthOutOfRange_ShowsValidationError()
    {
        using ShelverWebApplicationFactory factory = new ShelverWebApplicationFactory();
        HttpClient client = factory.CreateClient();

        HttpResponseMessage response = await PostCreateAsync(client, "Warehouse", "0", "1200");
        string body = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("The field Width (cm) must be between 100 and 100000.", body);
    }

    [Fact]
    public async Task GetIndex_AfterCreate_ListsFloorPlan()
    {
        using ShelverWebApplicationFactory factory = new ShelverWebApplicationFactory();
        HttpClient client = factory.CreateClient();
        await PostCreateAsync(client, "Warehouse", "2000", "1200");

        HttpResponseMessage response = await client.GetAsync("/FloorPlans");
        string body = await response.Content.ReadAsStringAsync();

        Assert.Contains("<table", body);
        Assert.Contains("Warehouse", body);
        Assert.DoesNotContain("floor-plan-empty", body);
    }

    [Fact]
    public async Task GetEdit_UnknownId_ReturnsNotFound()
    {
        using ShelverWebApplicationFactory factory = new ShelverWebApplicationFactory();
        HttpClient client = factory.CreateClient();

        HttpResponseMessage response = await client.GetAsync("/FloorPlans/Edit/999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // This submits the create form the way a browser does: it loads the form first so the
    // client holds the antiforgery cookie, then posts the matching token from the form.
    private static async Task<HttpResponseMessage> PostCreateAsync(HttpClient client, string name, string widthCm, string heightCm)
    {
        string formPage = await client.GetStringAsync("/FloorPlans/Create");
        string antiforgeryToken = Regex.Match(formPage, "name=\"__RequestVerificationToken\" type=\"hidden\" value=\"([^\"]+)\"").Groups[1].Value;

        Dictionary<string, string> fields = new Dictionary<string, string>
        {
            ["Name"] = name,
            ["WidthCm"] = widthCm,
            ["HeightCm"] = heightCm,
            ["__RequestVerificationToken"] = antiforgeryToken
        };
        return await client.PostAsync("/FloorPlans/Create", new FormUrlEncodedContent(fields));
    }
}
