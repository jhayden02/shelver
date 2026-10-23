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
using Shelver.Models;

namespace Shelver.Tests.Integration;

// Each test starts the whole app on an in-memory server with its own empty database, so each
// one checks routing, data access, and view rendering together.
public class MapPageTests
{
    [Fact]
    public async Task GetMap_KnownId_ReturnsRequestedFloorPlan()
    {
        using ShelverWebApplicationFactory factory = new ShelverWebApplicationFactory();
        FloorPlan plan = new FloorPlan { Name = "Office", WidthCm = 1000, HeightCm = 800 };
        await factory.AddAsync(plan);
        HttpClient client = factory.CreateClient();

        HttpResponseMessage response = await client.GetAsync($"/Map/Map/{plan.Id}");
        string body = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("Office", body);
    }

    [Fact]
    public async Task GetMap_UnknownId_ReturnsNotFound()
    {
        using ShelverWebApplicationFactory factory = new ShelverWebApplicationFactory();
        HttpClient client = factory.CreateClient();

        HttpResponseMessage response = await client.GetAsync("/Map/Map/999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
