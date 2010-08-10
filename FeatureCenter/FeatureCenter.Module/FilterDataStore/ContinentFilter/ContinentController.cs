using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using eXpand.ExpressApp.FilterDataStore.Core;
using System.Linq;

namespace FeatureCenter.Module.FilterDataStore.ContinentFilter {
    public class ContinentController:ViewController<ListView> {
        public ContinentController() {
            TargetObjectType = typeof (FDSCCustomer);
            var singleChoiceAction = new SingleChoiceAction(this,"Select Continent",PredefinedCategory.Filters);
            singleChoiceAction.Items.Add(new ChoiceActionItem("All", null));
            singleChoiceAction.Items.Add(new ChoiceActionItem("Asia", new List<string> {"Hong Kong"}));
            singleChoiceAction.Items.Add(new ChoiceActionItem("Europe", new List<string> {"Paris", "London"}));
            singleChoiceAction.Items.Add(new ChoiceActionItem("America", new List<string> {"New York"}));
            singleChoiceAction.Execute+=SingleChoiceActionOnExecute;
            singleChoiceAction.ItemType=SingleChoiceActionItemType.ItemIsOperation;            
        }
        protected override void OnDeactivating()
        {
            base.OnDeactivating();
            FilterProviderManager.Providers.OfType<ContinentFilterProvider>().Single().FilterValue=null;
        }
        void SingleChoiceActionOnExecute(object sender, SingleChoiceActionExecuteEventArgs singleChoiceActionExecuteEventArgs) {

            FilterProviderManager.Providers.OfType<ContinentFilterProvider>().Single().FilterValue = singleChoiceActionExecuteEventArgs.SelectedChoiceActionItem.Data;
            ObjectSpace.Refresh();
        }
    }
}