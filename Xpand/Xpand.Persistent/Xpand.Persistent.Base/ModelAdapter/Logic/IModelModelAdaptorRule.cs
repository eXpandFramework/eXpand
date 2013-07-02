using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.Logic.Model;

namespace Xpand.Persistent.Base.ModelAdapter.Logic {
    [ModelInterfaceImplementor(typeof(IModelAdaptorRule), "Attribute")]
    [ModelAbstractClass]
    public interface IModelModelAdaptorRule : IModelAdaptorRule, IModelConditionalLogicRule<IModelAdaptorRule> {
        
    }
}
