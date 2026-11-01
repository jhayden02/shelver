/***********************************************************************************************
*
*   Program.cs
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
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Shelver.Data;
using Shelver.Models;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// A relative database path is resolved against the project folder rather than the working
// directory, so the same database is used however the app is started.
SqliteConnectionStringBuilder connectionStringBuilder = new SqliteConnectionStringBuilder(builder.Configuration.GetConnectionString("Shelver"));
connectionStringBuilder.DataSource = Path.Combine(builder.Environment.ContentRootPath, connectionStringBuilder.DataSource);
Directory.CreateDirectory(Path.GetDirectoryName(connectionStringBuilder.DataSource)!);
builder.Services.AddDbContext<ShelverDbContext>((DbContextOptionsBuilder options) => options.UseSqlite(connectionStringBuilder.ConnectionString));

builder.Services.AddIdentityCore<ShelverUser>().AddRoles<IdentityRole>().AddEntityFrameworkStores<ShelverDbContext>();

WebApplication app = builder.Build();

// Pending migrations are applied at startup because a locally hosted install has no
// separate deployment step to run them. The roles are created afterwards if they are missing.
using (IServiceScope scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<ShelverDbContext>().Database.Migrate();

    RoleManager<IdentityRole> roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    foreach (string roleName in Roles.All)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            IdentityResult result = await roleManager.CreateAsync(new IdentityRole(roleName));
            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"The {roleName} role could not be created: {string.Join(" ", result.Errors.Select((IdentityError error) => error.Description))}");
            }
        }
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();

// This makes the compiler-generated Program class public so the test project's
// WebApplicationFactory can start the app.
public partial class Program { }
