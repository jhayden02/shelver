/***********************************************************************************************
*
*   wall.cs
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

public class wall
{
    public const int thickness_cm = 15;

    public int id { get; set; }
    public int start_x { get; set; }
    public int start_y { get; set; }
    public int end_x { get; set; }
    public int end_y { get; set; }
    public List<door> doors { get; set; } = new List<door>();

    // This is the direction from the start point to the end point, measured clockwise from
    // the positive x axis because y increases downward.
    public double angle_degrees => Math.Atan2(end_y - start_y, end_x - start_x) * 180.0 / Math.PI;
}
