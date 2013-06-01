@echo on
set ProgramFiles=%ProgramFiles(x86)%
call defines.bat
del /q Xpand.dll

if exist Xpand.Key\Xpand.snk goto build
echo Generating strong key
mkdir Xpand.Key
%sn% -k Xpand.Key\Xpand.snk

:build


call buildProjects.cmd

%sn% -q -T Xpand.Dll\Xpand.Utils.dll > PublicKeyToken.txt

echo Installing and refreshing visual studio templates
xcopy "Xpand.DesignExperience\vs_templates\cs\*.*" %csharptemplates% /S /Y /H /I
xcopy "Xpand.DesignExperience\vs_templates\vb\*.*" %vbtemplates% /S /Y /H /I 

%SystemDrive%
cd\
cd %devenv%
devenv.exe /InstallVSTemplates

pause
