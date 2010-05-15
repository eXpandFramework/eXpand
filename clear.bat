@echo off

call defines.bat

echo Clear GAC...

call clearProject.bat eXpand.Utils
call clearProject.bat eXpand.Xpo
call clearProject.bat eXpand.Persistent.Base
call clearProject.bat eXpand.ExpressApp
call clearProject.bat eXpand.ExpressApp.Win
call clearProject.bat eXpand.ExpressApp.Web
call clearProject.bat eXpand.ExpressApp.Security
call clearProject.bat eXpand.ExpressApp.Validation
call clearProject.bat eXpand.ExpressApp.Logic
call clearProject.bat eXpand.ExpressApp.Logic.Win
call clearProject.bat eXpand.ExpressApp.Logic.Conditional
call clearProject.bat eXpand.ExpressApp.MemberLevelSecurity
call clearProject.bat eXpand.ExpressApp.MemberLevelSecurity.Win
call clearProject.bat eXpand.ExpressApp.MemberLevelSecurity.Web
call clearProject.bat eXpand.ExpressApp.AdditionalViewControlsProvider
call clearProject.bat eXpand.ExpressApp.AdditionalViewControlsProvider.Win
call clearProject.bat eXpand.ExpressApp.AdditionalViewControlsProvider.Web
call clearProject.bat eXpand.ExpressApp.ModelDifference
call clearProject.bat eXpand.ExpressApp.ModelDifference.Win
call clearProject.bat eXpand.ExpressApp.ModelDifference.Web
call clearProject.bat eXpand.ExpressApp.FilterDataStore
call clearProject.bat eXpand.ExpressApp.FilterDataStore.Win
call clearProject.bat eXpand.ExpressApp.ModelArtifactState
call clearProject.bat eXpand.ExpressApp.ModelArtifactState.Win
call clearProject.bat eXpand.ExpressApp.WizardUI.Win
call clearProject.bat eXpand.ExpressApp.ViewVariants
call clearProject.bat eXpand.ExpressApp.ViewVariants.Win
call clearProject.bat eXpand.Persistent.BaseImpl
call clearProject.bat eXpand.ExpressApp.WorldCreator
call clearProject.bat eXpand.ExpressApp.WorldCreator.Win
call clearProject.bat eXpand.ExpressApp.WorldCreator.Web
call clearProject.bat eXpand.ExpressApp.WorldCreator.CThru
call clearProject.bat eXpand.ExpressApp.ExceptionHandling
call clearProject.bat eXpand.ExpressApp.ExceptionHandling.Win
call clearProject.bat eXpand.ExpressApp.ExceptionHandling.Web
call clearProject.bat eXpand.ExpressApp.TreeListEditors.Win
call clearProject.bat eXpand.ExpressApp.IO
call clearProject.bat eXpand.ExpressApp.IO.Win
call clearProject.bat eXpand.ExpressApp.PivotChart
call clearProject.bat eXpand.ExpressApp.PivotChart.Win
call clearProject.bat eXpand.ExpressApp.PivotChart.Web
call clearProject.bat eXpand.ExpressApp.NCarousel.Web
call clearProject.bat eXpand.NCarousel

rd /s /q %GACPATH%Microsoft.Practices.EnterpriseLibrary.Common.dll
rd /s /q %GACPATH%Microsoft.Practices.EnterpriseLibrary.Configuration.Design.DLL
rd /s /q %GACPATH%Microsoft.Practices.EnterpriseLibrary.Configuration.Design.UI.DLL
rd /s /q %GACPATH%Microsoft.Practices.EnterpriseLibrary.Logging.Configuration.Design.DLL
rd /s /q %GACPATH%Microsoft.Practices.EnterpriseLibrary.Logging.DLL
rd /s /q %GACPATH%Microsoft.Practices.ObjectBuilder2.DLL
rd /s /q %GACPATH%WM.EnterpriseLibraryExtensions.Logging.Configuration.Design.DLL
rd /s /q %GACPATH%WM.EnterpriseLibraryExtensions.Logging.DLL
pause