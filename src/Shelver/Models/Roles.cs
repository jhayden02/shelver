/***********************************************************************************************
*
*   Roles.cs
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

// These are constants so the same names can be used to create the roles and in [Authorize]
// attributes, which only accept constant values.
public static class Roles
{
    public const string Auditor = "Auditor";
    public const string Administrator = "Administrator";

    public static readonly string[] All = { Auditor, Administrator };
}
