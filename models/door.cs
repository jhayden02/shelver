/***********************************************************************************************
*
*   door.cs
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

public class door
{
    public const int width_cm = 90;

    public int id { get; set; }

    // This is the distance along the wall from its start point to the door's center.
    public int offset_cm { get; set; }

    public door_swing_side swing_side { get; set; }
    public door_hinge_end hinge_end { get; set; }
}
