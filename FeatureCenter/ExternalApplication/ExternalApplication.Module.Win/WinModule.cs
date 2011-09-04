using System.ComponentModel;
using System.Reflection;
using DevExpress.ExpressApp;

namespace ExternalApplication.Module.Win {
    [ToolboxItemFilter("Xaf.Platform.Win")]
    public sealed partial class ExternalApplicationWindowsFormsModule : ModuleBase {
        public ExternalApplicationWindowsFormsModule() {
            AdditionalExportedTypes.AddRange(ModuleHelper.CollectExportedTypesFromAssembly(Assembly.GetAssembly(typeof(Xpand.Persistent.BaseImpl.ImportExport.ClassInfoGraphNode))));
            InitializeComponent();
        }

    }
}
