using System.ComponentModel;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;

namespace Xpand.Persistent.Base.ModelDifference {
    [ModelAbstractClass]
    public interface ITypesInfoProvider:IModelApplication {
        [Browsable(false)]
        ITypesInfo TypesInfo { get; set; }
    }
}
