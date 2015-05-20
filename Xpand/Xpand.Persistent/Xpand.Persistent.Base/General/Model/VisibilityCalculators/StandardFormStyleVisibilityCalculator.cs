using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.XtraBars.Ribbon;

namespace Xpand.Persistent.Base.General.Model.VisibilityCalculators{
    public class StandardFormStyleVisibilityCalculator : IModelIsVisible {
        public bool IsVisible(IModelNode node, string propertyName){
            return ((IModelOptionsWin) node.Application.Options).FormStyle == RibbonFormStyle.Standard;
        }
    }
}