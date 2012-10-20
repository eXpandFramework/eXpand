using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using FeatureCenter.Base;

namespace ExternalApplication.Module.Win {
    
    [DefaultClassOptions]
    public class ExternalCustomer : CustomerBase {
        public ExternalCustomer(Session session) : base(session) {
        }
    }
}