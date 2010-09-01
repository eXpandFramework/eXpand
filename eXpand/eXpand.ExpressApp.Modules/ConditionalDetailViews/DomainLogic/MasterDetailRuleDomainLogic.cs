using System.Linq;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using eXpand.ExpressApp.ConditionalDetailViews.Model;

namespace eXpand.ExpressApp.ConditionalDetailViews.DomainLogic {
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