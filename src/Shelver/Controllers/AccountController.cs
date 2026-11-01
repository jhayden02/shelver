/***********************************************************************************************
*
*   AccountController.cs
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

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shelver.Models;
using IdentitySignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace Shelver.Controllers;

// The actions are not named SignIn and SignOut because Controller already has methods with
// those names for issuing authentication results.
public class AccountController : Controller
{
    private readonly UserManager<ShelverUser> _userManager;
    private readonly SignInManager<ShelverUser> _signInManager;

    public AccountController(UserManager<ShelverUser> userManager, SignInManager<ShelverUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpGet("Account/SignIn")]
    public async Task<IActionResult> ChooseAccount(string? returnUrl)
    {
        List<string> usernames = await _userManager.Users.Where((ShelverUser user) => user.IsActive).Select((ShelverUser user) => user.UserName!).OrderBy((string username) => username).ToListAsync();
        return View(new ChooseAccountViewModel { Usernames = usernames, ReturnUrl = returnUrl });
    }

    [HttpGet("Account/SignIn/{username}")]
    public async Task<IActionResult> EnterPin(string username, string? returnUrl)
    {
        ShelverUser? user = await FindActiveUserAsync(username);
        if (user == null)
        {
            return RedirectToAction(nameof(ChooseAccount), new { returnUrl });
        }
        return View(new EnterPinViewModel { Username = user.UserName!, ReturnUrl = returnUrl });
    }

    [HttpPost("Account/SignIn/{username}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EnterPin(string username, EnterPinViewModel form)
    {
        ShelverUser? user = await FindActiveUserAsync(username);
        if (user == null)
        {
            return RedirectToAction(nameof(ChooseAccount), new { returnUrl = form.ReturnUrl });
        }
        form.Username = user.UserName!;
        if (!ModelState.IsValid)
        {
            return View(form);
        }

        // The cookie is not persistent, so the browser drops it when it closes.
        IdentitySignInResult result = await _signInManager.PasswordSignInAsync(user, form.Pin, isPersistent: false, lockoutOnFailure: true);
        if (result.Succeeded)
        {
            return Url.IsLocalUrl(form.ReturnUrl) ? Redirect(form.ReturnUrl) : RedirectToAction("Index", "Home");
        }

        if (result.IsLockedOut)
        {
            DateTimeOffset? lockoutEnd = await _userManager.GetLockoutEndDateAsync(user);
            int minutesLeft = Math.Max(1, (int)Math.Ceiling(((lockoutEnd ?? DateTimeOffset.UtcNow) - DateTimeOffset.UtcNow).TotalMinutes));
            ModelState.AddModelError("", $"Too many wrong PINs. Try again in {minutesLeft} {(minutesLeft == 1 ? "minute" : "minutes")}.");
        }
        else
        {
            ModelState.AddModelError("", "Wrong PIN.");
        }
        form.Pin = "";
        return View(form);
    }

    [HttpPost("Account/SignOut")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EndSession()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction(nameof(ChooseAccount));
    }

    // A deactivated account is treated the same as a missing one, so neither can be signed in.
    private async Task<ShelverUser?> FindActiveUserAsync(string username)
    {
        ShelverUser? user = await _userManager.FindByNameAsync(username);
        return user != null && user.IsActive ? user : null;
    }
}
