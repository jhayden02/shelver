# Users

**Status: planned.**

Account management for Administrators. Roles and permissions are described in
[accounts.md](../accounts.md).

## Behavior

- Lists every account with its role, whether it is active, and its last activity from the
  [change history](../history.md).
- An Administrator can create an account, set its role to Auditor or Administrator, reset its
  password, and deactivate or reactivate it.
- Accounts are deactivated rather than deleted, so their history stays linked to them. A
  deactivated user cannot sign in.

## Open questions

- Whether the last active Administrator is prevented from being deactivated or demoted, or
  whether `tools/reset_admin.py` is the only safeguard.
- Whether accounts can be deleted from this page at all, or only deactivated.
