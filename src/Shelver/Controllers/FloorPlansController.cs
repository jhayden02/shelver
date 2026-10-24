/***********************************************************************************************
*
*   FloorPlansController.cs
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

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shelver.Data;
using Shelver.Models;

namespace Shelver.Controllers;

public class FloorPlansController : Controller
{
    private readonly ShelverDbContext _database;

    public FloorPlansController(ShelverDbContext database)
    {
        _database = database;
    }

    public async Task<IActionResult> Index()
    {
        List<FloorPlan> plans = await _database.FloorPlans.OrderBy((FloorPlan plan) => plan.Name).ToListAsync();
        return View(plans);
    }

    public IActionResult Create()
    {
        return View(new CreateFloorPlanViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateFloorPlanViewModel form)
    {
        if (!ModelState.IsValid)
        {
            return View(form);
        }

        // Validation guarantees the sizes are present, so the null-forgiving reads are safe.
        FloorPlan plan = new FloorPlan { Name = form.Name.Trim(), WidthMm = LengthUnits.FeetToMillimeters(form.WidthFeet!.Value), HeightMm = LengthUnits.FeetToMillimeters(form.HeightFeet!.Value) };
        _database.FloorPlans.Add(plan);
        await _database.SaveChangesAsync();

        return RedirectToAction(nameof(Edit), new { id = plan.Id });
    }

    public async Task<IActionResult> Edit(int id)
    {
        FloorPlan? plan = await _database.FloorPlans.FindAsync(id);
        if (plan == null)
        {
            return NotFound();
        }
        return View(plan);
    }
}
