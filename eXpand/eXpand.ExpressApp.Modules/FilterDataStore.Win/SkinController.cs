using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Win.SystemModule;
using eXpand.ExpressApp.FilterDataStore.Core;
using eXpand.ExpressApp.FilterDataStore.Win.Providers;

namespace eXpand.ExpressApp.FilterDataStore.Win
{
    public class SkinController:ViewController
    {
        protected override void OnActivated()
        {
            base.OnActivated();
            Frame.GetController<ChooseSkinController>().ChooseSkinAction.ExecuteCompleted+=ChooseSkinActionOnExecuteCompleted;
        }

        void ChooseSkinActionOnExecuteCompleted(object sender, ActionBaseEventArgs actionBaseEventArgs) {
            SkinFilterProvider skinFilterProvider = FilterProviderManager.Providers.OfType<SkinFilterProvider>().FirstOrDefault();
            if (skinFilterProvider != null) {
                skinFilterProvider.FilterValue = ((IModelApplicationOptionsSkin)Application.Model.Options).Skin;
                Frame.GetController<RefreshController>().RefreshAction.DoExecute();
            }
        }
    }
}
