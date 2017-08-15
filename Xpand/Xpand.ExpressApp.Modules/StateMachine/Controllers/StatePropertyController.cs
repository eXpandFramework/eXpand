using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.StateMachine;
using DevExpress.Xpo;
using Fasterflect;
using Xpand.Persistent.Base.General;
using Xpand.Utils.Linq;

namespace Xpand.ExpressApp.StateMachine.Controllers {
    public class StatePropertyController : DisableStatePropertyController{
        public event EventHandler<StatePropertyEventArgs> CustomStatePropertyIsEnabled;
        public event EventHandler<StatePropertyFilterEditorItemsEventArgs> CustomFilterEditorItems; 
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
        
        IEnumerable<IStateMachine> GetEnabledStateMachines(){
            return  GetStateMachines().Where(IsStatePropertyEnabled);
        }

        private  bool IsStatePropertyEnabled(IStateMachine machine){
            var baseObject = machine as XPBaseObject;
            if (baseObject!=null){
                var eventArgs = new StatePropertyEventArgs(machine);
                OnCustomStatePropertyIsEnabled(eventArgs);
                return eventArgs.Handled ? eventArgs.Enable
                    : true.Equals(baseObject.GetMemberValue(XpandStateMachineModule.EnableFilteredProperty));
            }
            return false;
        }

        protected override void OnDeactivated() {
            base.OnDeactivated();
            if (AppearanceController != null)
                AppearanceController.AppearanceApplied -= AppearanceController_AppearanceApplied;
            if (_stateMachineController != null) _stateMachineController.TransitionExecuted -= OnTransitionExecuted;
            ObjectSpace.ObjectChanged -= ObjectSpaceOnObjectChanged;
        }

        protected override void OnViewControllersActivated(){
            base.OnViewControllersActivated();
            Frame.GetController<StateMachineController>(controller => {
                _stateMachineController=controller;
                var enabledStateMachines = GetEnabledStateMachines().ToArray();
                if (enabledStateMachines.Any()) {
                    AppearanceController.AppearanceApplied += AppearanceController_AppearanceApplied;
                    ObjectSpace.ObjectChanged += ObjectSpaceOnObjectChanged;
                    _stateMachineController.TransitionExecuted += OnTransitionExecuted;
                    var stateProperties = enabledStateMachines.Select(machine => machine.StatePropertyName);
                    _propertyEditors = View.GetItems<PropertyEditor>()
                        .Where(editor => stateProperties.Contains(editor.PropertyName)).ToArray();
                    foreach (var item in _propertyEditors) {
                        item.ControlCreated += ItemOnControlCreated;
                    }
                }
            });
            
        }

        void ObjectSpaceOnObjectChanged(object sender, ObjectChangedEventArgs objectChangedEventArgs) {
           if (IsValidTransition(objectChangedEventArgs)) {
                var stateMachine = GetEnabledStateMachines().First(machine 
                    => machine.StatePropertyName == objectChangedEventArgs.PropertyName);
                try {
                    _changingValue = true;
                    var findCurrentState = stateMachine.FindCurrentState(View.CurrentObject);
                    stateMachine.ExecuteTransition(View.CurrentObject,findCurrentState);
                    _changingValue = false;
                    FilterEditors(stateMachine);
                }
                catch {
                    _changingValue = true;
                    objectChangedEventArgs.MemberInfo.SetValue(View.CurrentObject,objectChangedEventArgs.OldValue);
                    _changingValue = false;
                    throw;
                }
            }
        }

        bool IsValidTransition(ObjectChangedEventArgs objectChangedEventArgs) {
            return objectChangedEventArgs.NewValue!=null&&!_changingValue&&_propertyEditors.Select(editor 
                => editor.PropertyName).Contains(objectChangedEventArgs.PropertyName);
        }

        void OnTransitionExecuted(object sender, ExecuteTransitionEventArgs executeTransitionEventArgs) {
            var stateMachines = GetEnabledStateMachines().Where(machine 
                => machine == executeTransitionEventArgs.Transition.TargetState.StateMachine);
            foreach (var stateMachine in stateMachines) {
                FilterEditors(stateMachine);
            }
        }

        void FilterEditors(IStateMachine stateMachine) {
            var propertyEditor =View.GetItems<PropertyEditor>().Where(editor 
                => editor.PropertyName == stateMachine.StatePropertyName);
            foreach (var editor in propertyEditor){
                FilterEditorItems(stateMachine, editor);    
            }
        }

        private bool IsAvailable(ChoiceActionItem item){
            return item.Active && item.Enabled;
        }

        private ChoiceActionItem GetStateMachineChoiceActionItem(IStateMachine stateMachine){
            return _stateMachineController.ChangeStateAction.Items.GetItems<ChoiceActionItem>(item => item.Items).FirstOrDefault(
                item => IsAvailable(item)&&IsValid(stateMachine,item));
        }

        private bool IsValid(IStateMachine stateMachine, ChoiceActionItem item){
            return _stateMachineController.ChangeStateActionItemsMode==ChangeStateActionItemsMode.GroupByStateMachine? item.Data == stateMachine
                : ((ITransition) item.Data).TargetState.StateMachine == stateMachine;
        }

        public IEnumerable<object> GetMarkers(IStateMachine stateMachine){
            if (_stateMachineController.ChangeStateActionItemsMode == ChangeStateActionItemsMode.GroupByStateMachine) {
                var choiceActionItem = GetStateMachineChoiceActionItem(stateMachine);
                if (choiceActionItem != null){
                    var markers =choiceActionItem.Items.Select(actionItem => ((ITransition) actionItem.Data).TargetState.Marker);
                    return markers.Concat(choiceActionItem.Items.Where(IsAvailable).Select(actionItem
                        => ((ITransition) actionItem.Data).GetSourceState().Marker));
                }
                return new[] { stateMachine.FindCurrentState(View.CurrentObject).Marker };
            }
            return _stateMachineController.ChangeStateAction.Items.Where(IsAvailable).Select(item =>
                ((ITransition) item.Data).TargetState.Marker).Concat(new[] { stateMachine.FindCurrentState(View.CurrentObject).Marker });
        }

        void ItemOnControlCreated(object sender, EventArgs eventArgs) {
            var propertyEditor = ((PropertyEditor) sender);
            propertyEditor.ControlCreated-=ItemOnControlCreated;
            var stateMachines = GetEnabledStateMachines().Where(machine => machine.StatePropertyName == propertyEditor.PropertyName);
            foreach (var stateMachine in stateMachines){
                FilterEditorItems(stateMachine, propertyEditor);    
            }
        }

        private void FilterEditorItems(IStateMachine stateMachine, PropertyEditor propertyEditor){
            var args = new StatePropertyFilterEditorItemsEventArgs(stateMachine, propertyEditor);
            OnCustomFilterEditorItems(args);
            if (!args.Handled){
                var markers = GetMarkers(stateMachine);
                var propertyFilter = StatePropertyFilter.Create(propertyEditor);
                propertyFilter.Filter(stateMachine, markers);
            }
        }

        protected virtual void OnCustomStatePropertyIsEnabled(StatePropertyEventArgs e){
            var handler = CustomStatePropertyIsEnabled;
            handler?.Invoke(this, e);
        }

        protected virtual void OnCustomFilterEditorItems(StatePropertyFilterEditorItemsEventArgs e){
            var handler = CustomFilterEditorItems;
            handler?.Invoke(this, e);
        }
    }

    public class StatePropertyFilterEditorItemsEventArgs : StatePropertyEventArgsBase {
        public StatePropertyFilterEditorItemsEventArgs(IStateMachine stateMachine, PropertyEditor propertyEditor) : base(stateMachine){
            PropertyEditor = propertyEditor;
        }

        public PropertyEditor PropertyEditor{ get; }
    }

    public abstract class StatePropertyEventArgsBase : HandledEventArgs{
        protected StatePropertyEventArgsBase(IStateMachine stateMachine){
            StateMachine = stateMachine;
        }
        public IStateMachine StateMachine{ get; }
    }

    public class StatePropertyEventArgs : StatePropertyEventArgsBase{
        public StatePropertyEventArgs(IStateMachine stateMachine) : base(stateMachine){
        }

        public bool Enable { get; set; }
    }

    class EnumStatePropertyFilter : StatePropertyFilter {
        public EnumStatePropertyFilter(PropertyEditor propertyEditor) : base(propertyEditor){
            Init();
        }

        private object EditorProperties(){
            var objectInstance = PropertyEditor.Control;
            var baseType = FindBaseType(objectInstance, "DevExpress.XtraEditors.BaseEdit");
            if (baseType != null) return baseType.GetProperty("Properties")?.GetValue(objectInstance, null);
            throw new NotImplementedException();
        }

        private Type FindBaseType(object objectInstance, string fullName){
            var baseType = objectInstance.GetType();
            while (baseType != null && baseType.FullName != fullName){
                baseType = baseType.BaseType;
            }
            return baseType;
        }

        public void Init(){
            if (PropertyEditor.Model.Application.GetPlatform()==Platform.Win){
                var editorItems = GetEditorItems();
                editorItems.Clear();
                var editorProperties = EditorProperties();
                editorProperties.CallMethod("Init", PropertyEditor.MemberInfo.MemberType);
            }
        }

        public override void Filter(IStateMachine stateMachine, IEnumerable<object> markers){
            if (!stateMachine.CanExecuteAllTransitions()){
                var objects = markers as object[] ?? markers.ToArray();
                var comboBoxItemCollection = GetEditorItems();
                for (int index = comboBoxItemCollection.Count - 1; index >= 0; index--) {
                    var item = comboBoxItemCollection[index];
                    var enumerable = markers as object[] ?? objects.ToArray();
                    if (!enumerable.Contains(item.GetPropertyValue("Value")))
                        comboBoxItemCollection.RemoveAt(index);
                }
            }
        }

        private IList GetEditorItems(){
            var platform = PropertyEditor.Model.Application.GetPlatform();
            if (platform==Platform.Win) {
                var value = EditorProperties();
                var type = FindBaseType(value, "DevExpress.XtraEditors.Repository.RepositoryItemComboBox");
                return ((IList)type.GetProperty("Items")?.GetValue(value, null));
            }
            if (platform==Platform.Web) {
                var type = XafTypesInfo.Instance.FindTypeInfo("DevExpress.ExpressApp.Web.Editors.WebPropertyEditor").Type;
                var propertyInfo = type.Property("Editor");
                var delegateForGetPropertyValue = propertyInfo.DelegateForGetPropertyValue();
                var value = delegateForGetPropertyValue(PropertyEditor);
                return (IList) value.GetPropertyValue("Items");
            }
            return new List<object>();
        }
    }

    class LookupListViewStatePropertyFilter : StatePropertyFilter {
        private IStateMachine _stateMachine;
        private IEnumerable<object> _markers;
        private readonly XafApplication _application;
        private object _currentPropertyValue;

        public LookupListViewStatePropertyFilter(PropertyEditor propertyEditor):base(propertyEditor){
            _application = ApplicationHelper.Instance.Application;
        }

        public override void Filter(IStateMachine stateMachine, IEnumerable<object> markers){
            _stateMachine = stateMachine;
            _markers = markers;
            PropertyEditor.ControlValueChanged+=PropertyEditor_ControlValueChanged;
            ((ISupportViewShowing) PropertyEditor).ViewShowingNotification -=OnViewShowingNotification;
            ((ISupportViewShowing) PropertyEditor).ViewShowingNotification +=OnViewShowingNotification;
        }

        private void PropertyEditor_ControlValueChanged(object sender, EventArgs e){
            
        }

        private void OnViewShowingNotification(object sender, EventArgs eventArgs){
            if (PropertyEditor.PropertyValue!=_currentPropertyValue){
                _currentPropertyValue = PropertyEditor.PropertyValue;
                _application.ListViewCreated += ApplicationOnListViewCreated;
            }
        }

        private void ApplicationOnListViewCreated(object sender, ListViewCreatedEventArgs e){
            _application.ListViewCreated -= ApplicationOnListViewCreated;
            ApplyCriteria(e.ListView.CollectionSource);            
        }

        private void ApplyCriteria(CollectionSourceBase collectionSourceBase){
            var propertyName = GetStatePropertyDeclatedTypeDefaultProperty(_stateMachine);
            var values = _markers.Select(o => o.ToString());
            collectionSourceBase.Criteria["EnableStateProperty"] = new InOperator(propertyName, values);
        }

        private string GetStatePropertyDeclatedTypeDefaultProperty(IStateMachine stateMachine){
            var typeInfo = XafTypesInfo.Instance.FindTypeInfo(stateMachine.TargetObjectType);
            var defaultMember = typeInfo.FindMember(stateMachine.StatePropertyName).MemberTypeInfo.DeclaredDefaultMember;
            if (defaultMember != null) return defaultMember.Name;
            throw new NullReferenceException();
        }
    }

    internal abstract class StatePropertyFilter {
        protected StatePropertyFilter(PropertyEditor propertyEditor){
            PropertyEditor = propertyEditor;
        }

        public PropertyEditor PropertyEditor{ get; }

        public static StatePropertyFilter Create(PropertyEditor propertyEditor){
            return propertyEditor.MemberInfo.MemberTypeInfo.IsPersistent
                ? (StatePropertyFilter) new LookupListViewStatePropertyFilter(propertyEditor)
                : new EnumStatePropertyFilter(propertyEditor);
        }

        public abstract void Filter(IStateMachine stateMachine, IEnumerable<object> markers);

    }

}