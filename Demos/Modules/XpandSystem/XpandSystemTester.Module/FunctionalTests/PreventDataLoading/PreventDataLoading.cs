using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;

namespace XpandSystemTester.Module.FunctionalTests.PreventDataLoading {
    public class PreventDataLoading : ObjectViewController<ListView, PreventDataLoadingObject> {
        private readonly SingleChoiceAction _singleChoiceAction;

        public PreventDataLoading(){
            _singleChoiceAction = new SingleChoiceAction(this,GetType().Name,PredefinedCategory.View){
                ItemType = SingleChoiceActionItemType.ItemIsOperation
            };
        }

        public SingleChoiceAction SingleChoiceAction{
            get { return _singleChoiceAction; }
        }
    }
}
