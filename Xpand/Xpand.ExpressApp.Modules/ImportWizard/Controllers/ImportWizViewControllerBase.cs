using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;

namespace Xpand.ExpressApp.ImportWizard.Controllers {
    public abstract class ImportWizViewControllerBase : ViewController {
        private readonly SimpleAction _importAction;

        protected ImportWizViewControllerBase() {
            TargetViewType = ViewType.ListView;
            _importAction = new SimpleAction(this, "ImportWizardWinActionId","Export") {
                Caption = "Import from...",
                ImageName = "Import",
            };
            _importAction.Execute += ImportAction_Execute;
        }

        protected override void OnActivated() {
            if (View.ObjectTypeInfo.IsAbstract)
                _importAction.Active.SetItemValue(@"test", false);

            base.OnActivated();
        }

        private void ImportAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
            var objectSpace = Application.CreateObjectSpace(View.ObjectTypeInfo.Type);

            ShowWizard(objectSpace);
            View.Refresh();
            View.ObjectSpace.Refresh();
        }

        public abstract void ShowWizard(IObjectSpace objectSpace);
        

    }
}
