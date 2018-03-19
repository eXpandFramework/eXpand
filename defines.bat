@echo off
set configuration=Debug
set vsver=vs2017
set ProgramFiles=%ProgramFiles(x86)%

set GACPATH="%WinDir%\assembly\GAC_MSIL\"
set Gac4path="%WinDir%\Microsoft.NET\assembly\GAC_MSIL\"


:vs2017
set msbuild="%ProgramFiles%\Microsoft Visual Studio\2017\BuildTools\MSBuild\15.0\Bin\MSBuild.exe"
IF NOT EXIST %msbuild% goto VS2017Tools
goto other

:other
set sn="%ProgramFiles%\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.6 Tools\sn.exe"
set gacutil="%ProgramFiles%\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.6 Tools\gacutil.exe"
goto end


:VS2017Tools
echo "Although most of the projects can compile with an earlier msbuild version, these scripts are designed to compilete the whole framework, so you need to install VS Build Tools 2017"
pause
exit
:end