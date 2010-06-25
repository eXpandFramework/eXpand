using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using eXpand.ExpressApp.Logic.Model;

namespace eXpand.ExpressApp.ArtifactState.Model
{
    [ModelAbstractClass]
    public interface IModelArtifactStateRule:IModelLogicRule
    {
        [Browsable(false)]
        IEnumerable<string> Modules { get; }
    }
}
