@echo off

set exver=v0.1
set configuration=Debug

set GACPATH="%WinDir%\assembly\GAC_MSIL\"

rem VS2008 paths
set vsver=vs2008
set sn="%ProgramFiles%\Microsoft SDKs\Windows\v6.0A\Bin\sn.exe" 
set gacutil="%ProgramFiles%\Microsoft SDKs\Windows\v6.0A\Bin\gacutil.exe"
set msbuild="%WinDir%\Microsoft.NET\Framework\v3.5\MSBuild.exe"