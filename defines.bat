@echo off
set configuration=Debug
set vsver=vs2013
rem uncomment the line bellow if you not use vs2010
rem set vsver=vs2008
rem uncomment the line bellow if you not use vs2012
rem set vsver=vs2012
rem set vsver=vs2013
set ProgramFiles=%ProgramFiles(x86)%
set msbuild="%ProgramFiles%\MSBuild\12.0\Bin\MSBuild.exe"

set GACPATH="%WinDir%\assembly\GAC_MSIL\"
set Gac4path="%WinDir%\Microsoft.NET\assembly\GAC_MSIL\"

if '%vsver%'=='vs2008' goto vs2008
if '%vsver%'=='vs2010' goto vs2010
if '%vsver%'=='vs2012' goto vs2012
if '%vsver%'=='vs2013' goto vs2013

:vs2013
set sn="%ProgramFiles%\Microsoft SDKs\Windows\v8.0A\Bin\NETFX 4.0 Tools\sn.exe"
set gacutil="%ProgramFiles%\Microsoft SDKs\Windows\v8.0A\Bin\NETFX 4.0 Tools\gacutil.exe"
set csharptemplates="%ProgramFiles%\Microsoft Visual Studio 12.0\Common7\IDE\ProjectTemplates\CSharp\DevExpress XAF\"
set vbtemplates="%ProgramFiles%\Microsoft Visual Studio 12.0\Common7\IDE\ProjectTemplates\VisualBasic\DevExpress XAF\"
set devenv="%ProgramFiles%\Microsoft Visual Studio 12.0\Common7\IDE\"
goto end
goto end

:vs2012
set sn="%ProgramFiles%\Microsoft SDKs\Windows\v8.0A\Bin\NETFX 4.0 Tools\sn.exe"
set gacutil="%ProgramFiles%\Microsoft SDKs\Windows\v8.0A\Bin\NETFX 4.0 Tools\gacutil.exe"
set csharptemplates="%ProgramFiles%\Microsoft Visual Studio 11.0\Common7\IDE\ProjectTemplates\CSharp\DevExpress XAF\"
set vbtemplates="%ProgramFiles%\Microsoft Visual Studio 11.0\Common7\IDE\ProjectTemplates\VisualBasic\DevExpress XAF\"
set devenv="%ProgramFiles%\Microsoft Visual Studio 11.0\Common7\IDE\"
goto end
goto end

:vs2008
set sn="%ProgramFiles%\Microsoft SDKs\Windows\v6.0A\Bin\sn.exe"
set gacutil="%ProgramFiles%\Microsoft SDKs\Windows\v6.0A\Bin\gacutil.exe"
set csharptemplates="%ProgramFiles%\Microsoft Visual Studio 9.0\Common7\IDE\ProjectTemplates\CSharp\DevExpress XAF\"
set vbtemplates="%ProgramFiles%\Microsoft Visual Studio 9.0\Common7\IDE\ProjectTemplates\VisualBasic\DevExpress XAF\"
set devenv="%ProgramFiles%\Microsoft Visual Studio 9.0\Common7\IDE\"

goto end

:vs2010
set sn="%ProgramFiles%\Microsoft SDKs\Windows\v7.0A\Bin\sn.exe"
set gacutil="%ProgramFiles%\Microsoft SDKs\Windows\v7.0A\Bin\NETFX 4.0 Tools\gacutil.exe"
set csharptemplates="%ProgramFiles%\Microsoft Visual Studio 10.0\Common7\IDE\ProjectTemplates\CSharp\DevExpress XAF\"
set vbtemplates="%ProgramFiles%\Microsoft Visual Studio 10.0\Common7\IDE\ProjectTemplates\VisualBasic\DevExpress XAF\"
set devenv="%ProgramFiles%\Microsoft Visual Studio 10.0\Common7\IDE\"
goto end

:end