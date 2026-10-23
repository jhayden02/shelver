/***********************************************************************************************
*
*   floor_plan_store.cs
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

using shelver.models;

namespace shelver.data;

// This holds floor plans in memory until the SQLite database replaces it. It is registered as
// a singleton so every request sees the same plans.
public class floor_plan_store
{
    private readonly List<floor_plan> floor_plans = create_sample_floor_plans();

    public floor_plan? find_floor_plan(int floor_plan_id)
    {
        return floor_plans.Find((floor_plan plan) => plan.id == floor_plan_id);
    }

    private static List<floor_plan> create_sample_floor_plans()
    {
        floor_plan warehouse = new floor_plan
        {
            id = 1,
            name = "Warehouse",
            width_cm = 2000,
            height_cm = 1200,
            walls = new List<wall>
            {
                new wall
                {
                    id = 1, start_x = 100, start_y = 100, end_x = 1900, end_y = 100,
                    doors = new List<door>
                    {
                        new door { id = 1, offset_cm = 400, swing_side = door_swing_side.RIGHT, hinge_end = door_hinge_end.START }
                    }
                },
                new wall
                {
                    id = 2, start_x = 1900, start_y = 100, end_x = 1900, end_y = 1100,
                    doors = new List<door>
                    {
                        new door { id = 2, offset_cm = 500, swing_side = door_swing_side.RIGHT, hinge_end = door_hinge_end.END }
                    }
                },
                new wall { id = 3, start_x = 1900, start_y = 1100, end_x = 100, end_y = 1100 },
                new wall
                {
                    id = 4, start_x = 100, start_y = 1100, end_x = 100, end_y = 100,
                    doors = new List<door>
                    {
                        new door { id = 3, offset_cm = 300, swing_side = door_swing_side.LEFT, hinge_end = door_hinge_end.START }
                    }
                },
                new wall
                {
                    id = 5, start_x = 1000, start_y = 100, end_x = 1000, end_y = 700,
                    doors = new List<door>
                    {
                        new door { id = 4, offset_cm = 400, swing_side = door_swing_side.LEFT, hinge_end = door_hinge_end.END }
                    }
                },
                new wall
                {
                    id = 6, start_x = 1300, start_y = 700, end_x = 1700, end_y = 1100,
                    doors = new List<door>
                    {
                        new door { id = 5, offset_cm = 280, swing_side = door_swing_side.RIGHT, hinge_end = door_hinge_end.START }
                    }
                }
            }
        };

        floor_plan office = new floor_plan
        {
            id = 2,
            name = "Office",
            width_cm = 1000,
            height_cm = 800,
            walls = new List<wall>
            {
                new wall { id = 7, start_x = 100, start_y = 100, end_x = 900, end_y = 100 },
                new wall { id = 8, start_x = 900, start_y = 100, end_x = 900, end_y = 700 },
                new wall
                {
                    id = 9, start_x = 900, start_y = 700, end_x = 100, end_y = 700,
                    doors = new List<door>
                    {
                        new door { id = 6, offset_cm = 400, swing_side = door_swing_side.RIGHT, hinge_end = door_hinge_end.END }
                    }
                },
                new wall { id = 10, start_x = 100, start_y = 700, end_x = 100, end_y = 100 }
            }
        };

        return new List<floor_plan> { warehouse, office };
    }
}
