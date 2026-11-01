# Data

## Ownership

Each piece of data has exactly one owner. shelver never keeps its own authoritative copy of
data the store owns.

| Data | Owner |
|---|---|
| Products and variants: title, SKU, image, total quantity | Store |
| Shelves: ID, room, position, name | Store, written only by shelver |
| Which shelves each variant is on | Store, written only by shelver |
| Buildings, floors, walls, doors, stairs | shelver |
| Rooms | shelver |
| Shelf placement on the floor, width, depth, front, rotation, level count | shelver, keyed by the shelf's ID |
| Which levels a variant is on within each shelf | shelver |
| Change history and audit records | shelver |
| User accounts | shelver |

## Store interface

**Status: planned.**

shelver supports Shopify only. It talks to the store through one C# interface, so automated
tests and CI can use a local implementation instead of a live store.

The interface can:

- Read products and their variants: title, SKU, image, and total quantity.
- Read the list of shelves a variant is on, and replace the whole list.
- Create the Shelf and location field definitions.
- Create, update, and delete Shelf entries.

Implementations:

- **Local:** products stored in shelver's own database. Used by automated tests and CI, and
  for development without a store connection.
- **Shopify:** uses the GraphQL Admin API.

## Shelves in Shopify

**Status: planned.**

On connection, shelver creates two definitions in the store:

- **Shelf** metaobject, owned by shelver as an app type (`$app:shelf`) that staff can only
  read. Shelves are created, changed, and deleted only through shelver. One entry per shelf:

  | Field | Type | Required |
  |---|---|---|
  | Room | Single line text, the room's name | Yes |
  | Position | Integer | Yes |
  | Name | Single line text, Room followed by Position, for example `A5` | Yes |

- **Location field:** a variant metafield holding a list of references to the Shelf entries
  the variant is on. Owned by shelver in its app namespace, so staff can only read it.

Rules:

- A shelf's ID is its entry's Shopify ID.
- Creating a shelf creates its entry. Any change to its room or position updates only the
  entry; variants follow the reference. Deleting the shelf deletes the entry.
- How references behave in the admin, in exports, and on deletion is open in
  [#22](https://github.com/jhayden02/shelver/issues/22). It decides whether shelver removes a
  deleted entry from variants' location fields itself.

## Sync rules

**Status: planned.**

- Only shelver writes location fields. Before any write, shelver reads the variant's current
  location field, applies its change to that list, and writes the whole list back. Moving
  variants between shelves follows the same pattern.
- Writes are sent in batches of up to 25 variants, the most Shopify allows in one call.
- How edits by two users at once, or from an out-of-date view, are handled is open in
  [#32](https://github.com/jhayden02/shelver/issues/32).
- shelver refreshes products and quantities from the store when a floor or shelf is opened. It
  does not receive push notifications, because a locally hosted install has no public address
  for webhooks.
- Loading a save overwrites every location field from the save. See
  [Recovery](accounts.md#recovery).

## Quantities

shelver displays quantities and never changes them. The store tracks quantity per site, not
per shelf, so for a variant on several shelves shelver can only show the total. The shelf view
shows that total and lists the variant's other shelves.

## Units

**Status: built.**

All lengths are stored as whole millimeters and entered and shown in decimal feet. A foot is
exactly 304.8 mm.
