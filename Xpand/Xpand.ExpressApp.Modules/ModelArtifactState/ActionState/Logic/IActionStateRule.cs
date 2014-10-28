using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using Xpand.ExpressApp.ModelArtifactState.ArtifactState.Logic;

namespace Xpand.ExpressApp.ModelArtifactState.ActionState.Logic {
    public interface IActionStateRule : IArtifactStateRule {
        [Required(typeof(ActionStateRuleActionIdRequiredCalculator))]
        [DataSourceProperty("Actions")]
        [Category("Data")]
        string ActionId { get; set; }

        [Category("Behavior")]
        ActionState ActionState { get; set; }
    }

    public class ActionStateRuleActionIdRequiredCalculator:IModelIsRequired{
        public bool IsRequired(IModelNode node, string propertyName){
            return string.IsNullOrEmpty(((IContextActionStateRule) node).ActionContext);
        }
    }
}