using System.ComponentModel;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.Utils;
using Xpand.ExpressApp.Scheduler.Reminders;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.Controllers.Dashboard;

namespace Xpand.ExpressApp.Scheduler {
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabWinWebModules)]
    public sealed class XpandSchedulerModule : XpandModuleBase,IDashboardInteractionUser {
        public const string XpandScheduler = "Scheduler";
        public XpandSchedulerModule() {
            RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Scheduler.SchedulerModuleBase));
            RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.ConditionalAppearance.ConditionalAppearanceModule));
        }

        public override void AddGeneratorUpdaters(ModelNodesGeneratorUpdaters updaters) {
            base.AddGeneratorUpdaters(updaters);
            updaters.Add(new ReminderInfoModelMemberUpdater());
            updaters.Add(new ReminderInfoAppearenceRuleUpdater());
        }
    }
}