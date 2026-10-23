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

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Shelver.Data;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// A relative database path is resolved against the project folder rather than the working
// directory, so the same database is used however the app is started.
SqliteConnectionStringBuilder connectionStringBuilder = new SqliteConnectionStringBuilder(builder.Configuration.GetConnectionString("Shelver"));
connectionStringBuilder.DataSource = Path.Combine(builder.Environment.ContentRootPath, connectionStringBuilder.DataSource);
Directory.CreateDirectory(Path.GetDirectoryName(connectionStringBuilder.DataSource)!);
builder.Services.AddDbContext<ShelverDbContext>((DbContextOptionsBuilder options) => options.UseSqlite(connectionStringBuilder.ConnectionString));

WebApplication app = builder.Build();

// Pending migrations are applied at startup because a locally hosted install has no
// separate deployment step to run them.
using (IServiceScope scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<ShelverDbContext>().Database.Migrate();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Map/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Map}/{action=Map}/{id?}")
    .WithStaticAssets();


app.Run();

// This makes the compiler-generated Program class public so the test project's
// WebApplicationFactory can start the app.
public partial class Program { }
