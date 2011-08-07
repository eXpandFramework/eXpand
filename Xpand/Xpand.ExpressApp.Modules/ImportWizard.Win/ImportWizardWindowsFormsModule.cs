
using System.ComponentModel;
using System.Drawing;

namespace Xpand.ExpressApp.ImportWizard.Win {
    [ToolboxBitmap(typeof(ImportWizardWindowsFormsModule))]
    [ToolboxItem(true)]
    public sealed partial class ImportWizardWindowsFormsModule : XpandModuleBase {
        public const string XpandImportWizardWin = "eXpand.ImportWizard.Win";
        public ImportWizardWindowsFormsModule() {
            InitializeComponent();
        }
    }
}