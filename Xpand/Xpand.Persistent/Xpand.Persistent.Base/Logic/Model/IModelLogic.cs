using System.Linq;
using DevExpress.ExpressApp.Model;

namespace Xpand.Persistent.Base.Logic.Model {
    public interface IModelLogicContexts : IModelNode {
        IModelExecutionContextsGroup ExecutionContextsGroup { get; }
        IModelActionExecutionContextGroup ActionExecutionContextGroup { get; }
        [ModelBrowsable(typeof(ObjectChangedExecutionContextGroupVisibilityCalculator))]
        IModelObjectChangedExecutionContextGroup ObjectChangedExecutionContextGroup { get; }
        IModelViewContextsGroup ViewContextsGroup { get; }
        IModelFrameTemplateContextsGroup FrameTemplateContextsGroup { get; }
    }

    public class ObjectChangedExecutionContextGroupVisibilityCalculator:IModelIsVisible{
        public bool IsVisible(IModelNode node, string propertyName){
            var modelExecutionContextsGroup = ((IModelLogicContexts) node).ExecutionContextsGroup;
            return modelExecutionContextsGroup.SelectMany(modelExecutionContexts => modelExecutionContexts).Any(modelExecutionContext => 
                modelExecutionContext.Name == ExecutionContext.ObjectSpaceObjectChanged.ToString());
        }
    }
}