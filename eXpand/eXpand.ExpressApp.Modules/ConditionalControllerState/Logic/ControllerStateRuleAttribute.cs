using System;
using eXpand.ExpressApp.ArtifactState.Logic;

namespace eXpand.ExpressApp.ConditionalControllerState.Logic {
    public class ControllerStateRuleAttribute : ArtifactStateRuleAttribute, IControllerStateRule {
        public ControllerStateRuleAttribute(string id, string normalCriteria, string emptyCriteria, Type controllerType,
                                        ControllerState state) : base(id, normalCriteria, emptyCriteria) {
            ControllerType = controllerType;
            State = state;
        }
        #region IControllerStateRule Members
        public Type ControllerType { get; set; }
        public ControllerState State { get; set; }


        #endregion
    }
}