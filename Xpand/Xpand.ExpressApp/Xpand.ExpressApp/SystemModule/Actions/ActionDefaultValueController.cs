using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;

namespace Xpand.ExpressApp.SystemModule.Actions {
    [ModelAbstractClass]
    public interface IModelActionDefaultValue : IModelAction {
        IActionDefaultValue DefaultValue { get; }
    }

    public interface IActionDefaultValue : IModelNode {
        string DefaultValue { get; set; }
        int? DefaultIndex { get; set; }
        [DefaultValue(true)]
        bool Synchronize { get; set; }
    }

    public class ActionDefaultValueController : ViewController, IModelExtender {

        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            foreach (var action in GetActions()) {
                var actionDefaultValue = ((IModelActionDefaultValue)Application.Model.ActionDesign.Actions[action.Id]).DefaultValue;
                var parametrizedAction = action as ParametrizedAction;
                if (parametrizedAction != null && !string.IsNullOrEmpty(actionDefaultValue.DefaultValue)) {
                    parametrizedAction.Value = ReflectionHelper.Convert(actionDefaultValue.DefaultValue, parametrizedAction.ValueType);
                    parametrizedAction.DoExecute(parametrizedAction.Value);
                }
                var singleChoiceAction = action as SingleChoiceAction;
                if (singleChoiceAction != null && actionDefaultValue.DefaultIndex.HasValue) {
                    singleChoiceAction.SelectedIndex = actionDefaultValue.DefaultIndex.Value;
                    singleChoiceAction.DoExecute(singleChoiceAction.SelectedItem);
                }
            }
        }

        IEnumerable<ActionBase> GetActions() {
            return Frame.Controllers.Cast<Controller>().SelectMany(controller => controller.Actions).Where(@base => @base.Active);
        }

        protected override void OnDeactivated() {
            base.OnDeactivated();
            foreach (var action in GetActions()) {
                var actionDefaultValue = ((IModelActionDefaultValue)Application.Model.ActionDesign.Actions[action.Id]).DefaultValue;
                if (actionDefaultValue.Synchronize) {
                    var parametrizedAction = action as ParametrizedAction;
                    if (parametrizedAction != null) {
                        actionDefaultValue.DefaultValue = parametrizedAction.Value == null ? null : parametrizedAction.Value.ToString();
                    }
                    var singleChoiceAction = action as SingleChoiceAction;
                    if (singleChoiceAction != null) {
                        if (singleChoiceAction.SelectedIndex == -1)
                            actionDefaultValue.DefaultIndex = null;
                        else
                            actionDefaultValue.DefaultIndex = singleChoiceAction.SelectedIndex;
                    }
                }
            }
        }
        #region Implementation of IModelExtender
        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelAction, IModelActionDefaultValue>();
        }
        #endregion
    }
}
