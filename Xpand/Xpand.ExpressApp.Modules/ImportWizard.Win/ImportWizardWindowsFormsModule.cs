using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp;
using DevExpress.Utils;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.ImportWizard.Win {
    [ToolboxBitmap(typeof(ImportWizardWindowsFormsModule))]
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabWinModules)]
    public sealed class ImportWizardWindowsFormsModule : XpandModuleBase {
        public const string XpandImportWizardWin = "eXpand.ImportWizard.Win";

        public ImportWizardWindowsFormsModule() {
            ResourcesExportedToModel.Add(typeof(ImportWizResourceLocalizer));
            ResourcesExportedToModel.Add(typeof(ImportWizFrameTemplateLocalizer));
        }

        protected override ModuleTypeList GetRequiredModuleTypesCore() {
            var requiredModuleTypesCore = base.GetRequiredModuleTypesCore();
            requiredModuleTypesCore.AddRange(new[] { typeof(ImportWizardModule) });
            return requiredModuleTypesCore;
        }
    }
}