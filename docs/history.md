# History

shelver keeps two separate records so shelves can be sorted by when they last changed and by
when they were last checked.

## Change history

**Status: planned.**

Every change shelver makes to a shelf's contents is recorded: a variant moved, added, or
removed. Each entry stores what changed, on which shelf, when, and who made it.

- "Last adjusted" for a shelf comes from its most recent entry.
- Edits do not require an audit.
- shelver only knows about changes it makes. A change made directly in the store is noticed
  the next time shelver reads that variant, and is recorded as "changed outside shelver" with
  the time it was detected. Who made it and exactly when are unknown.

## Audits

**Status: planned.**

An audit is one user's confirmation that a shelf matches what shelver shows at that moment.
It records the user and the time, and changes nothing.

- Anything wrong is fixed first, in person or with edits, which are recorded as usual.
- Quantities are the store's totals, so they can only be checked where the whole quantity is
  on this shelf.
- "Last audited" for a shelf comes from its most recent audit.

## Users in history

Each history entry and audit record stores both a link to the user's account and a copy of
the user's name at the time. If the account is later deleted, the link is cleared and the name
remains, so history is never lost or left pointing at a missing account.
