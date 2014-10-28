using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using Xpand.ExpressApp.ModelArtifactState.ArtifactState.Logic;

namespace Xpand.ExpressApp.ModelArtifactState.ActionState.Logic {
    public interface IContextActionStateRule:IContextArtifactStateRule,IActionStateRule {
        [DataSourceProperty("ActionContexts")]
        [Category("Data")]
        [Required(typeof(ContextActionStateRuleActionContextRequiredCalculator))]
        string ActionContext { get; set; }
    }

    public class ContextActionStateRuleActionContextRequiredCalculator:IModelIsRequired{
        public bool IsRequired(IModelNode node, string propertyName){
            return string.IsNullOrEmpty(((IActionStateRule) node).ActionId);
        }
    }
}