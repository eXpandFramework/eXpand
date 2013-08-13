using System;
using System.ComponentModel;
using DevExpress.ExpressApp.Model.Core;
using Xpand.ExpressApp.ModelArtifactState.ArtifactState.Logic;

namespace Xpand.ExpressApp.ModelArtifactState.ControllerState.Logic{
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
