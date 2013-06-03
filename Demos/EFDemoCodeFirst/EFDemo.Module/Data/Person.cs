using System;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.General;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Filtering;

namespace EFDemo.Module.Data {
	[DefaultProperty("FullName")]
	[ImageName("BO_Person")]
	public class Person : Party, IPerson {
		public String FirstName { get; set; }
		public String LastName { get; set; }
		public String MiddleName { get; set; }
		public Nullable<DateTime> Birthday { get; set; }
		[FieldSize(255)]
		public String Email { get; set; }

		[NotMapped, ObjectValidatorIgnoreIssue(typeof(ObjectValidatorDefaultPropertyIsNonPersistentNorAliased)), SearchMemberOptions(SearchMemberMode.Include)]
		public String FullName {
			get { return ObjectFormatter.Format(FullNameFormat, this, EmptyEntriesMode.RemoveDelimeterWhenEntryIsEmpty); }
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override String DisplayName {
			get { return FullName; }
		}
		
		public static String FullNameFormat = "{FirstName} {MiddleName} {LastName}";

		DateTime IPerson.Birthday {
			get {
				if(Birthday.HasValue) {
					return Birthday.Value;
				}
				else {
					return DateTime.MinValue;
				}
			}
			set { Birthday = value; }
		}

		public void SetFullName(String fullName) {
			FirstName = fullName;
		}
	}
}
