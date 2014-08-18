using System;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl.EF;

namespace EFDemo.Module.Data {
	[ImageName("BO_FileAttachment")]
	public class PortfolioFileData : FileAttachment {
		public PortfolioFileData()
			: base() {
			DocumentType = DocumentType.Unknown;
		}

		[Browsable(false)]
		public Int32 DocumentType_Int { get; protected set; }
		public virtual Resume Resume { get; set; }

		[NotMapped]
		public DocumentType DocumentType {
			get { return (DocumentType)DocumentType_Int; }
			set { DocumentType_Int = (Int32)value; }
		}
	}
}
