#!/usr/bin/env bash
# Reset the demo database to its seeded state.
set -euo pipefail
here="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
rm -f "$here/../src/Orders.Api/orders.db" \
      "$here/../src/Orders.Api/orders.db-shm" \
      "$here/../src/Orders.Api/orders.db-wal"
echo "Database reset. It will be recreated and reseeded on the next run."
