using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using DevExpress.ExpressApp;
using Xpand.ExpressApp;

namespace ExternalApplication.Module.Win {
    [ToolboxItemFilter("Xaf.Platform.Win")]
    public sealed partial class ExternalApplicationWindowsFormsModule : XpandModuleBase {
        public ExternalApplicationWindowsFormsModule() {
            AdditionalExportedTypes.AddRange(ModuleHelper.CollectExportedTypesFromAssembly(Assembly.GetAssembly(typeof(Xpand.Persistent.BaseImpl.ImportExport.ClassInfoGraphNode)), IsExportedType));
            InitializeComponent();
        }

    }
}
