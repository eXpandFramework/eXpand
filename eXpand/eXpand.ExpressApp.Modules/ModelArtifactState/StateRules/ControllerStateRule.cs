using System;
using eXpand.ExpressApp.ModelArtifactState.Interfaces;

namespace eXpand.ExpressApp.ModelArtifactState.StateRules
{
    public class ControllerStateRule : ArtifactStateRule,IControllerStateRule
    {
        


        public ControllerStateRule(IArtifactStateRule controllerStateRule) : base(controllerStateRule)
        {
        }
        
        public new IControllerStateRule ArtifactRule
        {
            get { return base.ArtifactRule as IControllerStateRule; }
        }

        private Type controllerType;
        public Type ControllerType
        {
            get { return controllerType; }
            set
            {
                ArtifactRule.ControllerType = value.FullName;
                controllerType = value;
            }
        }

        string IControllerStateRule.ControllerType
        {
            get { return ArtifactRule.ControllerType; }
            set { ArtifactRule.ControllerType = value; }
        }
    }
}