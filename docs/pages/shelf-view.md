# Shelf view

**Status: planned.**

A full-screen view of one shelf for auditing and rearranging its variants. Available to
Auditors and Administrators. Designed for a tablet.

## Navigation

- Each shelf has its own address, `/Shelves/{id}`, where `id` is the number in its Shopify ID,
  so it can be bookmarked or linked, and the browser's back button works.
- A visible back button always returns to the shelf's floor on [Home](home.md), however the
  shelf was opened. It is disabled while editing.

## Layout

- The shelf is drawn as its levels, top to bottom, each listing its variants sorted by title,
  then SKU. A variant on several levels appears on each.
- Each variant shows its picture, SKU, and quantity.
- A variant on several shelves shows the store's total quantity and lists its other shelves.
  See [data.md](../data.md#quantities).
- An **unplaced** area holds variants the store lists on this shelf without a known level.
  Placing them is optional.

## Editing

**Edit** locks the shelf to the signed-in user. A user can edit one shelf at a time.

- Other users see the same view, with Edit disabled and "Being edited by <name>" above the
  shelf.
- **Done** ends the edit and releases the lock. The back button stays disabled until then.
- After 5 minutes without an action, the edit ends as if Done were pressed.
- Edit is disabled while the shelf's building is open in the
  [Buildings editor](buildings.md#locks).

While editing:

- **Move between levels:** drag a variant to another level, or to or from the unplaced area.
  Built with SortableJS; each drop is saved through a controller action.
- **Select several:** tap variants to select them, then act on all of them at once.
- **Remove:** removes this shelf from the variants' location fields. A variant on no other
  shelf then appears in the No location list on Home.
- **Add:** search every variant by title or SKU. Each result shows the shelves it is on, or
  that it is on none. Added variants arrive unplaced.

To move a variant to another shelf, remove it here, press Done, then add it from the other
shelf's view.

Every change is saved as it is made, recorded in the [change history](../history.md), and
written to the store following the [sync rules](../data.md#sync-rules). Edits do not require
an audit.

## Auditing

**Mark audited** records that the signed-in user confirmed this shelf matches the view. It is
available only while the shelf is not being edited. If something needs fixing, edit, press
Done, then mark it audited. See [history.md](../history.md#audits).
