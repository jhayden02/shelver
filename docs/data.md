# Data

## Ownership

Each piece of data has exactly one owner. shelver never keeps its own authoritative copy of
data the store owns.

| Data | Owner |
|---|---|
| Products and variants: title, SKU, image, total quantity | Store |
| Which shelves each variant is on (location names) | Store |
| Buildings, floors, walls, doors, stairs | shelver |
| Shelves: ID, name, position, level count | shelver |
| A variant's level and order within a shelf | shelver |
| Change history and audit records | shelver |
| Settings and user accounts | shelver |

## Store interface

**Status: planned.**

shelver talks to the store only through one C# interface. This interface is shaped around
what most e-commerce platforms offer, not specifically Shopify.

E-commerce platforms have no built-in record of which shelf a variant is on. Instead, the
store owner designates one custom variant field as the **location field**: a list of text
values, each a shelf's location name. Plain text works on every platform and stays readable
without shelver.

The interface can:

- Read products and their variants: title, SKU, image, and total quantity.
- Read a variant's location field, and replace its whole list of values.
- Optional capabilities, used when the platform has them:
  - Restricting the location field to a list of approved values (in Shopify, a choice-list
    metafield).
  - Writing the location field only if its value has not changed since it was read.

Implementations:

- **Local:** products stored in shelver's own database. Used by automated tests and CI, which
  cannot depend on a live store, and for development without a store connection.
- **Shopify:** the location field is a variant metafield holding a list of single-line text
  values, restricted to a choice list of approved shelf names. Which metafield to use is part
  of the store connection's configuration. Shopify-specific details (metafields, choice lists,
  the GraphQL Admin API) live only in this implementation.

## Sync rules

**Status: planned.**

- shelver should be used to interact with the location field, though it stays editable in the
  Shopify store.
- Before any write, shelver reads the variant's current location field, applies its change to
  that list, and writes the whole list back. This keeps changes made in the store since
  shelver last looked.
- When the store supports it, the write only succeeds if the value is unchanged since the
  read. Otherwise shelver reports the conflict instead of overwriting.
- shelver refreshes from the store when a floor or shelf is opened. It does not receive
  push notifications, because a locally hosted install has no public address for webhooks.
- Each shelf has a permanent ID used inside shelver, and a name. The store only holds the
  name, in variants' location fields, so the name links a shelf to its variants.
- Renaming a shelf in shelver changes its name, then rewrites it in every variant that lists
  it. A rename made directly in the store is not recognized: the new name appears as a new,
  unplaced shelf.
- Renaming a shelf in a store with an approved-names list happens in this order, because the
  store rejects names that are not approved:
  1. Add the new name to the approved list.
  2. Update every variant from the old name to the new one.
  3. Remove the old name.
  If step 2 fails partway, the shelf is shown as partly renamed and the rename can be retried.
- A batch of renames that swaps or chains names, such as A3 to A4 while A4 becomes A3, goes
  through temporary names so no two shelves ever share a name.
- Moving variants between shelves follows the same read, change, and write pattern.
- New names added to the approved list in the store appear in shelver as shelves that are not
  placed on any floor yet.

## Quantities

shelver displays quantities and never changes them. The store tracks quantity per site, not
per shelf, so for a variant on several shelves shelver can only show the total. The shelf view
shows that total and lists the variant's other shelves.

## Units

**Status: built.**

All lengths are stored as whole millimeters and entered and shown in decimal feet. A foot is
exactly 304.8 mm.

## Open questions

- Whether the Shopify Admin API supports "write only if unchanged" for metafields.
- Whether the API allows updating a metafield definition's approved choices.
- Whether Shopify caps the number of choices. If the cap is below a store's shelf count, drop
  the choice list and have shelver check names itself.
