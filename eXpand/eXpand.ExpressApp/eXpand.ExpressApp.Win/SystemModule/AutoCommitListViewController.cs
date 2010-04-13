using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win.SystemModule;
using eXpand.ExpressApp.Core.DictionaryHelpers;

namespace eXpand.ExpressApp.Win.SystemModule {
    public class AutoCommitListViewController : ViewController<ListView>
    {
        private const string AutoCommit = "AutoCommit";
        
        protected override void OnActivated()
        {
            base.OnActivated();
            var winDetailViewController = Frame.GetController<WinDetailViewController>();
            if (winDetailViewController != null && View.Info.GetAttributeBoolValue(AutoCommit))
                winDetailViewController.AutoCommitListView = true;
        }
        public override Schema GetSchema()
        {
            var dictionaryNode = new SchemaBuilder().InjectBoolAttribute(AutoCommit,ModelElement.ListView);
            return new Schema(dictionaryNode);
        }
    }
}