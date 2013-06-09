using System;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;

namespace EFDemo.Module.Data {
	[DefaultProperty("File")]
	[FileAttachmentAttribute("File")]
	public abstract class FileAttachment {
		[Browsable(false)]
		public Int32 ID { get; protected set; }
		[ExpandObjectMembers(ExpandObjectMembers.Never), RuleRequiredField("FileAttachmentBaseRule", "Save", "File should be assigned")]
		public virtual FileData File { get; set; }
	}
}
