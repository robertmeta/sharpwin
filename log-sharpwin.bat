@echo off
REM Example of using -Command for inline PowerShell commands with piping
powershell -ExecutionPolicy Bypass -Command "& { process { $_ } }"
