@echo on

call defines.bat

if exist eXpand.Key\eXpand.snk goto build
echo Generating strong key
mkdir eXpand.Key
%sn% -k eXpand.Key\eXpand.snk

:build
call RegisterAssemblyFolders32bit.bat


call buildprojects.cmd

%msbuild% /nologo /t:Rebuild /verbosity:quiet /p:Configuration=%configuration% ".\eXpand.AddIns\eXpandAddIns.csproj"
%msbuild% /nologo /t:Rebuild /verbosity:quiet /p:Configuration=%configuration% ".\eXpand.AddIns\DevExpress.ExpressApp.ModelEditor\DevExpress.ExpressApp.ModelEditor.csproj"

%sn% -q -T eXpand.Dll\eXpand.Utils.dll > PublicKeyToken.txt

rem Install VS Template
set templates="%ProgramFiles%\Microsoft Visual Studio 9.0\Common7\IDE\ProjectTemplates\CSharp\eXpand\"
set devenv="%ProgramFiles%\Microsoft Visual Studio 9.0\Common7\IDE\"

echo Installing and refreshing visual studio templates
xcopy "eXpand.DesignExperience\vs_templates\*.*" %templates% /Y /R /I

cd\
cd %devenv%
devenv.exe /InstallVSTemplates

pause
