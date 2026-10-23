/***********************************************************************************************
*
*   map_page_tests.cs
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

namespace shelver.tests.integration;

// These start the whole app on an in-memory server once for the class, so each test checks
// routing, dependency injection, and view rendering together.
public class map_page_tests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient client;

    public map_page_tests(WebApplicationFactory<Program> factory)
    {
        client = factory.CreateClient();
    }

    [Fact]
    public async Task root_returns_default_floor_plan()
    {
        HttpResponseMessage response = await client.GetAsync("/");
        string body = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("Warehouse", body);
    }

    [Fact]
    public async Task map_returns_requested_floor_plan()
    {
        HttpResponseMessage response = await client.GetAsync("/map/map/2");
        string body = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("Office", body);
    }

    [Fact]
    public async Task map_returns_not_found_for_unknown_id()
    {
        HttpResponseMessage response = await client.GetAsync("/map/map/999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
