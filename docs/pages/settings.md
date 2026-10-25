# Settings

**Status: planned.**

App-wide settings an Administrator can change while shelver is running. Administrators only.
Settings are stored in the database because `appsettings.json` is only read at startup.

## Settings

- **Audit expiry:** how long an audit may stay open before it expires. See
  [history.md](../history.md#audits).

The store connection (Shopify credentials) is not configured here; it is part of deployment
configuration.

## Open questions

- The default audit expiry.
