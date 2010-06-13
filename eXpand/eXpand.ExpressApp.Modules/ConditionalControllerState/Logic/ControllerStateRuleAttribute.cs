using System;
using eXpand.ExpressApp.ArtifactState.Logic;

namespace eXpand.ExpressApp.ConditionalControllerState.Logic {
    public class ControllerStateRuleAttribute : ArtifactStateRuleAttribute, IControllerStateRule {
        public ControllerStateRuleAttribute(string id, Type controllerType, string normalCriteria, string emptyCriteria, ControllerState state) : base(id, normalCriteria, emptyCriteria) {
            ControllerType = controllerType;
            State = state;
        }
        #region IControllerStateRule Members
        public Type ControllerType { get; set; }
        public ControllerState State { get; set; }


        #endregion
    }
}