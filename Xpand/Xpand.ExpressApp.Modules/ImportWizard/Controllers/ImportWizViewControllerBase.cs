using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;

namespace Xpand.ExpressApp.ImportWizard.Controllers {
    public abstract partial class ImportWizViewControllerBase : ViewController {
        protected ImportWizViewControllerBase() {
            InitializeComponent();
            RegisterActions(components);
            TargetViewType = ViewType.ListView;
        }

        protected override void OnActivated() {
            if (View.ObjectTypeInfo.IsAbstract)
                ImportAction.Active.SetItemValue(@"test", false);

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
