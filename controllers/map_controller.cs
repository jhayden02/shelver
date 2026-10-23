/***********************************************************************************************
*
*   map_controller.cs
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
using shelver.data;
using shelver.models;

namespace shelver.controllers;

public class map_controller : Controller
{
    private readonly floor_plan_store plan_store;

    public map_controller(floor_plan_store plan_store)
    {
        this.plan_store = plan_store;
    }

    public IActionResult map(int id = 1)
    {
        floor_plan? plan = plan_store.find_floor_plan(id);
        if (plan == null)
        {
            return NotFound();
        }
        return View(plan);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult error()
    {
        return View(new error_view_model { request_id = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
