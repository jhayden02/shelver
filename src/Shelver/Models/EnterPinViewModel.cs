/***********************************************************************************************
*
*   EnterPinViewModel.cs
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

using System.ComponentModel.DataAnnotations;

namespace Shelver.Models;

// The username comes from the address, not the form, so it is set by the controller.
public class EnterPinViewModel
{
    public string Username { get; set; } = "";
    public string? ReturnUrl { get; set; }

    [Required]
    [RegularExpression("^[0-9]{6}$", ErrorMessage = "The PIN is 6 digits.")]
    [Display(Name = "PIN")]
    public string Pin { get; set; } = "";
}
