using System.Linq;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.ConditionalDetailViews.Model;

namespace Xpand.ExpressApp.ConditionalDetailViews.DomainLogic {
    [DomainLogic(typeof(IModelConditionalDetailViewRule))]
    public class ConditionalDetailViewRuleDomainLogic {
        public static IModelList<IModelDetailView> Get_DetailViews(IModelConditionalDetailViewRule conditionalDetailViewRule)
        {
            var calculatedModelNodeList = new CalculatedModelNodeList<IModelDetailView>();
            if (conditionalDetailViewRule.ModelClass != null) {
                var modelDetailViews = conditionalDetailViewRule.Application.Views.OfType<IModelDetailView>().Where(view => view.ModelClass==conditionalDetailViewRule.ModelClass);
                calculatedModelNodeList.AddRange(modelDetailViews);
            }
            return calculatedModelNodeList;
        }
    }
}