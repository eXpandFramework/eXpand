@echo on

call defines.bat

if exist eXpand.Key\eXpand.snk goto build
echo Generating strong key
%sn% -k eXpand.Key\eXpand.snk

:build
call buildproject.cmd eXpand.Utils ".\eXpand\eXpand.Utils\eXpand.Utils.csproj"
call buildproject.cmd eXpand.Xpo ".\eXpand\eXpand.Xpo\eXpand.Xpo.csproj"
call buildproject.cmd eXpand.Persistent.Base ".\eXpand\eXpand.Persistent\eXpand.Persistent.Base\eXpand.Persistent.Base.csproj"
call buildproject.cmd eXpand.ExpressApp ".\eXpand\eXpand.ExpressApp\eXpand.ExpressApp\eXpand.ExpressApp.csproj"
call buildproject.cmd eXpand.ExpressApp.Win ".\eXpand\eXpand.ExpressApp\eXpand.ExpressApp.Win\eXpand.ExpressApp.Win.csproj"
call buildproject.cmd eXpand.ExpressApp.Web ".\eXpand\eXpand.ExpressApp\eXpand.ExpressApp.Web\eXpand.ExpressApp.Web.csproj"
call buildproject.cmd eXpand.Security ".\eXpand\eXpand.ExpressApp.Modules\Security\eXpand.ExpressApp.Security.csproj"
call buildproject.cmd eXpand.Validation ".\eXpand\eXpand.ExpressApp.Modules\Validation\eXpand.ExpressApp.Validation.csproj"
call buildproject.cmd eXpand.AdditionalViewControlsProvider ".\eXpand\eXpand.ExpressApp.Modules\AdditionalViewControlsProvider\eXpand.ExpressApp.AdditionalViewControlsProvider.csproj"
call buildproject.cmd eXpand.AdditionalViewControlsProvider.Win ".\eXpand\eXpand.ExpressApp.Modules\AdditionalViewControlsProvider.Win\eXpand.ExpressApp.AdditionalViewControlsProvider.Win.csproj"
call buildproject.cmd eXpand.AdditionalViewControlsProvider.Web ".\eXpand\eXpand.ExpressApp.Modules\AdditionalViewControlsProvider.Web\eXpand.ExpressApp.AdditionalViewControlsProvider.Web.csproj"
call buildproject.cmd eXpand.ModelDifference ".\eXpand\eXpand.ExpressApp.Modules\ModelDifference\eXpand.ExpressApp.ModelDifference.csproj"
call buildproject.cmd eXpand.ModelDifference.Win ".\eXpand\eXpand.ExpressApp.Modules\ModelDifference.Win\eXpand.ExpressApp.ModelDifference.Win.csproj"
call buildproject.cmd eXpand.ModelDifference.Web ".\eXpand\eXpand.ExpressApp.Modules\ModelDifference.Web\eXpand.ExpressApp.ModelDifference.Web.csproj"
call buildproject.cmd eXpand.FilterDataStore ".\eXpand\eXpand.ExpressApp.Modules\FilterDataStore\eXpand.ExpressApp.FilterDataStore.csproj"
call buildproject.cmd eXpand.FilterDataStore.Win ".\eXpand\eXpand.ExpressApp.Modules\FilterDataStore.Win\eXpand.ExpressApp.FilterDataStore.Win.csproj"
call buildproject.cmd eXpand.ModelArtifactState ".\eXpand\eXpand.ExpressApp.Modules\ModelArtifactState\eXpand.ExpressApp.ModelArtifactState.csproj"
call buildproject.cmd eXpand.ModelArtifactState.Win ".\eXpand\eXpand.ExpressApp.Modules\ModelArtifactState.Win\eXpand.ExpressApp.ModelArtifactState.Win.csproj"
call buildproject.cmd eXpand.WizardUI.Win ".\eXpand\eXpand.ExpressApp.Modules\WizardUI.Win\eXpand.ExpressApp.WizardUI.Win.csproj"


%sn% -q -T eXpand.Dll\eXpand.Utils.dll > PublicKeyToken.txt

pause
