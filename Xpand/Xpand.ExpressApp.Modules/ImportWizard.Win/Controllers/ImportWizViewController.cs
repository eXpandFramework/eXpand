using DevExpress.ExpressApp;
using Xpand.ExpressApp.ImportWizard.Controllers;
using Xpand.ExpressApp.ImportWizard.Win.Wizard;

namespace Xpand.ExpressApp.ImportWizard.Win.Controllers {
    /// <summary>
    /// Win Implementation of ImportWizViewController that has the Import action and exceute event handler
    /// </summary>
    public partial class ImportWizViewController : ImportWizViewControllerBase {
        public ImportWizViewController() {
            InitializeComponent();
            RegisterActions(components);
            TargetViewType = ViewType.ListView;
        }

        public override void ShowWizard(IObjectSpace objectSpace) {
            var wiz = new ExcelImportWizard(
                            (ObjectSpace)objectSpace,
                            View.ObjectTypeInfo,
                            this.GetCurrentCollectionSource());

            wiz.ShowDialog();
        }


    }
}
