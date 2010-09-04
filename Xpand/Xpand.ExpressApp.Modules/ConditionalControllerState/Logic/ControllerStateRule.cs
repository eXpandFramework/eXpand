using System;
using System.ComponentModel;
using DevExpress.ExpressApp.Model.Core;
using Xpand.ExpressApp.ArtifactState.Logic;

namespace Xpand.ExpressApp.ConditionalControllerState.Logic{
    public class ControllerStateRule : ArtifactStateRule,IControllerStateRule{
        

        public ControllerStateRule(IControllerStateRule controllerStateRule): base(controllerStateRule){
            ControllerType = controllerStateRule.ControllerType;    
            ControllerState=controllerStateRule.ControllerState;    
        }

        [Category("Data"), TypeConverter(typeof(StringToTypeConverter))]
        public Type ControllerType { get; set; }

        [Category("Behavior")]
        public ControllerState ControllerState { get; set; }
    }
}
