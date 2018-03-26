using System.ComponentModel;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using Xpand.ExpressApp.ExcelImporter.Services;

namespace ExcelImporterTester.Module.BusinessObjects{
    [DefaultClassOptions]
    [DefaultProperty(nameof(ImportName1))]
    public class Customer : BaseObject{
        // Fields...
        private GenderObject _gender;
        private string _importName;

        public Customer(Session session) : base(session){
        }

        public GenderObject Gender{
            get => _gender;
            set => SetPropertyValue("Gender", ref _gender, value);
        }
        [DevExpress.Xpo.DisplayName("ImportName")]
        public string ImportName1{
            get => _importName;
            set => SetPropertyValue("ImportName", ref _importName, value);
        }

        string _hidden1;

        [VisibleInListView(false)]
        public string Hidden1{
            get => _hidden1;
            set => SetPropertyValue(nameof(Hidden1), ref _hidden1, value);
        }

        string _hidden4;
        [Browsable(false)]
        public string Hidden4{
            get => _hidden4;
            set => SetPropertyValue(nameof(Hidden4), ref _hidden4, value);
        }
        string _hidden2;

        [VisibleInLookupListView(false)]
        public string Hidden2{
            get => _hidden2;
            set => SetPropertyValue(nameof(Hidden2), ref _hidden2, value);
        }

        string _hidden3;
        [VisibleInExcelMap(false)]
        public string Hidden3{
            get => _hidden3;
            set => SetPropertyValue(nameof(Hidden3), ref _hidden3, value);
        }
    }
}