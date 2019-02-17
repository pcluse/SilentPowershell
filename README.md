# SilentPowershell
Just a wrapper for powershell.exe so it runs silent without showing any console.

This is handy then you want to run powershell scripts from Scheduled tasks, as extensions in SCCM and more...

All output is logged to

C:\Windows\Temp\SilentPowerShell.log (if run by system)
C:\Users\xxx\AppData\Local\Temp\SilentPowerShell.log (if run by user)