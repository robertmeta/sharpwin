# Get the directory of the current script file
$scriptDir = $PSScriptRoot

# Search for Emacs in the PATH
$emacsInPath = Get-Command emacs -ErrorAction SilentlyContinue

if ($emacsInPath) {
    $emacs = $emacsInPath.Source
    Write-Host "Emacs found in PATH: $emacs"
}
else {
    # Search for Emacs in the EMACS environment variable
    $emacsEnvVar = $env:EMACS
    if ($emacsEnvVar -and (Test-Path $emacsEnvVar)) {
        $emacsExecutable = Join-Path $emacsEnvVar "emacs.exe"
        if (Test-Path $emacsExecutable) {
            $emacs = $emacsExecutable
            Write-Host "Emacs found in EMACS environment variable: $emacs"
        }
    }
}

# Check if Emacs was found
if (-not $emacs) {
    Write-Error "Emacs not found in PATH or EMACS environment variable."
    exit 1
}

# Check the EMACSPEAK_DIR environment variable
$emacspeak_dir = $env:EMACSPEAK_DIR
if ($emacspeak_dir -and (Test-Path $emacspeak_dir)) {
    Write-Host "Emacspeak directory found in EMACSPEAK_DIR environment variable: $emacspeak_dir"
}
else {
    Write-Error "Emacspeak directory not found in the project or EMACSPEAK_DIR environment variable."
    exit 1
}

# You can now use the $emacs and $emacspeak_dir variables for further processing

# Check if dotnet CLI is found in the PATH
$dotnetInPath = Get-Command dotnet -ErrorAction SilentlyContinue

if ($dotnetInPath) {
    $dotnet = $dotnetInPath.Source
    Write-Host "dotnet CLI found in PATH: $dotnet"
}
else {
    Write-Error "dotnet CLI not found in PATH."
    exit 1
}

# You can now use the $dotnet variable for further processing if needed

# Path to the solution file
$solutionPath = Join-Path $scriptDir "SharpWin.sln"

# Check if the solution file exists
if (-not (Test-Path $solutionPath)) {
    Write-Error "Solution file SharpWin.sln not found in the script directory."
    exit 1
}

# Build the solution in debug configuration
try {
    # Running dotnet build command
    & $dotnet build $solutionPath 
    Write-Host "Debug build completed successfully."
}
catch {
    Write-Error "An error occurred during the build process."
    exit 1
}

# Assuming the build output is located in the bin\Debug directory of your project
$buildOutputDir = Join-Path $scriptDir "bin\Debug\net8.0"

# The destination directory is $emacspeak_dir/servers
$destinationDir = Join-Path $emacspeak_dir "servers"

# Check if the build output directory exists
if (-not (Test-Path $buildOutputDir)) {
    Write-Error "Build output directory does not exist: $buildOutputDir"
    exit 1
}

# Check if the destination directory exists, create it if it does not
if (-not (Test-Path $destinationDir)) {
    $null = New-Item -ItemType Directory -Path $destinationDir
    Write-Host "Created destination directory: $destinationDir"
}

# Copy all files from the build output directory to the destination directory recursively
try {
    Copy-Item -Path "$buildOutputDir\*" -Destination $destinationDir -Recurse -Force
    Write-Host "All files copied successfully to $destinationDir"
}
catch {
    Write-Error "An error occurred while copying the files."
    exit 1
}

# Path to the original sharpwin.exe in the build output directory
$originalExePath = Join-Path $scriptDir "bin\Debug\net8.0\sharpwin.exe"

# Destination directory, assuming $emacspeak_dir is already defined
$destinationDir = Join-Path $emacspeak_dir "servers"

# Path for the copied executable renamed to log-sharpwin.exe
$destinationExePath = Join-Path $destinationDir "log-sharpwin.exe"

# Check if the original sharpwin.exe exists
if (Test-Path $originalExePath) {
    # Copy and rename sharpwin.exe to log-sharpwin.exe
    try {
        Copy-Item -Path $originalExePath -Destination $destinationExePath -Force
        Write-Host "sharpwin.exe copied and renamed to log-sharpwin.exe successfully."
    }
    catch {
        Write-Error "An error occurred while copying and renaming sharpwin.exe."
        exit 1
    }
}
else {
    Write-Error "Original sharpwin.exe not found at $originalExePath"
    exit 1
}

# Read the current content of the file into an array, one line per array element
$serversFile = Join-Path -Path $emacspeak_dir -ChildPath "servers\.servers"
$currentContent = Get-Content -Path $serversFile

# Check if the file already contains "log-sharpwin"
if (-not ($currentContent -contains "log-sharpwin")) {
    # If not, append "log-sharpwin" to the file
    "log-sharpwin" | Out-File -FilePath $serversFile -Append
    Write-Host "Added 'log-sharpwin' to $serversFile"
}

# Check if the file already contains "sharpwin"
if (-not ($currentContent -contains "sharpwin")) {
    # If not, append "sharpwin" to the file
    "sharpwin" | Out-File -FilePath $serversFile -Append
    Write-Host "Added 'sharpwin' to $serversFile"
}

# Define the path pattern for .elc files in the emacspeak lisp directory
$elcFilesPattern = Join-Path $emacspeak_dir "lisp\*.elc"

# Use Remove-Item to delete all .elc files matching the pattern
try {
    Remove-Item -Path $elcFilesPattern -Force -ErrorAction Stop
    Write-Host "All .elc files in the lisp directory have been removed."
}
catch {
    Write-Error "An error occurred while trying to remove .elc files: $_"
}

# Change directory to $emacspeak_dir\lisp
Set-Location -Path "$emacspeak_dir\lisp"

# Set the Emacs command and flags
$emacsFlags = @(
    "--batch",
    "-q",
    "--no-site-file",
    "--load", "$emacspeak_dir\lisp\emacspeak-preamble.el",
    "--load", "$emacspeak_dir\lisp\emacspeak-autoload.el",
    "--funcall", "emacspeak-auto-generate-autoloads"
)

# Run the Emacs command with the specified flags
# Builds the loaddefs
& $emacs $emacsFlags

# Set the Emacs command and flags
$batchFlags = @(
    "--batch",
    "-q",
    "--no-site-file",
    "--funcall", "package-initialize",
    "--eval", "(setq file-name-handler-alist nil gc-cons-threshold 64000000 load-source-file-function nil)"
)
$depsFlags = @(
    "--load", "$emacspeak_dir\lisp\emacspeak-preamble.el",
    "--load", "$emacspeak_dir\lisp\emacspeak-loaddefs.el"
)
$compileFlags = @("--funcall", "batch-byte-compile")

# Get all .el files in the $emacspeak_dir/lisp directory
$elFiles = Get-ChildItem -Path "$emacspeak_dir\lisp" -Filter "*.el"

# Compile each .el file to .elc
foreach ($elFile in $elFiles) {
    $elcFile = "$($elFile.BaseName).elc"
    $allFlags = $batchFlags + $depsFlags + $compileFlags + $elFile.FullName
    & $emacs $allFlags
    if ($LASTEXITCODE -eq 0) {
        Write-Host "Compiled $elFile to $elcFile"
    } else {
        Write-Error "Failed to compile $elFile"
    }
}