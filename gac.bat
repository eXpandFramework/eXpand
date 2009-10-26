@echo on

call defines.bat

cd eXpand.Dll

%gacutil% -i eXpand.Utils.dll
%gacutil% -i eXpand.Xpo.dll
%gacutil% -i eXpand.Persistent.Base.dll
%gacutil% -i eXpand.Persistent.TaxonomyImpl.dll
%gacutil% -i eXpand.ExpressApp.dll
%gacutil% -i eXpand.ExpressApp.Win.dll
%gacutil% -i eXpand.ExpressApp.Web.dll
%gacutil% -i eXpand.ExpressApp.Security.dll
%gacutil% -i eXpand.ExpressApp.Validation.dll
%gacutil% -i eXpand.ExpressApp.AdditionalViewControlsProvider.dll
%gacutil% -i eXpand.ExpressApp.AdditionalViewControlsProvider.Win.dll
%gacutil% -i eXpand.ExpressApp.AdditionalViewControlsProvider.Web.dll
%gacutil% -i eXpand.ExpressApp.ModelDifference.dll
%gacutil% -i eXpand.ExpressApp.ModelDifference.Win.dll
%gacutil% -i eXpand.ExpressApp.ModelDifference.Web.dll
%gacutil% -i eXpand.ExpressApp.FilterDataStore.dll
%gacutil% -i eXpand.ExpressApp.FilterDataStore.Win.dll
%gacutil% -i eXpand.ExpressApp.ModelArtifactState.dll
%gacutil% -i eXpand.ExpressApp.ModelArtifactState.Win.dll
%gacutil% -i eXpand.ExpressApp.WizardUI.Win.dll
%gacutil% -i eXpand.ExpressApp.Taxonomy.dll
%gacutil% -i eXpand.ExpressApp.Taxonomy.Win.dll
%gacutil% -i eXpand.ExpressApp.Taxonomy.Web.dll

cd..

pause