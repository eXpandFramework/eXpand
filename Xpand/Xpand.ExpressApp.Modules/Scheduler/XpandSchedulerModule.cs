using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.Utils;
using Xpand.ExpressApp.Scheduler.Reminders;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.Controllers.Dashboard;

namespace Xpand.ExpressApp.Scheduler {
    public interface IModelScheduler:IModelNode {
        [DefaultValue(1000)]
        int RemindersCheckInterval { get; set; }            
    }
    public interface IModelApplicationScheduler {
        IModelScheduler Scheduler { get; }
    }
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabWinWebModules)]
    public sealed class XpandSchedulerModule : XpandModuleBase,IDashboardUser {
        public const string XpandScheduler = "Scheduler";
        public XpandSchedulerModule() {
            RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Scheduler.SchedulerModuleBase));
            RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.ConditionalAppearance.ConditionalAppearanceModule));
        }

        public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            base.ExtendModelInterfaces(extenders);
            extenders.Add<IModelApplication,IModelApplicationScheduler>();
        }

        public override void AddGeneratorUpdaters(ModelNodesGeneratorUpdaters updaters) {
            base.AddGeneratorUpdaters(updaters);
            updaters.Add(new ReminderInfoModelMemberUpdater());
            updaters.Add(new ReminderInfoAppearenceRuleUpdater());
        }
    }
}