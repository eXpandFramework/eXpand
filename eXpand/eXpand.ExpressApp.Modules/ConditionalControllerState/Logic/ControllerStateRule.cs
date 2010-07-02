using System;
using System.ComponentModel;
using DevExpress.ExpressApp.Model.Core;
using eXpand.ExpressApp.ArtifactState.Logic;

namespace eXpand.ExpressApp.ConditionalControllerState.Logic{
    public class ControllerStateRule : ArtifactStateRule,IControllerStateRule{
        readonly IControllerStateRule _conditionalLogicRule;

        public ControllerStateRule(IControllerStateRule controllerStateRule): base(controllerStateRule){
            _conditionalLogicRule = controllerStateRule;
        }
        [Category("Data")]
        [TypeConverter(typeof(StringToTypeConverter))]
        public Type ControllerType {
            get { return _conditionalLogicRule.ControllerType; }
            set { _conditionalLogicRule.ControllerType = value; }
        }
        [Category("Behavior")]
        public ControllerState ControllerState {
            get { return _conditionalLogicRule.ControllerState; }
            set { _conditionalLogicRule.ControllerState = value; }
        }

    }
}
