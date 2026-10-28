# Data

## Ownership

Each piece of data has exactly one owner. shelver never keeps its own authoritative copy of
data the store owns.

| Data | Owner |
|---|---|
| Products and variants: title, SKU, image, total quantity | Store |
| Which shelves each variant is on | Store |
| Buildings, floors, walls, doors, stairs, rooms | shelver |
| Shelves: ID, name, placement, level count | shelver, which copies names and IDs to the store |
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
- Create, rename, and delete the store's copy of a shelf.

Implementations:

- **Local:** products stored in shelver's own database. Used by automated tests and CI, and
  for development without a store connection.
- **Shopify:** uses the GraphQL Admin API.

## Shelves in Shopify

**Status: planned, pending a trial on the development store.**

- Each placed shelf has a **Shelf** metaobject entry holding its name and ID. shelver writes
  these entries; they are not edited in Shopify.
- The **location field** is a variant metafield listing references to the Shelf entries the
  variant is on. The Shopify admin shows it as shelf names, edited by picking shelves.
- Renaming a shelf changes only its entry. Variants follow the reference.
- A spare shelf has no entry. Placing it creates one; removing it from its room deletes it.
- If the trial fails, shelver uses [text names](#text-names) instead.

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

## Text names

**Status: fallback.** Used only if Shelf metaobjects are not adopted.

The location field is a variant metafield holding a list of single-line text values, each a
shelf's name, restricted to a choice list of approved names. Which metafield to use is part
of the store connection's configuration. Plain text stays readable without shelver.

- The store only holds names, so the name links a shelf to its variants.
- Renaming a shelf in shelver changes its name, then rewrites it in every variant that lists
  it. A rename made directly in the store is not recognized: the new name appears as a name
  that matches no shelf.
- Renaming happens in this order, because the store rejects names that are not approved:
  1. Add the new name to the approved list.
  2. Update every variant from the old name to the new one.
  3. Remove the old name.
  If step 2 fails partway, the shelf is shown as partly renamed and the rename can be retried.
- A batch of renames that swaps or chains names, such as A3 to A4 while A4 becomes A3, goes
  through temporary names so no two shelves ever share a name.
- Names that match no shelf, including new names added to the approved list in the store, are
  listed in the building editor to be placed.

## Quantities

shelver displays quantities and never changes them. The store tracks quantity per site, not
per shelf, so for a variant on several shelves shelver can only show the total. The shelf view
shows that total and lists the variant's other shelves.

## Units

**Status: built.**

All lengths are stored as whole millimeters and entered and shown in decimal feet. A foot is
exactly 304.8 mm.
