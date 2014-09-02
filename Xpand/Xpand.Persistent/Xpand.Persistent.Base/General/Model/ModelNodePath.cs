using System.ComponentModel;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;

namespace Xpand.Persistent.Base.General.Model {
    public interface IModelNodePath:IModelNode{
        [Category(AttributeCategoryNameProvider.Xpand)]
        string PathToHere { get;  }
    }

    [DomainLogic(typeof(IModelNodePath))]
    public class ModelNodePathDomainLogic{
        public string Get_PathToHere(IModelNodePath modelNodePath) {
            return modelNodePath.Path();
        }
    }
}
