/***********************************************************************************************
*
*   LengthUnits.cs
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

// Lengths are stored as whole millimeters and shown in feet. A foot is exactly 304.8 mm, so
// decimal arithmetic converts without binary rounding error.
public static class LengthUnits
{
    public const decimal MillimetersPerFoot = 304.8m;

    public static int FeetToMillimeters(decimal feet)
    {
        return (int)Math.Round(feet * MillimetersPerFoot, MidpointRounding.AwayFromZero);
    }

    public static decimal MillimetersToFeet(int millimeters)
    {
        return millimeters / MillimetersPerFoot;
    }
}
