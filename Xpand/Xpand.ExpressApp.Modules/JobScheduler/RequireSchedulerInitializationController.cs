using System;
using DevExpress.ExpressApp;
using System.Linq;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.ExpressApp.Logic;
using Xpand.Persistent.Base.JobScheduler;
using Xpand.Utils.Helpers;
using Xpand.ExpressApp.Core;

namespace Xpand.ExpressApp.JobScheduler {
    public class RequireSchedulerInitializationController : ViewController<DetailView> {
        public RequireSchedulerInitializationController() {
            TargetObjectType = typeof (IRequireSchedulerInitialization);
        }

        public override void CustomizeTypesInfo(DevExpress.ExpressApp.DC.ITypesInfo typesInfo) {
            base.CustomizeTypesInfo(typesInfo);
            var typeInfos = typesInfo.PersistentTypes.Where(info => info.ImplementedInterfaces.Contains(typesInfo.FindTypeInfo(typeof(IRequireSchedulerInitialization))));
            typeInfos.Each(typeInfo =>typeInfo.AddAttribute(new AdditionalViewControlsRuleAttribute("RequireSchedulerInitialization_for_"+typeInfo.Name, "1=1",
                                                                              "1=1", "Scheduler is not started",Position.Top){ViewType = ViewType.DetailView}));
        }
        protected override void OnFrameAssigned() {
            base.OnFrameAssigned();
            Frame.GetController<AdditionalViewControlsRuleViewController>().LogicRuleExecuting+=OnLogicRuleExecuting;
            Frame.Disposing+=FrameOnDisposing;
        }

        void FrameOnDisposing(object sender, EventArgs eventArgs) {
            Frame.GetController<AdditionalViewControlsRuleViewController>().LogicRuleExecuting -= OnLogicRuleExecuting;
        }

        void OnLogicRuleExecuting(object sender, LogicRuleExecutingEventArgs<IAdditionalViewControlsRule> logicRuleExecutingEventArgs) {
            if (logicRuleExecutingEventArgs.LogicRuleInfo.Rule.TypeInfo.Implements<IRequireSchedulerInitialization>()) {
                logicRuleExecutingEventArgs.LogicRuleInfo.Active =!Application.FindModule<JobSchedulerModule>().Scheduler.IsStarted;
                View.AllowEdit["SchedulerNotStarted"] = !logicRuleExecutingEventArgs.LogicRuleInfo.Active;
            }
        }
    }
}