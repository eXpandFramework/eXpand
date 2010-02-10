using System;
using DevExpress.ExpressApp;
using eXpand.ExpressApp.ModelArtifactState.Interfaces;
using eXpand.Persistent.Base.General;

namespace eXpand.ExpressApp.ModelArtifactState.Attributes{
    public class ControllerStateRuleAttribute : ArtifactStateRuleAttribute, IControllerStateRule{
        private readonly Type controllerType;

        public ControllerStateRuleAttribute(string id, Type controllerType, Nesting targetViewNesting,
                                            string normalCriteria,
                                            string emptyCriteria, ViewType viewType, string module, State state,
                                            string viewId)
            : base(id, targetViewNesting, normalCriteria, emptyCriteria, viewType, module, state, viewId){
            this.controllerType = controllerType;
        }

        public Type ControllerType{
            get { return controllerType; }
        }
        #region IControllerStateRule Members
        public override State State{
            get{
                if (base.State == State.Disabled)
                    return State.Hidden;
                return base.State;
            }
            set { base.State = value; }
        }

        string IControllerStateRule.ControllerType{
            get { return controllerType.FullName; }
            set { throw new NotImplementedException(); }
        }
        #endregion
    }
}