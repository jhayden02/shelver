# Shelf view

**Status: planned.**

A full-screen view of one shelf for auditing and rearranging its variants. Available to
Auditors and Administrators. Designed for a tablet.

## Navigation

- Each shelf has its own address, `/Shelves/{id}`, so it can be bookmarked or linked, and the
  browser's back button works.
- A visible back button always returns to the shelf's floor on [Home](home.md), however the
  shelf was opened.

## Layout

- The shelf is drawn as its levels, top to bottom, each listing its variants sorted by title,
  then SKU. A variant on several levels appears on each.
- Each variant shows its picture, SKU, and quantity.
- A variant on several shelves shows the store's total quantity and lists its other shelves.
  See [data.md](../data.md#quantities).
- An **unplaced** area holds variants the store lists on this shelf without a known level,
  such as those added in the store. Placing them is optional.

## Editing

All edits are recorded in the [change history](../history.md) and do not require an audit.

- **Move between levels:** drag a variant to another level, or to or from the unplaced area.
  Built with SortableJS; each drop is saved through a controller action.
- **Select several:** tap variants to select them, then act on all of them at once.
- **Move to another shelf:** the variants arrive unplaced on the chosen shelf.
- **Move to nowhere:** removes this shelf from the variants' location fields. A variant on no
  other shelf then appears in the No location list on Home.
- **Add:** search the store and add variants to this shelf, where they arrive unplaced.

Moves, adds, and removals update location fields in the store following the
[sync rules](../data.md#sync-rules).

## Auditing

- **Begin audit** starts an audit of this shelf for the signed-in user. It is unavailable
  while another user has an open audit here, and shows who and since when.
- The user checks that each variant is on the right level and that each quantity is correct,
  fixing anything wrong with the edits above.
- **Complete audit** records the audit, even when nothing changed.
- Administrators can cancel or take over another user's open audit.

Audit rules, including expiry, are in [history.md](../history.md#audits).

## Open questions

- How the destination shelf is chosen: a searchable list of names, a small floor plan to tap,
  or both.
- Whether an audit can record a discrepancy that was found but not fixed.
