using System.Collections.Generic;
using System.ComponentModel;
using eXpand.ExpressApp.Logic.Model;

namespace eXpand.ExpressApp.ArtifactState.Model
{
    public interface IModelArtifactStateRule:IModelLogicRule
    {
        [Browsable(false)]
        IEnumerable<string> Modules { get; }
    }
}
