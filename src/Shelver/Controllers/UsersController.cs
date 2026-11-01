/***********************************************************************************************
*
*   UsersController.cs
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

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shelver.Models;

namespace Shelver.Controllers;

// Each change on the edit page is its own small form. Every post redirects back to the edit
// page with a message, so refreshing the page never repeats a change.
[Authorize(Roles = Roles.Administrator)]
public class UsersController : Controller
{
    private const string StatusMessageKey = "StatusMessage";
    private const string ErrorMessageKey = "ErrorMessage";

    private readonly UserManager<ShelverUser> _userManager;

    public UsersController(UserManager<ShelverUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        List<ShelverUser> users = await _userManager.Users.OrderBy((ShelverUser user) => user.UserName).ToListAsync();
        List<UserListItemViewModel> rows = new List<UserListItemViewModel>();
        foreach (ShelverUser user in users)
        {
            rows.Add(await ToListItemAsync(user));
        }
        return View(rows);
    }

    public IActionResult Create()
    {
        return View(new CreateUserViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateUserViewModel form)
    {
        if (!Roles.All.Contains(form.RoleName))
        {
            ModelState.AddModelError(nameof(form.RoleName), "Choose Auditor or Administrator.");
        }
        if (!ModelState.IsValid)
        {
            return ViewWithoutPin(form);
        }

        ShelverUser user = new ShelverUser { UserName = form.Username.Trim() };
        IdentityResult createResult = await _userManager.CreateAsync(user, form.Pin);
        if (!createResult.Succeeded)
        {
            AddErrors(createResult);
            return ViewWithoutPin(form);
        }
        IdentityResult roleResult = await _userManager.AddToRoleAsync(user, form.RoleName);
        if (!roleResult.Succeeded)
        {
            AddErrors(roleResult);
            return ViewWithoutPin(form);
        }

        TempData[StatusMessageKey] = $"Created {user.UserName}.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(string id)
    {
        ShelverUser? user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }
        return View(await ToListItemAsync(user));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Rename(string id, string? username)
    {
        ShelverUser? user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }
        string oldUsername = user.UserName!;
        IdentityResult result = await _userManager.SetUserNameAsync(user, (username ?? "").Trim());
        return RedirectWithResult(id, result, $"Renamed {oldUsername} to {user.UserName}.");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeRole(string id, string? roleName)
    {
        ShelverUser? user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }
        if (roleName == null || !Roles.All.Contains(roleName))
        {
            return RedirectWithError(id, "Choose Auditor or Administrator.");
        }
        if (roleName != Roles.Administrator && await IsLastActiveAdministratorAsync(user))
        {
            return RedirectWithError(id, $"{user.UserName} is the last active Administrator, so it cannot be demoted. Make another Administrator first.");
        }

        // Every account has exactly one role, so the old one is removed before the new one is
        // added.
        IList<string> currentRoleNames = await _userManager.GetRolesAsync(user);
        IdentityResult removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoleNames);
        if (!removeResult.Succeeded)
        {
            return RedirectWithResult(id, removeResult, "");
        }
        IdentityResult addResult = await _userManager.AddToRoleAsync(user, roleName);
        return RedirectWithResult(id, addResult, $"{user.UserName} is now an {roleName}.");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPin(string id, string? pin)
    {
        ShelverUser? user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        // The PIN is checked before the old one is removed, so a bad PIN never leaves the
        // account without one.
        foreach (IPasswordValidator<ShelverUser> validator in _userManager.PasswordValidators)
        {
            IdentityResult validation = await validator.ValidateAsync(_userManager, user, pin);
            if (!validation.Succeeded)
            {
                return RedirectWithResult(id, validation, "");
            }
        }

        // Replacing the PIN also updates the security stamp, which signs out the account's
        // sessions. A reset also clears any lockout from wrong PINs.
        IdentityResult result = await _userManager.RemovePasswordAsync(user);
        if (result.Succeeded)
        {
            result = await _userManager.AddPasswordAsync(user, pin!);
        }
        if (result.Succeeded)
        {
            result = await _userManager.SetLockoutEndDateAsync(user, null);
        }
        if (result.Succeeded)
        {
            result = await _userManager.ResetAccessFailedCountAsync(user);
        }
        return RedirectWithResult(id, result, $"Reset the PIN of {user.UserName}.");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetActive(string id, bool isActive)
    {
        ShelverUser? user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }
        if (!isActive && await IsLastActiveAdministratorAsync(user))
        {
            return RedirectWithError(id, $"{user.UserName} is the last active Administrator, so it cannot be deactivated. Make another Administrator first.");
        }

        user.IsActive = isActive;
        IdentityResult result = await _userManager.UpdateAsync(user);
        return RedirectWithResult(id, result, isActive ? $"Reactivated {user.UserName}." : $"Deactivated {user.UserName}.");
    }

    // This is the one place the last-Administrator protection is decided, for both demoting
    // and deactivating.
    private async Task<bool> IsLastActiveAdministratorAsync(ShelverUser user)
    {
        if (!user.IsActive || !await _userManager.IsInRoleAsync(user, Roles.Administrator))
        {
            return false;
        }
        IList<ShelverUser> administrators = await _userManager.GetUsersInRoleAsync(Roles.Administrator);
        return administrators.Count((ShelverUser administrator) => administrator.IsActive) == 1;
    }

    private async Task<UserListItemViewModel> ToListItemAsync(ShelverUser user)
    {
        IList<string> roleNames = await _userManager.GetRolesAsync(user);
        return new UserListItemViewModel
        {
            Id = user.Id,
            Username = user.UserName!,
            RoleName = roleNames.FirstOrDefault() ?? Roles.Auditor,
            IsActive = user.IsActive,
            IsLockedOut = await _userManager.IsLockedOutAsync(user)
        };
    }

    private IActionResult RedirectWithResult(string id, IdentityResult result, string successMessage)
    {
        if (!result.Succeeded)
        {
            return RedirectWithError(id, string.Join(" ", result.Errors.Select((IdentityError error) => error.Description)));
        }
        TempData[StatusMessageKey] = successMessage;
        return RedirectToAction(nameof(Edit), new { id });
    }

    private IActionResult RedirectWithError(string id, string message)
    {
        TempData[ErrorMessageKey] = message;
        return RedirectToAction(nameof(Edit), new { id });
    }

    // The PIN field is a text field, which would otherwise be refilled with what was typed and
    // put the PIN in the page's HTML.
    private IActionResult ViewWithoutPin(CreateUserViewModel form)
    {
        ModelState.Remove(nameof(CreateUserViewModel.Pin));
        form.Pin = "";
        return View(form);
    }

    private void AddErrors(IdentityResult result)
    {
        foreach (IdentityError error in result.Errors)
        {
            ModelState.AddModelError("", error.Description);
        }
    }
}
