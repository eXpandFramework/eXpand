using DevExpress.ExpressApp.Model;

namespace Xpand.Persistent.Base.Security {
    [ModelAbstractClass]
    public interface IModelOptionsRegistration : IModelOptions {
        IModelRegistrationEnabled Registration { get; }
    }
}