using System.ComponentModel;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace ExcelImporterTester.Module.BusinessObjects{
    [DefaultClassOptions]
    [DefaultProperty(nameof(ImportName))]
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

        public string ImportName{
            get => _importName;
            set => SetPropertyValue("ImportName", ref _importName, value);
        }
    }
}