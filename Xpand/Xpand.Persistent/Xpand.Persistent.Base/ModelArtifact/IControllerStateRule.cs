using System;
using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.Persistent.Base;

namespace Xpand.Persistent.Base.ModelArtifact {
    public interface IControllerStateRule : IArtifactStateRule {
        [Category("Data")]
        [Required(typeof(ControllerStateRuleControllerTypeRequiredCalculator))]
        [DataSourceProperty("Controllers")]
        [TypeConverter(typeof(StringToTypeConverter))]
        Type ControllerType { get; set; }

        [Category("Behavior")]
        [ModelPersistentName("State")]
        ControllerState ControllerState { get; set; }
    }

    public class ControllerStateRuleControllerTypeRequiredCalculator:IModelIsRequired{
        public bool IsRequired(IModelNode node, string propertyName){
            return string.IsNullOrEmpty(((IControllerStateRule) node).Module);
        }
    }
}