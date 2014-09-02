using System;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;
using SchedulerTester.Module.BusinessObjects;
using Xpand.ExpressApp.Scheduler.Reminders;

namespace SchedulerTester.Module.DatabaseUpdate {
    public class Updater : ModuleUpdater {
        public Updater(IObjectSpace objectSpace, Version currentDBVersion) : base(objectSpace, currentDBVersion) { }
        public override void UpdateDatabaseAfterUpdateSchema() {
            base.UpdateDatabaseAfterUpdateSchema();
            if (ObjectSpace.FindObject<Customer>(null) == null) {
                var customer = ObjectSpace.CreateObject<Customer>();
                customer.FirstName = "Tolis1";
                var testEvent = ObjectSpace.CreateObject<TestEvent>();
                testEvent.StartOn=DateTime.Today;
                testEvent.CreateReminderInfoMember(ObjectSpace);
                customer.TestEvents.Add(testEvent);
                customer = ObjectSpace.CreateObject<Customer>();
                customer.FirstName = "Tolis2";
                ObjectSpace.CommitChanges();
            }
        }
    }
}
