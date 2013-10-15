using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.StateMachine;
using DevExpress.ExpressApp.StateMachine.Xpo;
using DevExpress.Persistent.Validation;
using Fasterflect;

namespace Xpand.ExpressApp.StateMachine.Controllers {
    public class CriteriaActionStateController:ViewController<ObjectView> {
        private const string HideIfCriteriaDoNotFit = "HideIfCriteriaDoNotFit";
        ChangeStateActionController _changeStateActionController;

        protected override void OnActivated() {
            base.OnActivated();
            _changeStateActionController = Frame.GetController<ChangeStateActionController>();
            _changeStateActionController.RequestActiveState+=ChangeStateActionControllerOnRequestActiveStateAction;
        }

        public override void CustomizeTypesInfo(DevExpress.ExpressApp.DC.ITypesInfo typesInfo) {
            base.CustomizeTypesInfo(typesInfo);
            var typeInfo = typesInfo.FindTypeInfo(typeof (XpoState));
            typeInfo.CreateMember(HideIfCriteriaDoNotFit, typeof (bool));
        }

        protected override void OnDeactivated() {
            base.OnDeactivated();
            _changeStateActionController.RequestActiveState-=ChangeStateActionControllerOnRequestActiveStateAction;
        }

        void ChangeStateActionControllerOnRequestActiveStateAction(object sender, ChoiceActionItemArgs choiceActionItemArgs) {
            var key = typeof(CriteriaActionStateController).Name;
            choiceActionItemArgs.Active[key] = IsActive(choiceActionItemArgs.Transition);
        }

        bool IsActive(XpoTransition xpoTransition) {
            var hideIfCriteriaDoNotFit = xpoTransition.TargetState.GetMemberValue(HideIfCriteriaDoNotFit) as bool?;
            if (hideIfCriteriaDoNotFit.HasValue&&hideIfCriteriaDoNotFit.Value) {
                var stateMachineLogic = new StateMachineLogic(ObjectSpace);
                var ruleSetValidationResult = RuleSetValidationResult(xpoTransition, stateMachineLogic);
                return ruleSetValidationResult.State != ValidationState.Invalid;
            }
            return true;
        }

        [Obsolete("In 13.2 the ValidateTransition is public")]
        RuleSetValidationResult RuleSetValidationResult(XpoTransition xpoTransition, StateMachineLogic stateMachineLogic) {
            return (RuleSetValidationResult) stateMachineLogic.CallMethod("ValidateTransition", new[]{xpoTransition.TargetState, View.CurrentObject});
        }
    }
}