@echo on
set ProgramFiles=%ProgramFiles%
call defines.bat

if exist Xpand.Key\Xpand.snk goto build
echo Generating strong key
mkdir Xpand.Key
%sn% -k Xpand.Key\Xpand.snk

:build
call RegisterAssemblyFolders32bit.bat


call buildprojects.cmd


rem Install VS Template
set csharptemplates="%ProgramFiles%\Microsoft Visual Studio 10.0\Common7\IDE\ProjectTemplates\CSharp\eXpressApp Framework\"
set vbtemplates="%ProgramFiles%\Microsoft Visual Studio 10.0\Common7\IDE\ProjectTemplates\VisualBasic\eXpressApp Framework\"
set devenv="%ProgramFiles%\Microsoft Visual Studio 10.0\Common7\IDE\"

echo Installing and refreshing visual studio templates
xcopy "Xpand.DesignExperience\vs_templates\cs\*.*" %csharptemplates% /Y /R /I
xcopy "Xpand.DesignExperience\vs_templates\vb\*.*" %vbtemplates% /Y /R /I

%SystemDrive%
cd\
cd %devenv%
devenv.exe /InstallVSTemplates

pause
