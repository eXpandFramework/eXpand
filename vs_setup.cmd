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
rem set templates="%ProgramFiles%\Microsoft Visual Studio 9.0\Common7\IDE\ProjectTemplates\CSharp\eXpand\"
rem set devenv="%ProgramFiles%\Microsoft Visual Studio 9.0\Common7\IDE\"
rem x64
set templates="%ProgramFiles(x86)%\Microsoft Visual Studio 9.0\Common7\IDE\ProjectTemplates\CSharp\eXpand\"
set devenv="%ProgramFiles(x86)%\Microsoft Visual Studio 9.0\Common7\IDE\"

echo Installing and refreshing visual studio templates
xcopy "eXpand.DesignExperience\vs_templates\*.*" %templates% /Y /R /I

cd\
cd %devenv%
devenv.exe /InstallVSTemplates
pause