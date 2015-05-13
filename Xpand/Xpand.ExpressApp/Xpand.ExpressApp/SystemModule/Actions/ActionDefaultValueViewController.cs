using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.Model.VisibilityCalculators;

namespace Xpand.ExpressApp.SystemModule.Actions {
    [ModelAbstractClass]
    public interface IModelActionDefaultValue : IModelAction {
        IActionDefaultValue DefaultValue { get; }
    }

    public interface IActionDefaultValue : IModelNode {
        [ModelBrowsable(typeof(ActionVisibilityCalculator<ParametrizedAction>))]
        string DefaultValue { get; set; }
        [ModelBrowsable(typeof(ActionVisibilityCalculator<SingleChoiceAction>))]
        int? DefaultIndex { get; set; }
        bool Synchronize { get; set; }
    }

    public class ActionDefaultValueController:WindowController,IModelExtender{
        readonly Dictionary<ActionBase,ActionDefaultValueHelper>   _defaultValueHelpers=new Dictionary<ActionBase, ActionDefaultValueHelper>();
        private IEnumerable<ActionBase> _actions;

        class ActionDefaultValueHelper {
            private readonly ActionBase _action;

            public ActionDefaultValueHelper(ActionBase action){
                _action = action;
            }

            public static void ApplyDefaultValue(ActionBase action) {
                var modelAction = action.Application.Model.ActionDesign.Actions[action.Id];
                var actionDefaultValue = ((IModelActionDefaultValue)modelAction).DefaultValue;
                if (actionDefaultValue.Synchronize) {
                    var parametrizedAction = action as ParametrizedAction;
                    if (parametrizedAction != null && !string.IsNullOrEmpty(actionDefaultValue.DefaultValue)) {
                        parametrizedAction.Value = ReflectionHelper.Convert(actionDefaultValue.DefaultValue,
                            parametrizedAction.ValueType);
                        parametrizedAction.DoExecute(parametrizedAction.Value);
                    }
                    var singleChoiceAction = action as SingleChoiceAction;
                    if (singleChoiceAction != null && actionDefaultValue.DefaultIndex.HasValue) {
                        singleChoiceAction.SelectedIndex = actionDefaultValue.DefaultIndex.Value;
                        singleChoiceAction.DoExecute(singleChoiceAction.SelectedItem);
                    }
                }
            }

            public void ResultValueChanged(object sender, BoolValueChangedEventArgs e){
                if (_action.Enabled && _action.Active) {
                    ApplyDefaultValue(_action);
                }
            }

        }
        protected override void OnFrameAssigned(){
            base.OnFrameAssigned();
            ApplyDefaultValue();
            Frame.Disposing+=FrameOnDisposing;
            _actions = Frame.Actions().ToArray();
            foreach (var action in _actions){
                var actionDefaultValueHelper = new ActionDefaultValueHelper(action);
                _defaultValueHelpers.Add(action, actionDefaultValueHelper);
                action.Enabled.ResultValueChanged += actionDefaultValueHelper.ResultValueChanged;
                action.Active.ResultValueChanged += actionDefaultValueHelper.ResultValueChanged;
            }
        }

        private void FrameOnDisposing(object sender, EventArgs eventArgs){
            Frame.Disposing-=FrameOnDisposing;
            foreach (var action in _actions) {
                var helper = _defaultValueHelpers[action];
                action.Active.ResultValueChanged -= helper.ResultValueChanged;
                action.Enabled.ResultValueChanged -= helper.ResultValueChanged;
            }
        }

        IEnumerable<ActionBase> GetActions() {
            return Frame.Actions().Where(@base =>@base.Active&&@base.Enabled&& Frame.Application.Model.ActionDesign.Actions[@base.Id] != null);
        }

        public void ApplyDefaultValue() {
            foreach (var action in GetActions()) {
                ActionDefaultValueHelper.ApplyDefaultValue(action);
            }
        }

        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders){
            extenders.Add<IModelAction,IModelActionDefaultValue>();
        }
    }
}
