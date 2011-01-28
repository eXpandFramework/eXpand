using DevExpress.Xpo;
using FeatureCenter.Base;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.ExpressApp.Attributes;

namespace FeatureCenter.Module.Miscellaneous.PessimisticLocking {
    [PessimisticLocking]
    [Custom("ViewEditMode", "View")]
    [PessimisticLockingMessage("AnId")]
    public class PLCustomer : CustomerBase {
        public PLCustomer(Session session)
            : base(session) {
        }
    }
}
