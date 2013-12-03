using System;
using System.Collections;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.StateMachine;
using System.Linq;
using DevExpress.ExpressApp.StateMachine.Xpo;
using Xpand.Persistent.Base.General;
using Fasterflect;

namespace Xpand.ExpressApp.StateMachine.Controllers {
    public class EnableStatePropertyController : DisableStatePropertyController {
        bool _changingValue;
        StateMachineController _stateMachineController;
        PropertyEditor[] _propertyEditors;

        void AppearanceController_AppearanceApplied(object sender, ApplyAppearanceEventArgs e) {
            var appearanceEnabled = e.Item as IAppearanceEnabled;
            if (appearanceEnabled != null) {
                foreach (IStateMachine stateMachine in GetEnabledStateMachines()) {
                    if (e.ItemName == stateMachine.StatePropertyName) {
                        appearanceEnabled.ResetEnabled();
                    }
                }
            }
        }
        
        IEnumerable<object> GetMarkers(IStateMachine stateMachine) {
            var choiceActionItem = GetStateMachineChoiceActionItem(stateMachine);
            if (choiceActionItem != null) {
                var markers =choiceActionItem.Items.Select(actionItem => ((XpoTransition) actionItem.Data).TargetState.Marker.Marker);
                return markers.Concat(choiceActionItem.Items.Where(IsValid).Select(actionItem 
                    => ((XpoTransition)actionItem.Data).SourceState.Marker.Marker));
            }
            var xpoState = (XpoState) stateMachine.FindCurrentState(View.CurrentObject);
            return new []{xpoState.Marker.Marker};
        }

        ChoiceActionItem GetStateMachineChoiceActionItem(IStateMachine stateMachine) {
            return _stateMachineController.ChangeStateAction.Items.FirstOrDefault(
                    item => IsValid(item) && item.Data == stateMachine);
        }

        bool IsValid(ChoiceActionItem item) {
            return item.Active && item.Enabled;
        }

        protected virtual void FilterEditor(PropertyEditor propertyEditor, IEnumerable<object> markers) {
            var comboBoxItemCollection = GetEditorItems(propertyEditor);
            for (int index = comboBoxItemCollection.Count - 1; index >= 0; index--) {
                var item = comboBoxItemCollection[index];
                if (!markers.Contains(item.GetPropertyValue("Value")))
                    comboBoxItemCollection.RemoveAt(index);
            }
        }

        IList GetEditorItems(PropertyEditor propertyEditor) {
            if (!XpandModuleBase.IsHosted) {
                var value = EditorProperties(propertyEditor);
                var type = Application.TypesInfo.FindTypeInfo("DevExpress.XtraEditors.Repository.RepositoryItemComboBox").Type;
                return ((IList)type.GetProperty("Items").GetValue(value, null));
            }
            else {
                var type = Application.TypesInfo.FindTypeInfo("DevExpress.ExpressApp.Web.Editors.WebPropertyEditor").Type;
                var propertyInfo = type.Property("Editor");
                var delegateForGetPropertyValue = propertyInfo.DelegateForGetPropertyValue();
                var value = delegateForGetPropertyValue(propertyEditor);
                return (IList) value.GetPropertyValue("Items");
            }
        }

        object EditorProperties(PropertyEditor propertyEditor) {
            var type = Application.TypesInfo.FindTypeInfo("DevExpress.XtraEditors.BaseEdit").Type;
            return type.GetProperty("Properties").GetValue(propertyEditor.Control, null);
        }

        IEnumerable<IStateMachine> GetEnabledStateMachines() {
            return GetStateMachines().OfType<XpoStateMachine>().Where(machine 
                => (bool) machine.GetMemberValue(XpandStateMachineModule.EnableFilteredPropety));
        }

        protected override void OnDeactivated() {
            base.OnDeactivated();
            AppearanceController.AppearanceApplied -= AppearanceController_AppearanceApplied;
            var enabledStateMachines = GetEnabledStateMachines();
            if (enabledStateMachines.All(machine => machine.CanExecuteTransition()))
                return;
            _stateMachineController.TransitionExecuted -= OnTransitionExecuted;
            ObjectSpace.ObjectChanged -= ObjectSpaceOnObjectChanged;
        }

        protected override void OnActivated() {
            base.OnActivated();
            AppearanceController.AppearanceApplied += AppearanceController_AppearanceApplied;
            var enabledStateMachines = GetEnabledStateMachines();
            if (enabledStateMachines.All(machine => machine.CanExecuteTransition()))
                return;
            var stateProperties = enabledStateMachines.Select(machine => machine.StatePropertyName);
            
            _stateMachineController = Frame.GetController<StateMachineController>();
            _propertyEditors = View.GetItems<PropertyEditor>().Where(editor => stateProperties.Contains(editor.PropertyName)).ToArray();
            foreach (var item in _propertyEditors) {
                item.ControlCreated+=ItemOnControlCreated;
            }
            ObjectSpace.ObjectChanged+=ObjectSpaceOnObjectChanged;
            _stateMachineController.TransitionExecuted += OnTransitionExecuted;
        }

        void ObjectSpaceOnObjectChanged(object sender, ObjectChangedEventArgs objectChangedEventArgs) {
           if (CanExecuteTransition(objectChangedEventArgs)) {
                var stateMachine = GetEnabledStateMachines().First(machine 
                    => machine.StatePropertyName == objectChangedEventArgs.PropertyName);
                try {
                    _changingValue = true;
                    var findCurrentState = stateMachine.FindCurrentState(View.CurrentObject);
                    stateMachine.ExecuteTransition(View.CurrentObject,findCurrentState);
                    _changingValue = false;
                    ExecuteTransition(stateMachine);
                }
                catch {
                    _changingValue = true;
                    objectChangedEventArgs.MemberInfo.SetValue(View.CurrentObject,objectChangedEventArgs.OldValue);
                    _changingValue = false;
                    throw;
                }
            }
        }

        bool CanExecuteTransition(ObjectChangedEventArgs objectChangedEventArgs) {
            return objectChangedEventArgs.NewValue!=null&&objectChangedEventArgs.OldValue!=null&& !_changingValue&&_propertyEditors.Select(editor 
                => editor.PropertyName).Contains(objectChangedEventArgs.PropertyName);
        }

        void OnTransitionExecuted(object sender, ExecuteTransitionEventArgs executeTransitionEventArgs) {
            var stateMachines = GetEnabledStateMachines().Where(machine 
                => machine == executeTransitionEventArgs.Transition.TargetState.StateMachine);
            foreach (var stateMachine in stateMachines) {
                ExecuteTransition(stateMachine);
            }
        }

        void ExecuteTransition(IStateMachine stateMachine) {
            var propertyEditor =View.GetItems<PropertyEditor>().First(editor 
                => editor.PropertyName == stateMachine.StatePropertyName);
            Init(propertyEditor);
            FilterEditor(propertyEditor, GetMarkers(stateMachine));
        }

        protected virtual void Init(PropertyEditor propertyEditor) {
            if (!XpandModuleBase.IsHosted) {
                var editorItems = GetEditorItems(propertyEditor);
                editorItems.Clear();
                var editorProperties = EditorProperties(propertyEditor);
                editorProperties.CallMethod("Init", propertyEditor.MemberInfo.MemberType);
            }
        }

        void ItemOnControlCreated(object sender, EventArgs eventArgs) {
            var propertyEditor = ((PropertyEditor) sender);
            propertyEditor.ControlCreated-=ItemOnControlCreated;
            var stateMachine = GetEnabledStateMachines().First(machine => machine.StatePropertyName == propertyEditor.PropertyName);
            var markers = GetMarkers(stateMachine);
            FilterEditor(propertyEditor,markers);

        }
    }
}