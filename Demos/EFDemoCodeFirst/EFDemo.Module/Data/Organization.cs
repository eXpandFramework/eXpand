using System;
using System.Linq;
using System.Collections.Generic;

using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;

namespace EFDemo.Module.Data {
	public class Organization : Party {
		public String FullName { get; set; }
		public String Profile { get; set; }
		public String Email { get; set; }
		public String WebSite { get; set; }
		[FieldSize(4096), ObjectValidatorIgnoreIssue(typeof(ObjectValidatorLargeNonDelayedMember))]
		public String Description { get; set; }
		public String Name { get; set; }
		
		[ObjectValidatorIgnoreIssue(typeof(ObjectValidatorDefaultPropertyIsVirtual))]
        public override String DisplayName {
            get { return Name; }
        }
    }
}
