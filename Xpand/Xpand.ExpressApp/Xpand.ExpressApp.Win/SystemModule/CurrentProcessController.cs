using System.Diagnostics;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;

namespace Xpand.ExpressApp.Win.SystemModule {
    public interface IModelOpptionCurrentProcess {
        IModelCurrentProcess CurrentProcess { get; }
    }

    public interface IModelCurrentProcess:IModelNode {
        ProcessPriorityClass? PriorityClass { get; set; }
        bool? PriorityBoostEnabled { get; set; }
    }

    public class CurrentProcessController:Controller, IModelExtender {
        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelOptions,IModelOpptionCurrentProcess>();
        }

        public static void ApplyConfig(IModelOptions modelOptions) {
            var currentProcess = Process.GetCurrentProcess();
            var modelCurrentProcess = ((IModelOpptionCurrentProcess) modelOptions).CurrentProcess;
            var priorityClass = modelCurrentProcess.PriorityClass;
            if (priorityClass.HasValue)
                currentProcess.PriorityClass = priorityClass.Value;
            var priorityBoostEnabled = modelCurrentProcess.PriorityBoostEnabled;
            if (priorityBoostEnabled.HasValue)
                currentProcess.PriorityBoostEnabled = priorityBoostEnabled.Value;
        }
    }
}
