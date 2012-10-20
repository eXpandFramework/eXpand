using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.Persistent.BaseImpl;
using Xpand.ExpressApp.JobScheduler.Jobs.ThresholdCalculation;


namespace SecurityDemo.Module {
    public sealed partial class SecurityDemoModule : ModuleBase {
        public SecurityDemoModule() {
            AdditionalExportedTypes.AddRange(ModuleHelper.CollectExportedTypesFromAssembly(Assembly.GetAssembly(typeof(Analysis)), IsExportedType));
            AdditionalExportedTypes.AddRange(ModuleHelper.CollectExportedTypesFromAssembly(Assembly.GetAssembly(typeof(Xpand.Persistent.BaseImpl.Updater)), IsExportedType));
            AdditionalExportedTypes.AddRange(ModuleHelper.CollectExportedTypesFromAssembly(Assembly.GetAssembly(typeof(ThresholdSeverity)), IsExportedType));
            InitializeComponent();
        }

    }
}
