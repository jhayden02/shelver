/***********************************************************************************************
*
*   ChooseAccountViewModel.cs
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

// The return address is carried through both sign-in steps so the user ends up on the page
// that sent them to sign in.
public class ChooseAccountViewModel
{
    public List<string> Usernames { get; set; } = new List<string>();
    public string? ReturnUrl { get; set; }
}
