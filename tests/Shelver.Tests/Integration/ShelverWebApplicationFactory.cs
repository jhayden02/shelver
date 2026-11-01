/***********************************************************************************************
*
*   ShelverWebApplicationFactory.cs
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

using System.Net;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Shelver.Data;
using Shelver.Models;

namespace Shelver.Tests.Integration;

// This starts the app against its own temporary SQLite file, so the normal startup path,
// including migrations, runs without touching the real database or another test's data.
public class ShelverWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _databasePath = Path.Combine(Path.GetTempPath(), $"shelver_test_{Guid.NewGuid():N}.db");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("ConnectionStrings:Shelver", $"Data Source={_databasePath}");
    }

    // This adds test data through the app's own DbContext and returns once it is saved.
    public async Task AddAsync<TEntity>(TEntity entity) where TEntity : class
    {
        using IServiceScope scope = Services.CreateScope();
        ShelverDbContext database = scope.ServiceProvider.GetRequiredService<ShelverDbContext>();
        database.Add(entity);
        await database.SaveChangesAsync();
    }

    // This creates an account through Identity, so the PIN is validated and hashed exactly as
    // it is in the app.
    public async Task<ShelverUser> CreateUserAsync(string username, string pin, string roleName, bool isActive = true)
    {
        using IServiceScope scope = Services.CreateScope();
        UserManager<ShelverUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<ShelverUser>>();
        ShelverUser user = new ShelverUser { UserName = username, IsActive = isActive };
        IdentityResult createResult = await userManager.CreateAsync(user, pin);
        Assert.True(createResult.Succeeded, string.Join(" ", createResult.Errors.Select((IdentityError error) => error.Description)));
        IdentityResult roleResult = await userManager.AddToRoleAsync(user, roleName);
        Assert.True(roleResult.Succeeded, string.Join(" ", roleResult.Errors.Select((IdentityError error) => error.Description)));
        return user;
    }

    public const string SignedInPin = "246802";

    // This creates an account with the given role, named after the role in lowercase, and
    // returns a client already signed in to it.
    public async Task<HttpClient> CreateSignedInClientAsync(string roleName, WebApplicationFactoryClientOptions? options = null)
    {
        string username = roleName.ToLowerInvariant();
        await CreateUserAsync(username, SignedInPin, roleName);
        HttpClient client = CreateClient(options ?? new WebApplicationFactoryClientOptions());
        HttpResponseMessage response = await PostPinAsync(client, username, SignedInPin);
        Assert.True(response.StatusCode == HttpStatusCode.Redirect || response.IsSuccessStatusCode, $"Signing in as {username} failed with {response.StatusCode}.");
        return client;
    }

    // This signs in the way a browser does: it loads the PIN page first so the client holds the
    // antiforgery cookie, then posts the matching token from the form.
    public static async Task<HttpResponseMessage> PostPinAsync(HttpClient client, string username, string pin, string? returnUrl = null)
    {
        string pinPage = await client.GetStringAsync($"/Account/SignIn/{username}");
        Dictionary<string, string> fields = new Dictionary<string, string>
        {
            ["Pin"] = pin,
            ["ReturnUrl"] = returnUrl ?? "",
            ["__RequestVerificationToken"] = ReadAntiforgeryToken(pinPage)
        };
        return await client.PostAsync($"/Account/SignIn/{username}", new FormUrlEncodedContent(fields));
    }

    public static string ReadAntiforgeryToken(string page)
    {
        return Regex.Match(page, "name=\"__RequestVerificationToken\" type=\"hidden\" value=\"([^\"]+)\"").Groups[1].Value;
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        // SQLite keeps pooled connections open, which would lock the file on some platforms.
        SqliteConnection.ClearAllPools();
        File.Delete(_databasePath);
    }
}
