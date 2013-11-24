using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;

namespace Xpand.Persistent.Base.ModelDifference {
    [ModelAbstractClass]
    public interface IModelTypesInfoProvider:IModelApplication {
        [Browsable(false)]
        ITypesInfo TypesInfo { get; set; }
    }
    [DomainLogic((typeof(IModelTypesInfoProvider)))]
    public class TypesInfoProviderDomainLogic {
        public static ITypesInfo Get_TypesInfo(IModelTypesInfoProvider typesInfoProvider) {
            return XafTypesInfo.Instance;
        }
    }

}
