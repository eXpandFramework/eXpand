@echo off & if not "%ECHO%"=="" echo %ECHO%
setlocal
set LOCALDIR=%CD:\=\\%

echo Registering exp[and assembly folder
echo Windows Registry Editor Version 5.00 >> expand_vs.reg
echo [HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\.NETFramework\AssemblyFolders\eXpand] >> expand_vs.reg
echo @="%LOCALDIR%\\eXpand.DLL" >> expand_vs.reg
regedit expand_vs.reg
del expand_vs.reg

rem x86
set templates = "%ProgramFiles%\Microsoft Visual Studio 9.0\Common7\IDE\ProjectTemplates\CSharp\eXpand"
set devenv = "%ProgramFiles%\Microsoft Visual Studio 9.0\Common7\IDE\devenv.exe"
rem x64
rem set templates = "C:\Program Files\Microsoft Visual Studio 9.0\Common7\IDE\ProjectTemplates\CSharp\eXpand"
rem set devenv = "C:\Program Files\Microsoft Visual Studio 9.0\Common7\IDE\devenv.exe"

echo Installing and refreshing visual studio templates
xcopy "eXpand.DesignExperience\vs_templates\*.*" %templates% /Y /R /I
%devenv% /InstallVSTemplates
pause