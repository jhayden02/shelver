# Accounts

**Status: planned.**

Everyone signs in. Accounts are managed by ASP.NET Core Identity and stored in shelver's
database.

## Roles

| | Auditor | Administrator |
|---|---|---|
| Home, product search, No location list | Yes | Yes |
| Shelf view: move, add, and remove variants | Yes | Yes |
| Mark shelves audited | Yes | Yes |
| Buildings: create, delete, edit structure | No | Yes |
| Rooms and shelves: add, move, delete, change levels | No | Yes |
| Users | No | Yes |

Permissions are enforced on the server for each controller. Hiding a navigation link is a
convenience only; visiting a forbidden address directly is still denied.

## Deactivation

Accounts cannot be deleted in the app, only deactivated, so their history stays linked to
them. Only `reset_all.py` deletes accounts, along with everything else. See
[history.md](history.md#users-in-history).

## Recovery

Some problems cannot be fixed from inside the app, for example when the only Administrator
forgets their password, or when everything has to be reset or restored.

Recovery uses a command-line mode of the app, which runs on the server and exits without
starting the website. Only someone with access to the machine can run it. The tool scripts in
`tools/` wrap these commands; they never write to the database themselves, so Identity always
handles password hashing.

- `reset_admin.py`: creates an Administrator, or resets one's password. Other accounts are
  untouched. Also used to create the first Administrator.
- `reset_buildings.py`:
  1. Deletes the Shelf and Location definitions from the store, which also deletes every
     Shelf entry and every variant's location value. This step does not open or migrate the
     database, so a broken migration cannot block it.
  2. Deletes every building and everything in it, including its shelves, change history, and
     audits. Accounts are untouched.
- `reset_all.py`: runs `reset_buildings.py`, then deletes every account and creates a new
  Administrator.
- `save_state.py`: writes one save file holding everything:
  - A copy of shelver's database: accounts, buildings, shelves, history, and audits.
  - Every Shelf entry: ID, room, position, and name.
  - Every variant's location references.
- `load_state.py`: restores a save, overwriting both shelver and the store:
  1. Runs `reset_buildings.py`.
  2. Restores the database copy. The app's normal startup migrations bring it up to date.
  3. Recreates the definitions and Shelf entries. Shopify gives each entry a new ID, so every
     shelf ID in shelver's data is rewritten from old to new.
  4. Writes every variant's location field. Variants deleted since the save are skipped and
     listed.

  Everything reverts to its state at the save, including accounts deactivated since. Every
  ID tag must then be replaced; see [Shelf identity](pages/buildings.md#shelf-identity).

Saving and loading require the app to be stopped. A save only loads into the store it came
from. The save file holds password hashes and store data, so it must be kept private. Shopify
credentials are not in it, because they are deployment configuration.

These scripts replace the current `reset_database.py`.
