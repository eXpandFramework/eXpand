
@echo off

call defines.bat

echo Clear GAC...

call clearProject.bat Xpand.Utils
call clearProject.bat Xpand.Xpo
call clearProject.bat Xpand.Persistent.Base
call clearProject.bat Xpand.ExpressApp
call clearProject.bat Xpand.ExpressApp.Win
call clearProject.bat Xpand.ExpressApp.Web
call clearProject.bat Xpand.ExpressApp.Security
call clearProject.bat Xpand.ExpressApp.Validation
call clearProject.bat Xpand.ExpressApp.Validation.Win
call clearProject.bat Xpand.ExpressApp.Validation.Web
call clearProject.bat Xpand.ExpressApp.Logic
call clearProject.bat Xpand.ExpressApp.Logic.Win
call clearProject.bat Xpand.ExpressApp.Logic.Conditional
call clearProject.bat Xpand.ExpressApp.MemberLevelSecurity
call clearProject.bat Xpand.ExpressApp.MemberLevelSecurity.Win
call clearProject.bat Xpand.ExpressApp.MemberLevelSecurity.Web
call clearProject.bat Xpand.ExpressApp.AdditionalViewControlsProvider
call clearProject.bat Xpand.ExpressApp.AdditionalViewControlsProvider.Win
call clearProject.bat Xpand.ExpressApp.AdditionalViewControlsProvider.Web
call clearProject.bat Xpand.ExpressApp.ModelDifference
call clearProject.bat Xpand.ExpressApp.ModelDifference.Win
call clearProject.bat Xpand.ExpressApp.ModelDifference.Web
call clearProject.bat Xpand.ExpressApp.FilterDataStore
call clearProject.bat Xpand.ExpressApp.FilterDataStore.Win
call clearProject.bat Xpand.ExpressApp.FilterDataStore.Web
call clearProject.bat Xpand.ExpressApp.ImportWizard
call clearProject.bat Xpand.ExpressApp.ImportWizard.Win
call clearProject.bat Xpand.ExpressApp.ConditionalControllerState
call clearProject.bat Xpand.ExpressApp.ConditionalActionState
call clearProject.bat Xpand.ExpressApp.ArtifactState
call clearProject.bat Xpand.ExpressApp.ModelArtifactState
call clearProject.bat Xpand.ExpressApp.ModelArtifactState.Win
call clearProject.bat Xpand.ExpressApp.WizardUI.Win
call clearProject.bat Xpand.ExpressApp.ViewVariants
call clearProject.bat Xpand.Persistent.BaseImpl
call clearProject.bat Xpand.ExpressApp.WorldCreator
call clearProject.bat Xpand.ExpressApp.WorldCreator.Win
call clearProject.bat Xpand.ExpressApp.WorldCreator.Web
call clearProject4.bat Xpand.ExpressApp.ExceptionHandling
call clearProject4.bat Xpand.ExpressApp.ExceptionHandling.Win
call clearProject4.bat Xpand.ExpressApp.ExceptionHandling.Web
call clearProject.bat Xpand.ExpressApp.TreeListEditors
call clearProject.bat Xpand.ExpressApp.TreeListEditors.Web
call clearProject.bat Xpand.ExpressApp.TreeListEditors.Win
call clearProject.bat Xpand.ExpressApp.IO
call clearProject.bat Xpand.ExpressApp.IO.Win
call clearProject.bat Xpand.ExpressApp.IO.Web
call clearProject.bat Xpand.ExpressApp.PivotChart
call clearProject.bat Xpand.ExpressApp.PivotChart.Win
call clearProject.bat Xpand.ExpressApp.PivotChart.Web
call clearProject.bat Xpand.ExpressApp.Thumbnail.Web
call clearProject.bat Xpand.ExpressApp.NCarousel.Web
call clearProject.bat Xpand.NCarousel
call clearProject.bat Xpand.ExpressApp.MasterDetail
call clearProject.bat Xpand.ExpressApp.MasterDetail.Win
call clearProject.bat Xpand.ExpressApp.WorldCreator.SqlDBMapper
call clearProject.bat Xpand.ExpressApp.ConditionalDetailViews
call clearProject.bat Xpand.ExpressApp.EmailTemplateEngine
call clearProject4.bat Xpand.ExpressApp.JobScheduler
call clearProject4.bat Xpand.ExpressApp.JobScheduler.Jobs
call clearProject4.bat Xpand.ExpressApp.Workflow
call clearProject.bat Xpand.ExpressApp.StateMachine

rd /s /q %GACPATH%Microsoft.Practices.EnterpriseLibrary.Common.dll
rd /s /q %GACPATH%Microsoft.Practices.EnterpriseLibrary.Configuration.Design.DLL
rd /s /q %GACPATH%Microsoft.Practices.EnterpriseLibrary.Configuration.Design.UI.DLL
rd /s /q %GACPATH%Microsoft.Practices.EnterpriseLibrary.Logging.Configuration.Design.DLL
rd /s /q %GACPATH%Microsoft.Practices.EnterpriseLibrary.Logging.DLL
rd /s /q %GACPATH%Microsoft.Practices.ObjectBuilder2.DLL
rd /s /q %GACPATH%WM.EnterpriseLibraryExtensions.Logging.Configuration.Design.DLL
rd /s /q %GACPATH%WM.EnterpriseLibraryExtensions.Logging.DLL
rd /s /q .\FeatureCenter\FeatureCenter.Base\bin
rd /s /q .\FeatureCenter\FeatureCenter.Base\obj
rd /s /q .\FeatureCenter\FeatureCenter.Module\bin
rd /s /q .\FeatureCenter\FeatureCenter.Module\obj
rd /s /q .\FeatureCenter\FeatureCenter.Module.Win\bin
rd /s /q .\FeatureCenter\FeatureCenter.Module.Win\obj
rd /s /q .\FeatureCenter\FeatureCenter.Win\bin
rd /s /q .\FeatureCenter\FeatureCenter.Win\obj
rd /s /q .\FeatureCenter\ExternalApplication\ExternalApplication.Module.Win\bin
rd /s /q .\FeatureCenter\ExternalApplication\ExternalApplication.Module.Win\obj
rd /s /q .\FeatureCenter\ExternalApplication\ExternalApplication.Win\bin
rd /s /q .\FeatureCenter\ExternalApplication\ExternalApplication.Win\obj

pause