using System;
using eXpand.ExpressApp.ModelArtifactState.ArtifactState;
using eXpand.Persistent.Base.General;

namespace eXpand.ExpressApp.ModelArtifactState.ControllerState {
    public class ControllerStateRule : ArtifactStateRule, IControllerStateRule {
        Type controllerType;

        public ControllerStateRule(IControllerStateRule controllerStateRule) : base(controllerStateRule) {
        }

        public new IControllerStateRule ArtifactRule {
            get { return base.ArtifactRule as IControllerStateRule; }
        }

        public Type ControllerType {
            get { return controllerType; }
            set {
                ArtifactRule.ControllerType = value.FullName;
                controllerType = value;
            }
        }

        public State State {
            get { return ArtifactRule.State; }
            set { ArtifactRule.State=value; }
        }
        #region IControllerStateRule Members
        string IControllerStateRule.ControllerType {
            get { return ArtifactRule.ControllerType; }
            set { ArtifactRule.ControllerType = value; }
        }
        #endregion
    }
}