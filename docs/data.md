# Data

## Ownership

Each piece of data has exactly one owner. shelver never keeps its own authoritative copy of
data the store owns.

| Data | Owner |
|---|---|
| Products and variants: title, SKU, image, total quantity | Store |
| Which shelves each variant is on | Store |
| Buildings, floors, walls, doors, stairs | shelver |
| Rooms | shelver, which copies room names to the store |
| Shelves: ID, name, placement, level count | shelver, which copies each placed shelf's room, position, and name to the store |
| Which levels a variant is on within each shelf | shelver |
| Change history and audit records | shelver |
| Settings and user accounts | shelver |

## Store interface

**Status: planned.**

shelver supports Shopify only. It talks to the store through one C# interface, so automated
tests and CI can use a local implementation instead of a live store.

The interface can:

- Read products and their variants: title, SKU, image, and total quantity.
- Read the list of shelves a variant is on, and replace the whole list.
- Create the Shelf and location field definitions, and keep the Room choices in sync.
- Create, update, and delete Shelf entries.

Implementations:

- **Local:** products stored in shelver's own database. Used by automated tests and CI, and
  for development without a store connection.
- **Shopify:** uses the GraphQL Admin API.

## Shelves in Shopify

**Status: planned.**

On connection, shelver creates two definitions in the store:

- **Shelf** metaobject, one entry per placed shelf:

  | Field | Type | Required |
  |---|---|---|
  | Room | Single line text, choice list of shelver's room names | Yes |
  | Position | Integer | Yes |
  | Name | Single line text, Room followed by Position, for example `A5` | Yes |

- **Location field:** a variant metafield holding a list of references to the Shelf entries
  the variant is on.

Rules:

- The Room choices follow shelver's rooms. Adding, renaming, or deleting a room updates them.
  Whether the API allows this, and how many choices it allows, is open in
  [#5](https://github.com/jhayden02/shelver/issues/5) and
  [#6](https://github.com/jhayden02/shelver/issues/6). If not, Room becomes plain text checked
  by shelver.
- shelver keeps each placed shelf's entry ID. The entry does not hold shelver's shelf ID.
- Placing a shelf creates its entry. Any change to its room or position updates only the
  entry; variants follow the reference. Removing it from its room deletes the entry and clears
  the ID.
- Whether staff can edit entries in the Shopify admin is open in
  [#21](https://github.com/jhayden02/shelver/issues/21). How references behave in the admin,
  in exports, and on deletion is open in [#22](https://github.com/jhayden02/shelver/issues/22).

## Sync rules

**Status: planned.**

- shelver should be used to change the location field, though it stays editable in Shopify.
- Before any write, shelver reads the variant's current location field, applies its change to
  that list, and writes the whole list back. This keeps changes made in the store since
  shelver last looked.
- If Shopify supports it, the write only succeeds if the value is unchanged since the read.
  Otherwise shelver reports the conflict instead of overwriting.
- shelver refreshes from the store when a floor or shelf is opened. It does not receive
  push notifications, because a locally hosted install has no public address for webhooks.
- Moving variants between shelves follows the same read, change, and write pattern.

## Quantities

shelver displays quantities and never changes them. The store tracks quantity per site, not
per shelf, so for a variant on several shelves shelver can only show the total. The shelf view
shows that total and lists the variant's other shelves.

## Units

**Status: built.**

All lengths are stored as whole millimeters and entered and shown in decimal feet. A foot is
exactly 304.8 mm.
