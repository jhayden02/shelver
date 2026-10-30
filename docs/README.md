# shelver design

These documents describe what shelver does and why it is built the way it is. They are
written before each feature is built and updated in the same commit whenever its behavior
changes. Setup and deployment instructions are not covered here.

## Status markers

Most of shelver is still being designed, so every behavior is marked:

- **Built:** works in the current code and is covered by tests.
- **Planned:** decided, but not built yet.

Unsettled decisions are tracked as GitHub issues.

## Purpose

shelver is a visual frontend over a Shopify store's inventory. It shows where
shelves physically are in a building and which inventory sits on each shelf, so users can:

- Find which shelf holds an item of inventory.
- Audit a shelf by comparing it, level by level, against what the store says is on it.
- Move inventory between shelves, and rename or rearrange shelves, without editing dozens of
  items by hand in the store.

The store remains the system of record for inventory. shelver adds the spatial knowledge the
store does not have.

## Concepts

- **Building:** a named site such as "Main Office" or "Warehouse," created and saved as a
  whole, used only for organization. Its number of floors is set when it is created and never
  changes.
- **Floor:** one level of a building, numbered 1, 2, 3 upward. Each floor has its own size,
  structure, and shelves. All floors of a building share one coordinate system, so a point on
  one floor is directly above or below the same point on the others; a smaller floor, such as
  a mezzanine, is positioned by an offset within it.
- **Structure:** a floor's walls, doors, and stairs.
- **Stairs:** a stairway linking a lower and an upper floor, drawn as a rectangle of any length
  covering the same area on both. One side is marked **bottom**, where it is entered from the
  lower floor, and the opposite side **top**, where it is entered from the upper floor.
- **Room:** a named rectangle on a floor that groups shelves and has one entrance door. Its
  name, such as `A`, is unique across all buildings. Shelf numbering runs from the entrance.
- **Shelf:** one physical shelving unit, with a permanent ID and a number of levels. While
  placed in a room it has a position and a name assigned from its room and number, for
  example `A1`. A shelf in no room is **spare**.
- **Level:** one tier of a shelf, such as a shelf board, drawer, or top surface. A variant's
  place within a shelf is one or more levels; arrangement within a level is not tracked.
- **Product:** an item in the store, with a title and one or more variants.
- **Variant:** one version of a product, such as a size or color, with its own SKU, quantity,
  and image (the product's image when it has none). Variants are what sit on shelves, and
  variants of the same product can be on different shelves. A product without options still
  has one variant.
- **Location field:** the variant metafield listing references to the shelves a variant is
  on. See [data.md](data.md#shelves-in-shopify).
- **Location name:** a shelf's name as it appears in the store, for example `A1`. This is not
  Shopify's own "Location," which means a whole site.
- **Unplaced:** a variant the store lists on a shelf without a known level.
- **No location:** a variant the store lists on no shelf at all.
- **Audit:** a recorded check of one shelf by one user, from beginning to completion.

## Documents

- [data.md](data.md): what the store owns, what shelver owns, and how they stay in sync.
- [history.md](history.md): change history and audit records.
- [accounts.md](accounts.md): roles, permissions, and administrator recovery.
- [architecture.md](architecture.md): technical constraints and stack.
- Pages:
  - [Buildings](pages/buildings.md)
  - [Home](pages/home.md)
  - [Shelf view](pages/shelf-view.md)
  - [Settings](pages/settings.md)
  - [Users](pages/users.md)
