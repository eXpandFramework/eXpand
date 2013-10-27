using DevExpress.ExpressApp.Model;

namespace Xpand.Persistent.Base.Security {
    public interface IModelRegistrationEnabled:IModelNode {
        bool Enabled { get; set; }
    }
}