/***********************************************************************************************
*
*   MapController.cs
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
using Shelver.Data;
using Shelver.Models;

namespace Shelver.Controllers;

public class MapController : Controller
{
    private readonly FloorPlanStore _planStore;

    public MapController(FloorPlanStore planStore)
    {
        _planStore = planStore;
    }

    public IActionResult Map(int id = 1)
    {
        FloorPlan? plan = _planStore.FindFloorPlan(id);
        if (plan == null)
        {
            return NotFound();
        }
        return View(plan);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
