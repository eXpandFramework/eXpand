using System;

namespace Xpand.Persistent.Base.ModelArtifact {
    public sealed class ControllerStateRuleAttribute : ArtifactStateRuleAttribute, IContextControllerStateRule {
        public ControllerStateRuleAttribute(string id, Type controllerType, string normalCriteria, string emptyCriteria, ControllerState state) : base(id, normalCriteria, emptyCriteria) {
            ControllerType = controllerType;
            ControllerState = state;
        }
        #region IControllerStateRule Members
        public Type ControllerType { get; set; }
        public ControllerState ControllerState { get; set; }

        #endregion
    }
}