using DevExpress.ExpressApp;
using DevExpress.ExpressApp.StateMachine;
using DevExpress.ExpressApp.StateMachine.Xpo;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;

namespace Xpand.ExpressApp.StateMachine.Controllers {
    public class CriteriaActionStateController : ViewController<ObjectView> {
        private const string HideIfCriteriaDoNotFit = "HideIfCriteriaDoNotFit";
        ChangeStateActionController _changeStateActionController;

        protected override void OnActivated() {
            base.OnActivated();
            _changeStateActionController = Frame.GetController<ChangeStateActionController>();
            _changeStateActionController.RequestActiveState += ChangeStateActionControllerOnRequestActiveStateAction;
        }

        public override void CustomizeTypesInfo(DevExpress.ExpressApp.DC.ITypesInfo typesInfo) {
            base.CustomizeTypesInfo(typesInfo);
            var typeInfo = typesInfo.FindTypeInfo(typeof(XpoState));
            if (typeInfo != null && typeInfo.FindMember(HideIfCriteriaDoNotFit) == null) {
                typeInfo.CreateMember(HideIfCriteriaDoNotFit, typeof(bool));
            }
        }

        protected override void OnDeactivated() {
            base.OnDeactivated();
            _changeStateActionController.RequestActiveState -= ChangeStateActionControllerOnRequestActiveStateAction;
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

            if (hideIfCriteriaDoNotFit.HasValue && hideIfCriteriaDoNotFit.Value){
                var stateMachineLogic = new StateMachineLogic(ObjectSpace);
                RuleSetValidationResult ruleSetValidationResult = RuleSetValidationResult(iTransition, stateMachineLogic);
                return ruleSetValidationResult.State != ValidationState.Invalid;
            }
            return true;
        }

        RuleSetValidationResult RuleSetValidationResult(ITransition iTransition, StateMachineLogic stateMachineLogic) {
            return stateMachineLogic.ValidateTransition(iTransition.TargetState, View.CurrentObject);
        }
    }
}