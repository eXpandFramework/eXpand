using System.ComponentModel;
using DevExpress.ExpressApp.DC;

namespace Xpand.Persistent.Base.ModelDifference {
    public interface ITypesInfoProvider {
        [Browsable(false)]
        ITypesInfo TypesInfo { get; set; }
    }
}
