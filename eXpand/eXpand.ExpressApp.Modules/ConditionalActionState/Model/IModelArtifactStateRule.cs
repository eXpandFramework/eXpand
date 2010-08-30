using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using eXpand.ExpressApp.Logic.Conditional.Model;

namespace eXpand.ExpressApp.ConditionalActionState.Model
{
    [ModelAbstractClass]
    public interface IModelArtifactStateRule:IModelConditionalLogicRule
    {
        [Browsable(false)]
        IEnumerable<string> Modules { get; }
    }
}
