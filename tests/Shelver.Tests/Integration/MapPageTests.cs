/***********************************************************************************************
*
*   MapPageTests.cs
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

namespace Shelver.Tests.Integration;

// These start the whole app on an in-memory server once for the class, so each test checks
// routing, dependency injection, and view rendering together.
public class MapPageTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public MapPageTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetRoot_Always_ReturnsDefaultFloorPlan()
    {
        HttpResponseMessage response = await _client.GetAsync("/");
        string body = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("Warehouse", body);
    }

    [Fact]
    public async Task GetMap_KnownId_ReturnsRequestedFloorPlan()
    {
        HttpResponseMessage response = await _client.GetAsync("/Map/Map/2");
        string body = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("Office", body);
    }

    [Fact]
    public async Task GetMap_UnknownId_ReturnsNotFound()
    {
        HttpResponseMessage response = await _client.GetAsync("/Map/Map/999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
