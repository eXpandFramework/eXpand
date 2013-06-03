using System;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.General;
using DevExpress.XtraReports.UI;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Reports;

namespace EFDemo.Module.Data {
	[DefaultProperty("ReportName")]
	public class ReportData_EF : IReportData, IXtraReportData, IInplaceReport {
		[Browsable(false)]
		public Int32 ID { get; protected set; }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public Byte[] Content { get; set; }
		[Browsable(false)]
		public String DataTypeName { get; set; }
		[VisibleInListView(false)]
		public Boolean IsInplaceReport { get; set; }
		public String ReportName { get; set; }

		[NotMapped, Browsable(false)]
		public Type DataType {
			get {
				Type result = null;
				if(!String.IsNullOrWhiteSpace(DataTypeName)) {
					ITypeInfo typeInfo = XafTypesInfo.Instance.FindTypeInfo(DataTypeName);
					if(typeInfo != null) {
						result = typeInfo.Type;
					}
				}
				return result;
			}
			set {
				if(value != null) {
					DataTypeName = value.FullName;
				}
				else {
					DataTypeName = "";
				}
			}
		}
		[NotMapped, DisplayName("Data Type")]
		public String DataTypeCaption {
			get { return CaptionHelper.GetClassCaption(DataTypeName); }
		}

		public XtraReport LoadReport(IObjectSpace objectSpace) {
			XafReport result = new XafReport();
			result.ObjectSpace = objectSpace;
			XafReportSerializationHelper.LoadReport(this, result);
			return result;
		}
		public void SaveReport(XtraReport report) {
			XafReport xafReport = report as XafReport;
			if(xafReport == null) {
				throw new ArgumentException("XafReport is expected", "report");
			}
			XafReportSerializationHelper.SaveReport(this, xafReport);
		}
	}
}
