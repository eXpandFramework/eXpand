using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.Logic.Model;

namespace Xpand.ExpressApp.Logic.DomainLogic {
    [DomainLogic(typeof(IModelLogicRule))]
    public class ModelLogicRuleDomainLogic {
        public static List<string> Get_ExecutionContexts(IModelLogicRule modelLogicRule) {
            var contexts = ((IModelLogic)modelLogicRule.Parent.Parent).ExecutionContextsGroup;
            return contexts.Select(groupContext => groupContext.Id).ToList();
        }
        public static List<string> Get_ViewContexts(IModelLogicRule modelLogicRule) {
            var contexts = ((IModelLogic)modelLogicRule.Parent.Parent).ViewContextsGroup;
            return contexts.Select(groupContext => groupContext.Id).ToList();
        }
        public static List<string> Get_FrameTemplateContexts(IModelLogicRule modelLogicRule) {
            var contexts = ((IModelLogic)modelLogicRule.Parent.Parent).FrameTemplateContextsGroup;
            return contexts.Select(groupContext => groupContext.Id).ToList();
        }

        public static IModelList<IModelView> Get_Views(IModelLogicRule modelLogicRule) {
            var calculatedModelNodeList = new CalculatedModelNodeList<IModelView>();
            if (modelLogicRule.ModelClass == null)
                return calculatedModelNodeList;
            IEnumerable<IModelObjectView> modelViews = modelLogicRule.Application.Views.OfType<IModelObjectView>().Where(view => view.ModelClass == modelLogicRule.ModelClass);

            if (modelLogicRule.ViewType != ViewType.Any)
                modelViews =
                    modelViews.Where(modelView => (modelLogicRule.ViewType == ViewType.ListView
                             ? modelView is IModelListView
                             : modelView is IModelDetailView));
            calculatedModelNodeList.AddRange(modelViews.Cast<IModelView>());
            return calculatedModelNodeList;
        }
    }
}
