using DevExpress.ExpressApp.Model;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using eXpand.ExpressApp.Logic.Conditional.Model;

namespace eXpand.ExpressApp.AdditionalViewControlsProvider.Model {
    [ModelInterfaceImplementor(typeof(IAdditionalViewControlsRule), "Attribute")]
    public interface IModelAdditionalViewControlsRule : IAdditionalViewControlsRule, IModelConditionalLogicRule<IAdditionalViewControlsRule>
    {
        
    }

    
}