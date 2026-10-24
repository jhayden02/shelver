/***********************************************************************************************
*
*   CreateFloorPlanViewModel.cs
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

using System.ComponentModel.DataAnnotations;

namespace Shelver.Models;

// The create form binds to this instead of FloorPlan so a posted form cannot set fields such
// as Id or State. Sizes are entered in decimal feet and converted to millimeters on save. They
// are nullable so a blank field reports "required" instead of silently becoming 0.
public class CreateFloorPlanViewModel
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = "";

    [Required]
    [Range(typeof(decimal), "3", "3000")]
    [Display(Name = "Width (ft)")]
    public decimal? WidthFeet { get; set; }

    [Required]
    [Range(typeof(decimal), "3", "3000")]
    [Display(Name = "Height (ft)")]
    public decimal? HeightFeet { get; set; }
}
