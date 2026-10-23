/***********************************************************************************************
*
*   wall_tests.cs
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

namespace shelver.tests.models;

public class wall_tests
{
    // These cases come from the sample walls, where y increases downward so a wall running
    // down the screen points at positive 90 degrees.
    [Theory]
    [InlineData(100, 100, 1900, 100, 0.0)]
    [InlineData(1900, 100, 1900, 1100, 90.0)]
    [InlineData(1900, 1100, 100, 1100, 180.0)]
    [InlineData(100, 1100, 100, 100, -90.0)]
    [InlineData(1300, 700, 1700, 1100, 45.0)]
    public void angle_degrees_points_from_start_to_end(int start_x, int start_y, int end_x, int end_y, double expected_angle_degrees)
    {
        wall test_wall = new wall { start_x = start_x, start_y = start_y, end_x = end_x, end_y = end_y };

        Assert.Equal(expected_angle_degrees, test_wall.angle_degrees, precision: 6);
    }
}
