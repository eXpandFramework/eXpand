using System;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace PivotChartTester.Module.BusinessObjects {
    [DefaultClassOptions]
    public class TestClass:BaseObject {
        public TestClass(Session session) : base(session){
        }

        string _projectName;
        [Size(255)]
        
        public string ProjectName {
            get { return _projectName; }
            set { SetPropertyValue("ProjectName", ref _projectName, value); }
        }
        DateTime _dateRegistered;
        
        public DateTime DateRegistered {
            get { return _dateRegistered; }
            set { SetPropertyValue<DateTime>("DateRegistered", ref _dateRegistered, value); }
        }
        decimal _originalCostEstimate;
        
        public decimal OriginalCostEstimate {
            get { return _originalCostEstimate; }
            set { SetPropertyValue<decimal>("OriginalCostEstimate", ref _originalCostEstimate, value); }
        }
    }
}
