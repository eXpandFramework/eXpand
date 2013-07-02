using System.Linq;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.ConditionalObjectView.Model;

namespace Xpand.ExpressApp.ConditionalObjectView.DomainLogic {
    [DomainLogic(typeof(IModelConditionalObjectViewRule))]
    public class ConditionalObjectViewRuleDomainLogic {
        public static IModelList<IModelObjectView> Get_ObjectViews(IModelConditionalObjectViewRule conditionalObjectViewRule) {
            var calculatedModelNodeList = new CalculatedModelNodeList<IModelObjectView>();
            if (conditionalObjectViewRule.ModelClass != null) {
                var modelDetailViews = conditionalObjectViewRule.Application.Views.OfType<IModelObjectView>().Where(view => view.ModelClass == conditionalObjectViewRule.ModelClass);
                calculatedModelNodeList.AddRange(modelDetailViews);
            }
            return calculatedModelNodeList;
        }
    }
}