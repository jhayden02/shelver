/***********************************************************************************************
*
*   FloorPlanStore.cs
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

using Shelver.Models;

namespace Shelver.Data;

// This holds floor plans in memory until the SQLite database replaces it. It is registered as
// a singleton so every request sees the same plans.
public class FloorPlanStore
{
    private readonly List<FloorPlan> _floorPlans = CreateSampleFloorPlans();

    public FloorPlan? FindFloorPlan(int floorPlanId)
    {
        return _floorPlans.Find((FloorPlan plan) => plan.Id == floorPlanId);
    }

    private static List<FloorPlan> CreateSampleFloorPlans()
    {
        FloorPlan warehouse = new FloorPlan
        {
            Id = 1,
            Name = "Warehouse",
            WidthCm = 2000,
            HeightCm = 1200,
            Walls = new List<Wall>
            {
                new Wall
                {
                    Id = 1, StartX = 100, StartY = 100, EndX = 1900, EndY = 100,
                    Doors = new List<Door>
                    {
                        new Door { Id = 1, OffsetCm = 400, SwingSide = DoorSwingSide.Right, HingeEnd = DoorHingeEnd.Start }
                    }
                },
                new Wall
                {
                    Id = 2, StartX = 1900, StartY = 100, EndX = 1900, EndY = 1100,
                    Doors = new List<Door>
                    {
                        new Door { Id = 2, OffsetCm = 500, SwingSide = DoorSwingSide.Right, HingeEnd = DoorHingeEnd.End }
                    }
                },
                new Wall { Id = 3, StartX = 1900, StartY = 1100, EndX = 100, EndY = 1100 },
                new Wall
                {
                    Id = 4, StartX = 100, StartY = 1100, EndX = 100, EndY = 100,
                    Doors = new List<Door>
                    {
                        new Door { Id = 3, OffsetCm = 300, SwingSide = DoorSwingSide.Left, HingeEnd = DoorHingeEnd.Start }
                    }
                },
                new Wall
                {
                    Id = 5, StartX = 1000, StartY = 100, EndX = 1000, EndY = 700,
                    Doors = new List<Door>
                    {
                        new Door { Id = 4, OffsetCm = 400, SwingSide = DoorSwingSide.Left, HingeEnd = DoorHingeEnd.End }
                    }
                },
                new Wall
                {
                    Id = 6, StartX = 1300, StartY = 700, EndX = 1700, EndY = 1100,
                    Doors = new List<Door>
                    {
                        new Door { Id = 5, OffsetCm = 280, SwingSide = DoorSwingSide.Right, HingeEnd = DoorHingeEnd.Start }
                    }
                }
            }
        };

        FloorPlan office = new FloorPlan
        {
            Id = 2,
            Name = "Office",
            WidthCm = 1000,
            HeightCm = 800,
            Walls = new List<Wall>
            {
                new Wall { Id = 7, StartX = 100, StartY = 100, EndX = 900, EndY = 100 },
                new Wall { Id = 8, StartX = 900, StartY = 100, EndX = 900, EndY = 700 },
                new Wall
                {
                    Id = 9, StartX = 900, StartY = 700, EndX = 100, EndY = 700,
                    Doors = new List<Door>
                    {
                        new Door { Id = 6, OffsetCm = 400, SwingSide = DoorSwingSide.Right, HingeEnd = DoorHingeEnd.End }
                    }
                },
                new Wall { Id = 10, StartX = 100, StartY = 700, EndX = 100, EndY = 100 }
            }
        };

        return new List<FloorPlan> { warehouse, office };
    }
}
