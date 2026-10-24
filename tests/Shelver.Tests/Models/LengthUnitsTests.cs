/***********************************************************************************************
*
*   LengthUnitsTests.cs
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

using System.Globalization;
using Shelver.Models;

namespace Shelver.Tests.Models;

public class LengthUnitsTests
{
    // Decimal values cannot be attribute arguments, so the feet are passed as strings.
    [Theory]
    [InlineData("1", 305)]
    [InlineData("10", 3048)]
    [InlineData("65.5", 19964)]
    [InlineData("0.5", 152)]
    public void FeetToMillimeters_DecimalFeet_RoundsToNearestMillimeter(string feet, int expectedMillimeters)
    {
        int millimeters = LengthUnits.FeetToMillimeters(decimal.Parse(feet, CultureInfo.InvariantCulture));

        Assert.Equal(expectedMillimeters, millimeters);
    }

    [Fact]
    public void MillimetersToFeet_WholeFeet_ReturnsExactFeet()
    {
        decimal feet = LengthUnits.MillimetersToFeet(3048);

        Assert.Equal(10m, feet);
    }
}
