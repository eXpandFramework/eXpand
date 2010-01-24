@echo off & if not "%ECHO%"=="" echo %ECHO%
call RegisterAssemblyFolders32bit.bat

rem x86
set templates="%ProgramFiles%\Microsoft Visual Studio 9.0\Common7\IDE\ProjectTemplates\CSharp\eXpand\"
set devenv="%ProgramFiles%\Microsoft Visual Studio 9.0\Common7\IDE\"

echo Installing and refreshing visual studio templates
xcopy "eXpand.DesignExperience\vs_templates\*.*" %templates% /Y /R /I

cd\
cd %devenv%
devenv.exe /InstallVSTemplates
pause