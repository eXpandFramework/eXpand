@echo off
set configuration=Debug
set vsver=vs2015
REM set vsver=vs2017
set ProgramFiles=%ProgramFiles(x86)%

set GACPATH="%WinDir%\assembly\GAC_MSIL\"
set Gac4path="%WinDir%\Microsoft.NET\assembly\GAC_MSIL\"


if '%vsver%'=='vs2015' goto vs2015

:vs2015
set msbuild="%ProgramFiles%\MSBuild\14.0\Bin\MSBuild.exe"

:vs2017
set msbuild="%ProgramFiles%\Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0\Bin\MSBuild.exe"

IF NOT EXIST %msbuild% goto VS2015Tools

set sn="%ProgramFiles%\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.6 Tools\sn.exe"
set gacutil="%ProgramFiles%\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.6 Tools\gacutil.exe"
goto end


:VS2015Tools
echo "You need to download Build Tools 2015 https://www.microsoft.com/en-us/download/confirmation.aspx?id=48159"
pause
exit
:end