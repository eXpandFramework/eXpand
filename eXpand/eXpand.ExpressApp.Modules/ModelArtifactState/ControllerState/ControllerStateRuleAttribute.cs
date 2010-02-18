using System;
using DevExpress.ExpressApp;
using eXpand.ExpressApp.ModelArtifactState.ArtifactState;
using eXpand.Persistent.Base.General;

namespace eXpand.ExpressApp.ModelArtifactState.ControllerState{
    public class ControllerStateRuleAttribute : ArtifactRuleAttribute, IControllerStateRule{
        private readonly Type controllerType;
        State _state;

        public ControllerStateRuleAttribute(string id, Type controllerType, Nesting targetViewNesting,
                                            string normalCriteria,
                                            string emptyCriteria, ViewType viewType, string module, State state,
                                            string viewId)
            : base(id, targetViewNesting, normalCriteria, emptyCriteria, viewType, module,  viewId){
            this.controllerType = controllerType;
            _state=state;
        }

        public Type ControllerType{
            get { return controllerType; }
        }
        #region IControllerStateRule Members
        public State State{
            get{
                if (_state == State.Disabled)
                    return State.Hidden;
                return _state;
            }
            set { _state = value; }
        }

        string IControllerStateRule.ControllerType{
            get { return controllerType.FullName; }
            set { throw new NotImplementedException(); }
        }
        #endregion
    }
}