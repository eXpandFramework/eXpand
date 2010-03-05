using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.Base;
using DevExpress.Xpo.Metadata;
using eXpand.ExpressApp.PivotChart.Core;

namespace eXpand.ExpressApp.PivotChart
{
    public abstract class PivotOptionsController : AnalysisViewControllerBase
    {
        readonly SingleChoiceAction _pivotSettingsChoiceAction;
        

        public SingleChoiceAction PivotSettingsChoiceAction {
            get { return _pivotSettingsChoiceAction; }
        }

        protected PivotOptionsController()
        {
            _pivotSettingsChoiceAction = new SingleChoiceAction { Id = "PivotSettings", Caption = "PivotSettings", ItemType = SingleChoiceActionItemType.ItemIsOperation };
            _pivotSettingsChoiceAction.Execute+=PivotSettingsChoiceActionOnExecute;
            Actions.Add(_pivotSettingsChoiceAction);
            TargetObjectType = typeof(IAnalysisInfo);
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            PivotSettingsChoiceAction.Active["EditMode"] = View.ViewEditMode == ViewEditMode.Edit;
            PivotSettingsChoiceAction.Items.Clear();
            foreach (var keyValuePair in GetActionChoiceItems()){
                PivotSettingsChoiceAction.Items.Add(new ChoiceActionItem(keyValuePair.Key.Name, keyValuePair.Key));
            }

        }

        protected abstract Dictionary<Type, Type> GetActionChoiceItems();

        void PivotSettingsChoiceActionOnExecute(object sender, SingleChoiceActionExecuteEventArgs singleChoiceActionExecuteEventArgs) {
            var objectSpace = ObjectSpaceInMemory.CreateNew();
            var type = (Type)singleChoiceActionExecuteEventArgs.SelectedChoiceActionItem.Data;
            var persistentType = GetPersistentType(type);
            var pivotOption = objectSpace.CreateObject(persistentType);
            XPClassInfo classInfo = ObjectSpace.Session.GetClassInfo(persistentType);

            Synchonize(pivotOption, type, classInfo);
            var showViewParameters = singleChoiceActionExecuteEventArgs.ShowViewParameters;
            showViewParameters.CreatedView = Application.CreateDetailView(objectSpace, pivotOption, true);
            
            showViewParameters.TargetWindow = TargetWindow.NewModalWindow;
            var dialogController = new DialogController();
            dialogController.AcceptAction.Execute += (o, args) => Synchonize(classInfo, type, args.CurrentObject);
            showViewParameters.Controllers.Add(dialogController);
            ((DetailView) showViewParameters.CreatedView).ViewEditMode=ViewEditMode.Edit;

        }

        protected abstract Type GetPersistentType(Type type);

        protected virtual void Synchonize(XPClassInfo classInfo, Type optionType, object currentObject){
            var gridOptionInstances = GetGridOptionInstance(optionType);
            foreach (var gridOptionInstance in gridOptionInstances){
                var propertyInfos = gridOptionInstance.GetType().GetProperties().Where(propertyInfo => propertyInfo.GetSetMethod() != null);
                foreach (var propertyInfo in propertyInfos){
                    var value = classInfo.GetMember(propertyInfo.Name).GetValue(currentObject);
                    propertyInfo.SetValue(gridOptionInstance, value, null);
                }
            }
        }

        protected virtual void Synchonize(object persistentPivotOption, Type type, XPClassInfo classInfo){
            var gridOptionInstances = GetGridOptionInstance(type);
            foreach (var gridOptionInstance in gridOptionInstances){
                var propertyInfos = gridOptionInstance.GetType().GetProperties().Where(propertyInfo => propertyInfo.GetSetMethod() != null);
                foreach (var propertyInfo in propertyInfos){
                    classInfo.GetMember(propertyInfo.Name).SetValue(persistentPivotOption, propertyInfo.GetValue(gridOptionInstance, null));
                }
            }
        }

        protected abstract IEnumerable<object> GetGridOptionInstance(Type type);
        

    }
}
