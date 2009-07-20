using System;
using DevExpress.ExpressApp;
using eXpand.ExpressApp.ModelArtifactState.Interfaces;

namespace eXpand.ExpressApp.ModelArtifactState.Attributes
{
    public class ControllerStateRuleAttribute : StateModelArtifactRuleAttribute,IControllerState
    {
        public ControllerStateRuleAttribute(Type ControllerType, Nesting TargetViewNesting, string NormalCriteria,
                                                 string EmptyCriteria, ViewType viewType,string module)
            : base(TargetViewNesting, NormalCriteria, EmptyCriteria, viewType,module)
        {
            this.ControllerType = ControllerType;
        }


        public bool ApplyToDerivedController { get; set; }

        string IControllerState.ControllerType
        {
            get { return ControllerType.FullName; }
            set { ControllerType = Type.GetType(value); }
        }

        /// <summary>
        /// Type of controller to activate or not
        /// </summary>
        public Type ControllerType { get; set; }

    }
}