using System.ComponentModel;
using System.Drawing;
using Xpand.ExpressApp.Win.SystemModule;

namespace Xpand.ExpressApp.ImportWizard.Win {
    [ToolboxBitmap(typeof(ImportWizardWindowsFormsModule))]
    [ToolboxItem(true)]
    public sealed partial class ImportWizardWindowsFormsModule : XpandModuleBase {
        public const string XpandImportWizardWin = "eXpand.ImportWizard.Win";

        public ImportWizardWindowsFormsModule() {
            ResourcesExportedToModel.Add(typeof(ImportWizResourceLocalizer));
            ResourcesExportedToModel.Add(typeof(ImportWizFrameTemplateLocalizer));
            RequiredModuleTypes.AddRange(new[] { Xpand.ExpressApp.ImportWizard.Core });
        }

        protected override DevExpress.ExpressApp.ModuleTypeList GetRequiredModuleTypesCore() {
            var requiredModuleTypesCore = base.GetRequiredModuleTypesCore();
            requiredModuleTypesCore.AddRange(new[] { typeof(XpandSystemWindowsFormsModule) });
            requiredModuleTypesCore.AddRange(new[] { typeof(ImportWizardModule) });
            return requiredModuleTypesCore;
        }
    }
}