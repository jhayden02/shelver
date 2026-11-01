# Users

**Status: planned.**

Account management for Administrators. Roles and permissions are described in
[accounts.md](../accounts.md).

## Behavior

- Lists every account with its role, whether it is active, and its last activity from the
  [change history](../history.md).
- An Administrator can create an account, set its role to Auditor or Administrator, reset its
  PIN, and deactivate or reactivate it.
- Accounts cannot be deleted here, only deactivated, so their history stays linked to them. A
  deactivated user cannot sign in.
- The last active Administrator cannot be deactivated or demoted. To replace them, first make
  another Administrator, here or with `reset_admin.py` (see
  [Recovery](../accounts.md#recovery)).
