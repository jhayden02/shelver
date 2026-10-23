/***********************************************************************************************
*
*   HomeController.cs
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

using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shelver.Data;
using Shelver.Models;

namespace Shelver.Controllers;

public class HomeController : Controller
{
    private readonly ShelverDbContext _database;

    public HomeController(ShelverDbContext database)
    {
        _database = database;
    }

    public async Task<IActionResult> Index()
    {
        int readyFloorPlanCount = await _database.FloorPlans.CountAsync((FloorPlan plan) => plan.State == FloorPlanState.Done);
        return View(readyFloorPlanCount);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
