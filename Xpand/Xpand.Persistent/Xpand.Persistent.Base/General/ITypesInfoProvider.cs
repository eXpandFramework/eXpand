using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;

namespace Xpand.Persistent.Base.General {
    [ModelAbstractClass]
    public interface ITypesInfoProvider:IModelApplication {
        [Browsable(false)]
        ITypesInfo TypesInfo { get; set; }
    }
    [DomainLogic((typeof(ITypesInfoProvider)))]
    public class TypesInfoProviderDomainLogic {
        public static ITypesInfo Get_TypesInfo(ITypesInfoProvider typesInfoProvider) {
            return XafTypesInfo.Instance;
        }
    }

}
