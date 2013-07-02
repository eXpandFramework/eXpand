using System.ComponentModel;
using DevExpress.ExpressApp.Model;

namespace Xpand.Persistent.Base.Logic.Model {
    [ModelAbstractClass]
    public interface IModelConditionalLogicRule<T> : IModelConditionalLogicRule where T : ILogicRule {
        [Browsable(false)]
        T Attribute { get; set; }
    }
    [ModelAbstractClass]
    public interface IModelConditionalLogicRule : IModelLogicRule, IConditionalLogicRule {

    }
}