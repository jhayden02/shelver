# History

shelver keeps two separate records so shelves can be sorted by when they last changed and by
when they were last checked.

## Change history

**Status: planned.**

Every change shelver makes to a shelf's contents is recorded: a variant moved, added, or
removed. Each entry stores what changed, on which shelf, when, and who made it.

- "Last adjusted" for a shelf comes from its most recent entry.
- Edits do not require an audit. Changes made while restocking are recorded the same way as
  changes made during an audit.
- shelver only knows about changes it makes. A change made directly in the store is noticed
  the next time shelver reads that variant, and is recorded as "changed outside shelver" with
  the time it was detected. Who made it and exactly when are unknown.

## Audits

**Status: planned.**

An audit is one user's check of one shelf.

- A user begins an audit on a shelf, checks it against the shelf view, and completes it.
- Completed audits are recorded even when nothing changed. A clean audit is the evidence that
  the shelf was checked and found correct.
- "Last audited" for a shelf comes from its most recent completed audit.
- A shelf can have only one open audit. Beginning an audit on a shelf that already has one is
  blocked, and shelver shows who has it open and since when.
- An open audit expires after the time limit set on the [Settings](pages/settings.md) page.
  Expiry is worked out when the audit is next shown or acted on, not by a background job.
  Expired audits stay in the history marked as expired.
- An Administrator can cancel another user's open audit, or take it over.

## Users in history

Each history entry and audit record stores both a link to the user's account and a copy of
the user's name at the time. If the account is later deleted, the link is cleared and the name
remains, so history is never lost or left pointing at a missing account.

## Open questions

- Whether changes made during an audit are linked to that audit.
- What a taken-over audit records: the original user, the new user, or both.
