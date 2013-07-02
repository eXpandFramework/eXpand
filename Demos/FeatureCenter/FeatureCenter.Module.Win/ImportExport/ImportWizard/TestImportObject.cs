using System;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using Xpand.Xpo.Converters.ValueConverters;

namespace FeatureCenter.Module.Win.ImportExport.ImportWizard {


    [DisplayFeatureModel("FeatureCenter.Module.Win.ImportExport.ImportWizard.TestImportObject_ListView", "ImportWizard")]
    public class TestImportObject : BaseObject {
        private string _Code;

        [DisplayName("Code")]
        public string Code {
            get { return _Code; }
            set { SetPropertyValue("Code", ref _Code, value); }
        }

        private String _Name;
        [Size(255)]
        [DisplayName("Name")]
        public String Name {
            get { return _Name; }
            set { SetPropertyValue("Name", ref _Name, value); }
        }

        private DateTime _date;
        [ValueConverter(typeof(XpandUtcDateTimeConverter))]
        public DateTime Date {
            get {
                return _date;
            }
            set {
                SetPropertyValue("Date", ref _date, value);
            }
        }
        private int? _Age;
        
        [DisplayName("Age")]
        public int? Age {
            get { return _Age; }
            set { SetPropertyValue("Age", ref _Age, value); }
        }


        private int _IntNum;

        [DisplayName("IntNum")]
        public int IntNum {
            get { return _IntNum; }
            set { SetPropertyValue("IntNum", ref _IntNum, value); }
        }

        private TestGroupObject _Group;

        [DisplayName("Group")]
        public TestGroupObject Group {
            get { return _Group; }
            set { SetPropertyValue("Group", ref _Group, value); }
        }

        private string _description;
        [Size(SizeAttribute.Unlimited)]
        public string Description {
            get {
                return _description;
            }
            set {
                SetPropertyValue("Description", ref _description, value);
            }
        }
        public TestImportObject(Session session)
            : base(session) {
        }

        public TestImportObject() {
        }
    }
}