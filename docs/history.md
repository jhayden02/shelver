# History

shelver keeps two separate records so shelves can be sorted by when they last changed and by
when they were last checked.

## Change history

**Status: planned.**

Every change shelver makes to a shelf's contents is recorded: a variant moved, added, or
removed. Each entry stores what changed, on which shelf, when, and who made it.

- "Last adjusted" for a shelf comes from its most recent entry.
- Edits do not require an audit.

## Audits

**Status: planned.**

An audit is one user's confirmation that a shelf matches what shelver shows at that moment.
It records the user and the time, and changes nothing.

- Anything wrong is fixed first, in person or with edits, which are recorded as usual.
- Quantities are the store's totals, so they can only be checked where the whole quantity is
  on this shelf.
- "Last audited" for a shelf comes from its most recent audit.

## Users in history

Each history entry and audit record links to the user's account and shows its current name.
Accounts are only deleted along with all history, by [`reset_all.py`](accounts.md#recovery),
so the link is always valid.
