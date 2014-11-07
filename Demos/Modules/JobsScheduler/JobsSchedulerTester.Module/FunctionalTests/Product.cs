using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using Xpand.Xpo;

namespace JobsSchedulerTester.Module.FunctionalTests{
    [DefaultClassOptions]
    public class Product : XpandCustomObject{
        private int _parts;

        public Product(Session session) : base(session){
        }

        public int Parts{
            get { return _parts; }
            set { SetPropertyValue("Parts", ref _parts, value); }
        }
    }
}