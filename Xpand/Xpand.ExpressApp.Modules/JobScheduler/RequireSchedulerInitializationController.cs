using System;
using DevExpress.ExpressApp;
using System.Linq;
using Quartz;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.ExpressApp.Logic;
using Xpand.Persistent.Base.JobScheduler;
using Xpand.Utils.Helpers;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.JobScheduler {
    public class RequireSchedulerInitializationController : ViewController<DetailView> {
        public RequireSchedulerInitializationController() {
            TargetObjectType = typeof(IRequireSchedulerInitialization);
        }

        public override void CustomizeTypesInfo(DevExpress.ExpressApp.DC.ITypesInfo typesInfo) {
            base.CustomizeTypesInfo(typesInfo);
            var typeInfos = typesInfo.PersistentTypes.Where(info => info.ImplementedInterfaces.Contains(typesInfo.FindTypeInfo(typeof(IRequireSchedulerInitialization))));
            typeInfos.Each(typeInfo => typeInfo.AddAttribute(new AdditionalViewControlsRuleAttribute("RequireSchedulerInitialization_for_" + typeInfo.Name, "1=1",
                                                                              "1=1", "Scheduler is not started", Position.Top) { ViewType = ViewType.DetailView }));
        }
        protected override void OnFrameAssigned() {
            base.OnFrameAssigned();
            var additionalViewControlsRuleViewController = Frame.GetController<AdditionalViewControlsRuleViewController>();
            if (additionalViewControlsRuleViewController == null)
                throw new NullReferenceException("Use the application designer to drag and drop the AdditionalViewControlsProvider module");
            additionalViewControlsRuleViewController.LogicRuleExecuting += OnLogicRuleExecuting;
            Frame.Disposing += FrameOnDisposing;
        }

        void FrameOnDisposing(object sender, EventArgs eventArgs) {
            Frame.GetController<AdditionalViewControlsRuleViewController>().LogicRuleExecuting -= OnLogicRuleExecuting;
        }

        void OnLogicRuleExecuting(object sender, LogicRuleExecutingEventArgs<IAdditionalViewControlsRule> logicRuleExecutingEventArgs) {
            if (logicRuleExecutingEventArgs.LogicRuleInfo.Rule.TypeInfo.Implements<IRequireSchedulerInitialization>()) {
                View.AllowEdit["SchedulerNotStarted"] = GetScedulerState(logicRuleExecutingEventArgs.LogicRuleInfo);
            }
        }

        bool GetScedulerState(LogicRuleInfo<IAdditionalViewControlsRule> logicRuleInfo) {
            IScheduler scheduler = Application.FindModule<JobSchedulerModule>().Scheduler;
            if (scheduler != null) {
                logicRuleInfo.Active = !scheduler.IsStarted;
                return !logicRuleInfo.Active;
            }
            return false;
        }
    }
}