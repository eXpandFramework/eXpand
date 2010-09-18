using System;
using DevExpress.ExpressApp.Model.Core;

namespace Xpand.ExpressApp.ModelDifference.Core {
    public interface ISupportMasterModel {
        ModelApplicationBase MasterModel { get; }
        event EventHandler ModelCreated;
    }
}