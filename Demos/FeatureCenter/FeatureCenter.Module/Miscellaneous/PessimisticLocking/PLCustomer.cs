using DevExpress.ExpressApp.Model;
using DevExpress.Xpo;
using FeatureCenter.Base;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.Persistent.Base.General.Controllers;

namespace FeatureCenter.Module.Miscellaneous.PessimisticLocking {
    [PessimisticLocking]
    [ModelDefault("ViewEditMode", "View")]
    [PessimisticLockingMessage("AnId")]
    public class PLCustomer : CustomerBase {
        public PLCustomer(Session session)
            : base(session) {
        }
    }
}
