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

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Shelver.Data;

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

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        // SQLite keeps pooled connections open, which would lock the file on some platforms.
        SqliteConnection.ClearAllPools();
        File.Delete(_databasePath);
    }
}
