using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.Logic.Conditional.Logic;
using Xpand.ExpressApp.Logic.Model;

namespace Xpand.ExpressApp.Logic.Conditional.Model {
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