@echo off
cls
SET RULENAME=EasyTest
ECHO Create in/out firewall rules for all *.exe files with the rulename of "%RULENAME%" ?
ECHO.
ECHO.

pause
Echo.
FOR /r %%G in ("*.exe") Do (@echo %%G
NETSH advfirewall firewall add rule name="%RULENAME%-%%~nxG" dir=in program="%%G" action="allow" enable="yes")
FOR /r %%G in ("*.exe") Do (@echo %%G
NETSH advfirewall firewall add rule name="%RULENAME%-%%~nxG" dir=out program="%%G" action="allow" enable="yes")
Echo.
Echo done.
Echo.
GOTO :Finish
:norulename
Echo Error! - You did not specify a Rulename type - Addfwrs "Rulename"
Echo.
:Finish
Echo Batch ended...