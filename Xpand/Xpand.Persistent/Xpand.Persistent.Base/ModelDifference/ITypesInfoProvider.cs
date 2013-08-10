using System.ComponentModel;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.General;

namespace Xpand.Persistent.Base.ModelDifference {
    [ModelAbstractClass]
    public interface ITypesInfoProvider:IModelApplication {
        [Browsable(false)]
        ITypesInfo TypesInfo { get; set; }
    }
    [DomainLogic((typeof(ITypesInfoProvider)))]
    public class TypesInfoProviderDomainLogic {
        public static ITypesInfo Get_TypesInfo(ITypesInfoProvider typesInfoProvider) {
            return XpandModuleBase.TypesInfo;
        }
    }

}
