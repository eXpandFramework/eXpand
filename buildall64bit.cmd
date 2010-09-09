@echo on

call defines.bat

if exist Xpand.Key\Xpand.snk goto build
echo Generating strong key
mkdir Xpand.Key
%sn% -k Xpand.Key\Xpand.snk

:build
call RegisterAssemblyFolders64bit.bat

call buildProjects.cmd

%sn% -q -T Xpand.Dll\Xpand.Utils.dll > PublicKeyToken.txt

rem Install VS Template
set templates="%ProgramFiles(x86)%\Microsoft Visual Studio 10.0\Common7\IDE\ProjectTemplates\CSharp\eXpressApp Framework\"
set devenv="%ProgramFiles(x86)%\Microsoft Visual Studio 10.0\Common7\IDE\"

echo Installing and refreshing visual studio templates
xcopy "Xpand.DesignExperience\vs_templates\*.*" %templates% /Y /R /I

%SystemDrive%
cd\
cd %devenv%
devenv.exe /InstallVSTemplates

pause
