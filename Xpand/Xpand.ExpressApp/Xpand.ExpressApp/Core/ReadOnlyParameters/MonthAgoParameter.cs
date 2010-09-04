using System;
using DevExpress.Persistent.Base;

namespace Xpand.ExpressApp.Core.ReadOnlyParameters
{
    public class MonthAgoParameter:ReadOnlyParameter
    {
        public MonthAgoParameter() : base("MonthAgo", typeof(DateTime)) {
        }

        public override object CurrentValue {
            get { return DateTime.Today.AddDays(-30); }
        }
    }
}
