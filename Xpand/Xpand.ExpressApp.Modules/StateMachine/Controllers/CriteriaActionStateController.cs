using DevExpress.ExpressApp;
using DevExpress.ExpressApp.StateMachine;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.StateMachine.Controllers {
    public class CriteriaActionStateController : ViewController<ObjectView> {
        private const string HideIfCriteriaDoNotFit = "HideIfCriteriaDoNotFit";
        private StateMachineLogic _stateMachineLogic;

        protected override void OnActivated() {
            base.OnActivated();
            _stateMachineLogic = new StateMachineLogic(ObjectSpace);
            Frame.GetController<ChangeStateActionController>(controller => controller.RequestActiveState += ChangeStateActionControllerOnRequestActiveStateAction);
        }

        public override void CustomizeTypesInfo(DevExpress.ExpressApp.DC.ITypesInfo typesInfo) {
            base.CustomizeTypesInfo(typesInfo);
            var typeInfo = typesInfo.GetStateTypeInfo();
            if (typeInfo != null && typeInfo.FindMember(HideIfCriteriaDoNotFit) == null) {
                typeInfo.CreateMember(HideIfCriteriaDoNotFit, typeof(bool));
            }
        }

        protected override void OnDeactivated() {
            base.OnDeactivated();
            Frame.GetController<ChangeStateActionController>(controller => controller.RequestActiveState -= ChangeStateActionControllerOnRequestActiveStateAction);
        }

        void ChangeStateActionControllerOnRequestActiveStateAction(object sender, ChoiceActionItemArgs choiceActionItemArgs) {
            var key = typeof(CriteriaActionStateController).Name;
            choiceActionItemArgs.Active[key] = IsActive(choiceActionItemArgs.Transition);
        }

        private bool IsActive(ITransition iTransition){
            bool? hideIfCriteriaDoNotFit = null;
            var targetState = iTransition.TargetState as XPBaseObject;
            if (targetState != null){
                hideIfCriteriaDoNotFit =targetState.GetMemberValue(HideIfCriteriaDoNotFit) as bool?;
            }

            if (hideIfCriteriaDoNotFit.HasValue && hideIfCriteriaDoNotFit.Value && View.CurrentObject != null) {
                var ruleSetValidationResult = _stateMachineLogic.ValidateTransition(iTransition.TargetState, View.CurrentObject);
                return ruleSetValidationResult.State != ValidationState.Invalid;
            }
            return true;
        }
    }
}