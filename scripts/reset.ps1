# Reset the demo to its starting state: seeded database, no agent output.
#
# The agents write a brief, a specification and a plan into docs/. Those are the DEMO'S
# OUTPUT, not part of main - if they are present when Session 1 starts, Demo 1 opens with
# the brief already written and there is nothing to show. They live on the reference
# branches instead, which is why they are not git-ignored here.
$ErrorActionPreference = "Stop"
$root = Join-Path $PSScriptRoot ".."

# --- database
$db = Join-Path $root "src\Orders.Api\orders.db"
foreach ($f in @($db, "$db-shm", "$db-wal")) {
    if (Test-Path $f) { Remove-Item $f -Force }
}
Write-Host "Database reset. It will be recreated and reseeded on the next run."

# --- agent output
$outputs = @(
    "docs\brief\cancel-order-brief.md",
    "docs\spec\cancel-order.md",
    "docs\plans\cancel-order.md"
)
$removed = 0
foreach ($rel in $outputs) {
    $f = Join-Path $root $rel
    if (Test-Path $f) { Remove-Item $f -Force; Write-Host "  removed $rel"; $removed++ }
}
if ($removed -eq 0) { Write-Host "No agent output to clear." }

# --- the check that matters
Push-Location $root
try {
    $branch = (git rev-parse --abbrev-ref HEAD).Trim()
    if ($branch -eq "main") {
        $tracked = git ls-files $outputs
        if ($tracked) {
            Write-Warning "These are TRACKED on main and should not be - they are demo output:"
            $tracked | ForEach-Object { Write-Warning "    $_" }
            Write-Warning "Remove them with: git rm --cached <path>   (the reference branches keep their own copies)"
        }
    }
    $dirty = git status --porcelain
    if ($dirty) {
        Write-Host ""
        Write-Host "Working tree is not clean:" -ForegroundColor Yellow
        $dirty | ForEach-Object { Write-Host "    $_" }
    } else {
        Write-Host "Working tree clean, on $branch." -ForegroundColor Green
    }
}
finally { Pop-Location }
