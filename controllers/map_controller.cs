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
using shelver.models;

namespace shelver.controllers;

public class map_controller : Controller
{
    public IActionResult map()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult error()
    {
        error_view_model model = new error_view_model
        {
            request_id = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        };
        return View(model);
    }
}
