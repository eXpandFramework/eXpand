using DevExpress.ExpressApp.Model;

namespace Xpand.ExpressApp.AuditTrail.Model {
    public interface IModelApplicationAudiTrail:IModelNode {
        IModelLogicAuditTrail AudiTrail { get; }
    }
}