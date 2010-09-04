using System.ComponentModel;
using DevExpress.Persistent.Base;
using eXpand.ExpressApp.Logic.Conditional.Logic;

namespace eXpand.ExpressApp.ArtifactState.Logic {
    public interface IArtifactStateRule:IConditionalLogicRule {
        [Category("Data")]
        [DataSourceProperty("Modules")]
        string Module { get; set; }
    }
}