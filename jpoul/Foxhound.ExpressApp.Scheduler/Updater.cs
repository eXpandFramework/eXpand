using System;
using DevExpress.ExpressApp.Updating;
using DevExpress.Xpo;

namespace Foxhound.ExpressApp.Scheduler{
    public class Updater : ModuleUpdater{
        public Updater(Session session, Version currentDBVersion) : base(session, currentDBVersion) {}

        public override void UpdateDatabaseAfterUpdateSchema(){
            base.UpdateDatabaseAfterUpdateSchema();

            //Session.BeginTransaction();
            //PersistentEnum.RegisterPersistentEnum(
            //    Session
            //    , PersistentCalendar.CalendarTypeGroup
            //    , PersistentCalendar.CalendarTypeUndefined
            //    , PersistentCalendar.CalendarTypeUndefined
            //    , typeof (FoxhoundSchedulerModule).FullName);

            //PersistentEnum.RegisterPersistentEnum(
            //    Session
            //    , PersistentDateRange.DateRangeTypeGroup
            //    , PersistentDateRange.DateRangeTypeUndefined
            //    , PersistentDateRange.DateRangeTypeUndefined
            //    , typeof (FoxhoundSchedulerModule).FullName);


            //PersistentEnum.RegisterPersistentEnum(
            //    Session
            //    , PersistentDateRange.DateRangeTypeGroup
            //    , PersistentDateRange.DateRangeTypeCalendarWeek
            //    , PersistentDateRange.DateRangeTypeCalendarWeek
            //    , typeof (FoxhoundSchedulerModule).FullName);

            //PersistentEnum.RegisterPersistentEnum(
            //    Session
            //    , PersistentDateRangeTemplate.DateRangeTemplateTypeGroup
            //    , PersistentDateRangeTemplate.DateRangeTemplateTypeUndefined
            //    , PersistentDateRangeTemplate.DateRangeTemplateTypeUndefined
            //    , typeof (FoxhoundSchedulerModule).FullName);

            //PersistentCalendar.RegisterPersistentCalendar(
            //    Session
            //    , "Default Calendar"
            //    , 2000
            //    , PersistentCalendar.CalendarTypeUndefined
            //    , typeof (FoxhoundSchedulerModule).FullName);
            //Session.CommitTransaction();
        }

    }
}