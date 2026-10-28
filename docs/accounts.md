# Accounts

**Status: planned.**

Everyone signs in. Accounts are managed by ASP.NET Core Identity and stored in shelver's
database.

## Roles

| | Auditor | Administrator |
|---|---|---|
| Home, product search, No location list | Yes | Yes |
| Shelf view: move, add, and remove variants | Yes | Yes |
| Begin and complete audits | Yes | Yes |
| Cancel or take over another user's audit | No | Yes |
| Buildings: create, delete, edit structure | No | Yes |
| Rooms and shelves: add, move, delete, change levels | No | Yes |
| Settings and Users | No | Yes |

Permissions are enforced on the server for each controller. Hiding a navigation link is a
convenience only; visiting a forbidden address directly is still denied.

## Deactivation

Accounts are deactivated rather than deleted in normal use, so their history stays linked to
them. See [history.md](history.md) for how history survives an account that is deleted.

## Administrator recovery

The app cannot help anyone who cannot sign in as an Administrator, for example when the only
Administrator forgets their password or is deactivated by mistake.

Recovery uses a command-line mode of the app, which runs on the server and exits without
starting the website. Only someone with access to the machine can run it. The tool scripts in
`tools/` wrap these commands; they never write to the database themselves, so Identity always
handles password hashing.

- `reset_admin.py`: creates an Administrator, or resets one's password. Other accounts are
  untouched. Also used to create the first Administrator.
- `reset_users.py`: deletes every account, then creates a new Administrator. History keeps the
  copied names.
- `reset_buildings.py`: deletes every building and everything in it, including its shelves,
  change history, and audits. Accounts are untouched.

Running `reset_buildings.py` and `reset_users.py` together gives a clean slate. Settings are
configuration rather than data, so neither script resets them. To start from a completely
blank database, for example after breaking a migration during development, delete the
database file while the app is stopped.

These scripts replace the current `reset_database.py`.

## Open questions

- What signs a user out: closing the browser, a fixed time limit, or neither.
