using System.ComponentModel;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using Xpand.Persistent.Base.General.Controllers;

namespace SystemTester.Module.FunctionalTests.PessimisticLocking{
    [PessimisticLocking]
    [DefaultClassOptions]
    public class PessimisticLockingObject : XPLiteObject{
        private int _key;
        private string _name;

        public PessimisticLockingObject(Session session) : base(session){
        }

        [Key(true)]
        [Browsable(false)]
        public int Key{
            get { return _key; }
            set { SetPropertyValue("Key", ref _key, value); }
        }

        public string Name{
            get { return _name; }
            set { SetPropertyValue("Name", ref _name, value); }
        }
    }
}