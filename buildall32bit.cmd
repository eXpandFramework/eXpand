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


echo Installing and refreshing visual studio templates
xcopy "Xpand.DesignExperience\vs_templates\cs\*.*" %csharptemplates% /Y /R /I
xcopy "Xpand.DesignExperience\vs_templates\vb\*.*" %vbtemplates% /Y /R /I

%SystemDrive%
cd\
cd %devenv%
devenv.exe /InstallVSTemplates

pause
