@echo off & if not "%ECHO%"=="" echo %ECHO%
call RegisterAssemblyFolders64bit.bat

rem x64
set templates="%ProgramFiles(x86)%\Microsoft Visual Studio 9.0\Common7\IDE\ProjectTemplates\CSharp\eXpand\"
set devenv="%ProgramFiles(x86)%\Microsoft Visual Studio 9.0\Common7\IDE\"

echo Installing and refreshing visual studio templates
xcopy "eXpand.DesignExperience\vs_templates\*.*" %templates% /Y /R /I

cd\
cd %devenv%
devenv.exe /InstallVSTemplates
pause