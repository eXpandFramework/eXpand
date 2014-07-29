@echo off
set version=14.1
set junctionDir=C:\Projects\%version%\BuildLabel\Temp\NetStudio.v%version%.2005
set targetDir=C:\DevExpress %version%\Components\Sources

rd /s /q C:\Projects\%version%
mkdir %junctionDir%
rd %junctionDir%
junction -s %junctionDir% "%targetDir%"

set winJunctionDir=%junctionDir%\Win
rd /s /q %winJunctionDir%
junction -s %winJunctionDir% "%targetDir%"

set xafJunctionDir=%junctionDir%\X
rd /s /q %xafJunctionDir%
mkdir %xafJunctionDir%

set XAFDirs=DevExpress.ExpressApp  DevExpress.ExpressApp.Workflow DevExpress.ExpressApp.Design DevExpress.ExpressApp.Tools DevExpress.Persistent
for %%x in (%XAFDirs%) do for /f %%l in ('dir /b "%targetDir%"\%%x') do junction -s %xafJunctionDir%\%%l "%targetDir%"\%%x\%%l

junction -s %xafJunctionDir%\DevExpress.ExpressApp.Modules "%targetDir%\DevExpress.ExpressApp.Modules"










