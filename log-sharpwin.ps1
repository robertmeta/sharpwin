# Define the log file name with a unique identifier (timestamp)
$LogPath = Join-Path $env:TEMP -ChildPath ("sharpwin-{0}.log" -f (Get-Date -Format "yyyyMMddHHmmss"))

# Call sharpwin.ps1 (or .exe, adjust accordingly) and append output to log file
& "$PSScriptRoot\sharpwin.ps1" | Tee-Object -FilePath $LogPath -Append
