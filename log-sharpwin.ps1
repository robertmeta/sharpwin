# Define log file path with a timestamp for uniqueness
$timestamp = Get-Date -Format "yyyyMMdd-HHmmss"
$LOG = "C:\temp\sharpwin-$timestamp.log"
# Ensure the temp directory exists
$null = New-Item -ItemType Directory -Path "C:\temp" -Force
# Execute sharpwin script and append output to log file
# Assuming sharpwin is a PowerShell script (.ps1) in the same directory as this script
$scriptDir = Split-Path -Path $MyInvocation.MyCommand.Definition -Parent
& "$scriptDir\sharpwin.exe" | Tee-Object -FilePath $LOG -Append
