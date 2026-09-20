# Reset the demo database to its seeded state.
$ErrorActionPreference = "Stop"
$db = Join-Path $PSScriptRoot "..\src\Orders.Api\orders.db"
foreach ($f in @($db, "$db-shm", "$db-wal")) {
    if (Test-Path $f) { Remove-Item $f -Force }
}
Write-Host "Database reset. It will be recreated and reseeded on the next run."
