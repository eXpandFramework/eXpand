using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.Logic.Conditional.Model;
using Xpand.ExpressApp.ModelAdaptor.Logic;

namespace Xpand.ExpressApp.ModelAdaptor.Model {
    [ModelInterfaceImplementor(typeof(IModelAdaptorRule), "Attribute")]
    [ModelAbstractClass]
    public interface IModelModelAdaptorRule : IModelAdaptorRule, IModelConditionalLogicRule<IModelAdaptorRule> {
        
    }
}
