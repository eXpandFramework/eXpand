using System.ComponentModel;
using DevExpress.ExpressApp.Model;

namespace Xpand.Persistent.Base.General.Model {
    public enum ClientSideSecurity {
        UIlevel,
        IntegratedMode
    }
    [ModelAbstractClass]
    public interface IModelOptionsClientSideSecurity : IModelOptions {
        [Category("eXpand")]
        [Description("When SecurityStrategyComplex is used defined the type of security for eXpand objectspaceprovider")]
        ClientSideSecurity? ClientSideSecurity { get; set; }
    }
}
