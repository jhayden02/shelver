/***********************************************************************************************
*
*   UserListItemViewModel.cs
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

// This is one row of the Users list and the header of the edit page.
public class UserListItemViewModel
{
    public string Id { get; set; } = "";
    public string Username { get; set; } = "";
    public string RoleName { get; set; } = "";
    public bool IsActive { get; set; }
    public bool IsLockedOut { get; set; }
}
