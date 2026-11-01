# Users

**Status: built, except last activity, which is planned.**

Account management for Administrators. Roles and permissions are described in
[accounts.md](../accounts.md).

## Behavior

- Lists every account with its role, whether it is active, and its last activity from the
  [change history](../history.md).
- An Administrator can create an account, rename it, set its role to Auditor or
  Administrator, reset its PIN, and deactivate or reactivate it.
- Resetting a PIN signs out the account's sessions and clears any lockout.
- Accounts cannot be deleted here, only deactivated, so their history stays linked to them. A
  deactivated user cannot sign in.
- The last active Administrator cannot be deactivated or demoted. To replace them, first make
  another Administrator, here or with `reset_admin.py` (see
  [Recovery](../accounts.md#recovery)).
