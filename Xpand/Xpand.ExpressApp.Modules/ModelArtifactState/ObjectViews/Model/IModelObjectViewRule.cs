using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.ModelArtifactState.ObjectViews.Logic;
using Xpand.Persistent.Base.Logic.Model;

namespace Xpand.ExpressApp.ModelArtifactState.ObjectViews.Model {
    [ModelInterfaceImplementor(typeof(IContextObjectViewRule), "Attribute")]
    public interface IModelObjectViewRule : IContextObjectViewRule, IModelConditionalLogicRule<IObjectViewRule> {
        [Browsable(false)]
        IModelList<IModelObjectView> ObjectViews { get; }
    }

    [DomainLogic(typeof(IModelObjectViewRule))]
    public class ObjectViewRuleDomainLogic {
        public static IModelList<IModelObjectView> Get_ObjectViews(IModelObjectViewRule objectViewRule) {
            var calculatedModelNodeList = new CalculatedModelNodeList<IModelObjectView>();
            if (objectViewRule.ModelClass != null) {
                var modelDetailViews = objectViewRule.Application.Views.OfType<IModelObjectView>().Where(view => view.ModelClass == objectViewRule.ModelClass);
                calculatedModelNodeList.AddRange(modelDetailViews);
            }
            return calculatedModelNodeList;
        }
        public static string Get_ActionExecutionContextGroup(IObjectViewRule objectViewRule) {
            return ObjectViewActionExecutionContextsGroupNodeUpdater.ObjectViewActionContext;
        }

    }

}
