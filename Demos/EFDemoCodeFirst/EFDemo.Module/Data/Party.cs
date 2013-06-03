using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Drawing;
using System.ComponentModel.DataAnnotations.Schema;

using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.DC;

namespace EFDemo.Module.Data {
	[DefaultProperty("DisplayName")]
	public abstract class Party {
		private Image photo;
		protected Image GetPhoto() {
			if((photo == null) && (Photo_Binary != null)) {
				ImageConverter imageConverter = new ImageConverter();
				photo = (Image)imageConverter.ConvertFrom(Photo_Binary);
			}
			return photo;
		}
		protected void SetPhoto(Image value) {
			photo = value;
			if(photo != null) {
				ImageConverter imageConverter = new ImageConverter();
				Photo_Binary = (Byte[])imageConverter.ConvertTo(photo, typeof(Byte[]));
			}
			else {
				Photo_Binary = null;
			}
		}
		public Party() {
			PhoneNumbers = new List<PhoneNumber>();
			AssignedTasks = new List<Task>();
		}
		[Browsable(false)]
		public Int32 ID { get; protected set; }
		[Browsable(false)]
		public Byte[] Photo_Binary { get; protected set; }
		[Aggregated]
		public virtual IList<PhoneNumber> PhoneNumbers { get; set; }
		[Aggregated, ExpandObjectMembers(ExpandObjectMembers.Never)]
		public virtual Address Address1 { get; set; }
		[Aggregated, ExpandObjectMembers(ExpandObjectMembers.Never)]
		public virtual Address Address2 { get; set; }
		public virtual IList<Task> AssignedTasks { get; set; }
		[NotMapped, ObjectValidatorIgnoreIssue(typeof(ObjectValidatorDefaultPropertyIsVirtual), typeof(ObjectValidatorDefaultPropertyIsNonPersistentNorAliased))]
		public abstract String DisplayName { get; }
		public override String ToString() {
			return DisplayName;
		}
	}
}
