# Tools

Helper scripts for working on shelver. They are not part of the app and are never deployed.

## Running the scripts

The scripts are run with [uv](https://docs.astral.sh/uv/). To install it, follow the
instructions [here](https://docs.astral.sh/uv/getting-started/installation/).

### On any platform

```bash
uv run tools/generate_banner.py
```

This works on Linux, macOS, and Windows.

### On Linux and macOS

The scripts are executable and start with this line:

```bash
#!/usr/bin/env -S uv run --script
```

so they can also be run directly, and the system hands them to uv:

```bash
./tools/generate_banner.py
```

### Do not run them with python directly

`python3 tools/generate_banner.py` ignores the `# /// script` block. The script then runs on
whatever Python is on your `PATH`, without its version check and without its dependencies, so
it may fail or behave differently.

## Scripts

### generate_banner.py

Writes `res/img/banner.svg`, the README banner, from the site font. Run it after changing the
font to update the banner. The text is converted to vector paths because GitHub shows README
images in a sandbox that cannot load web fonts.

### reset_admin.py

Creates an Administrator, or resets an existing Administrator's PIN. Also used to create the
first account on a new install.

```bash
uv run tools/reset_admin.py <username>
```

- It asks for the new 6-digit PIN twice, without showing it.
- Resetting also reactivates the account, clears any lockout, and signs out its sessions.
- It refuses a username that belongs to an Auditor.
- It runs the app's `reset-admin` command with `dotnet run`, so Identity validates and hashes
  the PIN. The PIN is passed on standard input, never as an argument.

### reset_database.py

Deletes the local SQLite database, removing every floor plan and account. The app creates a
fresh, empty database the next time it starts, and `reset_admin.py` creates its first account.
It will warn you before doing so, as this is irreversible.

- Stop shelver first. SQLite keeps the database files open while it runs.
- It asks for confirmation. Pass `--yes` to skip the prompt.
- The database path is read from the `Shelver` connection string in
  `src/Shelver/appsettings.json`, the same setting the app uses.
