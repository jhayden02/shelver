# Architecture

## Constraints

- **ASP.NET Core MVC.** shelver is built for an MVC class. Every page is served by an MVC
  controller and view; interactive behavior is added on top with JavaScript libraries.
- **Purpose-built JavaScript libraries over hand-written JavaScript.** Custom JavaScript is
  limited to thin glue between a library and the controllers.
- **Shopify only.** shelver connects to one Shopify store. See [data.md](data.md).
- **Locally hosted.** shelver runs on a machine on the same local network as the devices that
  use it, and is not reachable from the internet.
- **Standard naming for each language:** PascalCase C#, kebab-case CSS and web assets,
  snake_case Python.

## Stack

| Concern | Choice | Status |
|---|---|---|
| Web framework | ASP.NET Core MVC on .NET 10 | Built |
| Database | SQLite through EF Core; migrations applied at startup | Built |
| Accounts | ASP.NET Core Identity with roles | Planned |
| Partial page updates | htmx: controller actions return partial views | Planned |
| Moving variants between levels | SortableJS with its MultiDrag plugin | Planned |
| Building editor | interact.js on the server-rendered SVG | Planned |
| Tests | xUnit with in-memory integration tests through `WebApplicationFactory` | Built |
| Tool scripts | Python run through uv | Built |

## Devices

- The building editor is designed for a desktop with a mouse.
- Home and the shelf view are designed for a tablet: touch input, large tap targets, and no
  behavior that depends on hovering.

## Data storage

- The database file defaults to `src/Shelver/App_Data/shelver.db`, set by the `Shelver`
  connection string in `appsettings.json`. A relative path is resolved against the project
  folder.
- Lengths are stored in whole millimeters. See [data.md](data.md#units).
- Settings an Administrator can change at runtime live in the database, not in
  `appsettings.json`, which is only read at startup.
