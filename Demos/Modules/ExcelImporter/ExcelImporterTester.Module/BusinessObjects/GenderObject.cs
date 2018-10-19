using System.ComponentModel;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace ExcelImporterTester.Module.BusinessObjects{
    [DefaultProperty("Gender1")]
    public abstract class GenderBase:BaseObject {
        protected GenderBase(Session session) : base(session){
        }
        private string _gender1;
        [DevExpress.Xpo.DisplayName("Gender")]
        public string Gender1{
            get => _gender1;
            set => SetPropertyValue("Gender1", ref _gender1, value);
        }
    }
    [DefaultClassOptions]
    public class GenderSuper:GenderBase {
        public GenderSuper(Session session) : base(session){
        }
        
    }
    
    [DefaultClassOptions]
    public class GenderObject : GenderBase{
        

        public GenderObject(Session session) : base(session){
        }

        // Fields...


        
    }
}