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
call clearProject.bat eXpand.ExpressApp.RuleModeller
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
call clearProject.bat eXpand.ExpressApp.Taxonomies
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

pause