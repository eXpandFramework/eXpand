using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.General;

namespace EFDemo.Module.Data {
    [DefaultProperty("Text")]
    public class Note : INote {
		[Browsable(false)]
		public Int32 ID { get; protected set; }
		public String Author { get; set; }
		public Nullable<DateTime> DateTime { get; set; }
		[FieldSize(FieldSizeAttribute.Unlimited), ObjectValidatorIgnoreIssue(typeof(ObjectValidatorLargeNonDelayedMember))]
		public String Text { get; set; }
		
		DateTime INote.DateTime {
            get {
                if(DateTime.HasValue) {
                    return DateTime.Value;
                }
                else {
                    return System.DateTime.MinValue;
                }
            }
            set { DateTime = value; }
        }
    }
}
