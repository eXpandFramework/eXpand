using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.Logic;
using Xpand.Persistent.Base.Logic.Model;

namespace Xpand.ExpressApp.Logic.DomainLogic {
    public static class ModelLogicRuleDomainLogic {
        public static IModelLogicWrapper Get_ModelLogicWrapper(IModelLogicRule modelLogicRule) {
            return LogicInstallerManager.Instance[modelLogicRule].GetModelLogic(modelLogicRule.Application);
        }

        public static List<string> Get_ActionExecutionContexts(IModelLogicRule modelLogicRule) {
            var actionExecutionContextGroup = modelLogicRule.ModelLogicWrapper.ActionExecutionContextGroup;
            return actionExecutionContextGroup != null ? actionExecutionContextGroup.Select(groupContext => groupContext.Id).ToList() : new List<string>();
        }

        public static bool Get_GroupContext(IModelLogicRule modelLogicRule) {
            return !string.IsNullOrEmpty(modelLogicRule.ExecutionContextGroup) ||
                   !string.IsNullOrEmpty(modelLogicRule.ActionExecutionContextGroup);
        }

        public static List<string> Get_ExecutionContexts(IModelLogicRule modelLogicRule) {
            return modelLogicRule.ModelLogicWrapper.ExecutionContextsGroup.Select(groupContext => groupContext.Id).ToList();
        }
        public static List<string> Get_ViewContexts(IModelLogicRule modelLogicRule) {
            return modelLogicRule.ModelLogicWrapper.ViewContextsGroup.Select(groupContext => groupContext.Id).ToList();
        }
        public static List<string> Get_FrameTemplateContexts(IModelLogicRule modelLogicRule) {
            return modelLogicRule.ModelLogicWrapper.FrameTemplateContextsGroup.Select(templateContexts => templateContexts.Id).ToList();
        }

        public static IModelList<IModelView> Get_Views(IModelLogicRule modelLogicRule) {
            var calculatedModelNodeList = new CalculatedModelNodeList<IModelView>();
            if (modelLogicRule.ModelClass == null)
                return calculatedModelNodeList;
            var modelViews = modelLogicRule.Application.Views.OfType<IModelObjectView>().Where(view => view.ModelClass == modelLogicRule.ModelClass);

            if (modelLogicRule.ViewType != ViewType.Any)
                modelViews =
                    modelViews.Where(modelView => (modelLogicRule.ViewType == ViewType.ListView
                             ? modelView is IModelListView
                             : modelView is IModelDetailView));
            calculatedModelNodeList.AddRange(modelViews);
            return calculatedModelNodeList;
        }
    }

}
