using System;
using DevExpress.Xpo;
using DevExpress.Persistent.BaseImpl;

namespace Foxhound.ExpressApp.Scheduler.Controllers{
    [NonPersistent]
    public class CreateDateRangeActionParameters : BaseObject {
        public CreateDateRangeActionParameters(Session session) : base(session) { }

        private DateTime startDate;
        public DateTime StartDate {
            get {
                return startDate;
            }
            set {
                SetPropertyValue("StartDate", ref startDate, value);
            }
        }

        private DateTime endDate;
        public DateTime EndDate {
            get {
                return endDate;
            }
            set {
                SetPropertyValue("EndDate", ref endDate, value);
            }
        }
    }
}