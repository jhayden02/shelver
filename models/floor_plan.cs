/***********************************************************************************************
*
*   floor_plan.cs
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

namespace shelver.models;

// All measurements are whole centimeters from the plan's top-left corner, with y increasing
// downward to match SVG.
public class floor_plan
{
    public int id { get; set; }
    public string name { get; set; } = "";
    public int width_cm { get; set; }
    public int height_cm { get; set; }
    public List<wall> walls { get; set; } = new List<wall>();
}
