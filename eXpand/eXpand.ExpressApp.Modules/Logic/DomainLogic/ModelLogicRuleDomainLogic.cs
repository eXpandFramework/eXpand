using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using eXpand.ExpressApp.Logic.Model;
using System.Linq;

namespace eXpand.ExpressApp.Logic.DomainLogic
{
    [DomainLogic(typeof(IModelLogicRule))]
    public class ModelLogicRuleDomainLogic
    {
        public static List<string> Get_ExecutionContexts(IModelLogicRule modelLogicRule)
        {
            var contexts = ((IModelLogic)modelLogicRule.Parent.Parent).GroupContexts;
            return contexts.Select(groupContext => groupContext.Id).ToList();

        }

        public static IModelList<IModelView> Get_Views(IModelLogicRule modelLogicRule) {
            var calculatedModelNodeList = new CalculatedModelNodeList<IModelView>();
            if (modelLogicRule.ModelClass== null)
                return calculatedModelNodeList;
            IEnumerable<IModelView> modelViews = modelLogicRule.Application.Views.Where(view => view.ModelClass == modelLogicRule.ModelClass);

            if (modelLogicRule.ViewType != ViewType.Any)
                modelViews =
                    modelViews.Where(modelView =>(modelLogicRule.ViewType == ViewType.ListView
                             ? modelView is IModelListView
                             : modelView is IModelDetailView));
            calculatedModelNodeList.AddRange(modelViews);
            return calculatedModelNodeList;
        }
    }
}
