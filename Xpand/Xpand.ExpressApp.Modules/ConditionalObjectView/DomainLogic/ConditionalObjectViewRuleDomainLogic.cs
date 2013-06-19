using System.Linq;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.ConditionalObjectView.Model;

namespace Xpand.ExpressApp.ConditionalObjectView.DomainLogic {
    [DomainLogic(typeof(IModelConditionalObjectViewRule))]
    public class ConditionalObjectViewRuleDomainLogic {
        public static IModelList<IModelDetailView> Get_DetailViews(IModelConditionalObjectViewRule conditionalObjectViewRule) {
            var calculatedModelNodeList = new CalculatedModelNodeList<IModelDetailView>();
            if (conditionalObjectViewRule.ModelClass != null) {
                var modelDetailViews = conditionalObjectViewRule.Application.Views.OfType<IModelDetailView>().Where(view => view.ModelClass == conditionalObjectViewRule.ModelClass);
                calculatedModelNodeList.AddRange(modelDetailViews);
            }
            return calculatedModelNodeList;
        }
    }
}