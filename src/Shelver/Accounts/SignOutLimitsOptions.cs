/***********************************************************************************************
*
*   SignOutLimitsOptions.cs
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

namespace Shelver.Accounts;

// This is bound to the SignOutLimits section of appsettings.json. The defaults here apply when
// the section is missing.
public class SignOutLimitsOptions
{
    public const string SectionName = "SignOutLimits";

    public RoleSignOutLimits Auditor { get; set; } = new RoleSignOutLimits { IdleMinutes = 15, AbsoluteHours = 12 };
    public RoleSignOutLimits Administrator { get; set; } = new RoleSignOutLimits { IdleMinutes = 60, AbsoluteHours = 12 };

    public RoleSignOutLimits ForRoles(IEnumerable<string> roleNames)
    {
        return roleNames.Contains(Roles.Administrator) ? Administrator : Auditor;
    }

    public bool IsValid()
    {
        return Auditor.IsValid() && Administrator.IsValid();
    }
}

public class RoleSignOutLimits
{
    public int IdleMinutes { get; set; }
    public int AbsoluteHours { get; set; }

    public TimeSpan Idle => TimeSpan.FromMinutes(IdleMinutes);
    public TimeSpan Absolute => TimeSpan.FromHours(AbsoluteHours);

    public bool IsValid()
    {
        return IdleMinutes > 0 && AbsoluteHours > 0;
    }
}
