using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using eXpand.ExpressApp.Logic.Conditional.Logic;
using eXpand.ExpressApp.Logic.Model;

namespace eXpand.ExpressApp.Logic.Conditional.Model {
    [ModelAbstractClass]
    public interface IModelConditionalLogicRule<T> : IModelConditionalLogicRule where T : ILogicRule
    {
        [Browsable(false)]
        T Attribute { get; set; }
    }

    public interface IModelConditionalLogicRule:IModelLogicRule,IConditionalLogicRule 
    {
        
    }
}