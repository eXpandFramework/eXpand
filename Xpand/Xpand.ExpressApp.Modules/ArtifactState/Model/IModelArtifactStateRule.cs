using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.Logic.Conditional.Model;

namespace Xpand.ExpressApp.ArtifactState.Model
{
    [ModelAbstractClass]
    public interface IModelArtifactStateRule:IModelConditionalLogicRule
    {
        [Browsable(false)]
        IEnumerable<string> Modules { get; }
    }
}
