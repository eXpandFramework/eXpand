using System.ComponentModel;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace ExcelImporterTester.Module.BusinessObjects{
    [DefaultProperty("Gender")]
    [DefaultClassOptions]
    public class GenderObject : BaseObject{
        private string _gender;

        public GenderObject(Session session) : base(session){
        }

        // Fields...


        public string Gender{
            get => _gender;
            set => SetPropertyValue("Gender", ref _gender, value);
        }
    }
}