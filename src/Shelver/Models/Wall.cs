/***********************************************************************************************
*
*   Wall.cs
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

namespace Shelver.Models;

public class Wall
{
    public const int ThicknessMm = 150;

    public int Id { get; set; }
    public int FloorPlanId { get; set; }
    public int StartX { get; set; }
    public int StartY { get; set; }
    public int EndX { get; set; }
    public int EndY { get; set; }
    public List<Door> Doors { get; set; } = new List<Door>();

    // This is the direction from the start point to the end point, measured clockwise from
    // the positive x axis because y increases downward.
    public double AngleDegrees => Math.Atan2(EndY - StartY, EndX - StartX) * 180.0 / Math.PI;
}
