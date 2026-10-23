/***********************************************************************************************
*
*   WallTests.cs
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

namespace Shelver.Tests.Models;

public class WallTests
{
    // These cases come from the sample walls, where y increases downward so a wall running
    // down the screen points at positive 90 degrees.
    [Theory]
    [InlineData(100, 100, 1900, 100, 0.0)]
    [InlineData(1900, 100, 1900, 1100, 90.0)]
    [InlineData(1900, 1100, 100, 1100, 180.0)]
    [InlineData(100, 1100, 100, 100, -90.0)]
    [InlineData(1300, 700, 1700, 1100, 45.0)]
    public void AngleDegrees_WallDirection_PointsFromStartToEnd(int startX, int startY, int endX, int endY, double expectedAngleDegrees)
    {
        Wall wall = new Wall { StartX = startX, StartY = startY, EndX = endX, EndY = endY };

        Assert.Equal(expectedAngleDegrees, wall.AngleDegrees, precision: 6);
    }
}
