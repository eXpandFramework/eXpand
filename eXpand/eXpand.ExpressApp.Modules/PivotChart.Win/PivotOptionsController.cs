using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.Base;
using DevExpress.Xpo.Metadata;
using DevExpress.XtraPivotGrid;
using eXpand.ExpressApp.PivotChart.Win.Core;
using System.Linq;

namespace eXpand.ExpressApp.PivotChart.Win
{
    public class PivotOptionsController : PivotChart.PivotOptionsController
    {
        public PivotOptionsController()
        {
            var singleChoiceAction = new SingleChoiceAction {Id = "PivotSettings", Caption = "PivotSettings",ItemType = SingleChoiceActionItemType.ItemIsOperation};
            AddItems(singleChoiceAction);

            singleChoiceAction.Execute+=SingleChoiceActionOnExecute; 
            Actions.Add(singleChoiceAction);
            TargetObjectType = typeof (IAnalysisInfo);
        }

        void SingleChoiceActionOnExecute(object sender, SingleChoiceActionExecuteEventArgs singleChoiceActionExecuteEventArgs) {
            var objectSpace = ObjectSpaceInMemory.CreateNew();
            var type = (Type) singleChoiceActionExecuteEventArgs.SelectedChoiceActionItem.Data;
            var persistentType = PivotGridOptionMapper.Instance[type];
            var pivotOption = objectSpace.CreateObject(persistentType);
            var classInfo = ObjectSpace.Session.GetClassInfo(persistentType);
            
            synchonize(pivotOption, type, classInfo);
            var showViewParameters = singleChoiceActionExecuteEventArgs.ShowViewParameters;
            showViewParameters.CreatedView = Application.CreateDetailView(objectSpace, pivotOption, true);
            showViewParameters.TargetWindow = TargetWindow.NewModalWindow;
            var dialogController = new DialogController();
            dialogController.AcceptAction.Execute +=(o, args) =>synchonize(classInfo, type, args.CurrentObject);
            showViewParameters.Controllers.Add(dialogController);            
        }

        void synchonize(XPClassInfo classInfo, Type optionType, object currentObject) {
            var gridOptionInstances = GetGridOptionInstance(optionType);
            foreach (var gridOptionInstance in gridOptionInstances) {
                var propertyInfos = gridOptionInstance.GetType().GetProperties().Where(propertyInfo => propertyInfo.GetSetMethod() != null);
                foreach (var propertyInfo in propertyInfos){
                    var value = classInfo.GetMember(propertyInfo.Name).GetValue(currentObject);
                    propertyInfo.SetValue(gridOptionInstance, value, null);
                }
            }
        }

        void synchonize(object persistentPivotOption, Type type, XPClassInfo classInfo) {
            var gridOptionInstances = GetGridOptionInstance(type);
            foreach (var gridOptionInstance in gridOptionInstances) {
                var propertyInfos = gridOptionInstance.GetType().GetProperties().Where(propertyInfo => propertyInfo.GetSetMethod() != null);
                foreach (var propertyInfo in propertyInfos){
                    classInfo.GetMember(propertyInfo.Name).SetValue(persistentPivotOption, propertyInfo.GetValue(gridOptionInstance, null));
                }
            }
        }

        List<object> GetGridOptionInstance(Type type) {
            return AnalysisEditors.Select(analysisEditor => ((AnalysisControlWin) analysisEditor.Control).PivotGrid).Select
                (pivotGridControl =>
                 typeof (PivotGridControl).GetProperties().Where(propertyInfo => propertyInfo.PropertyType == type).
                     Select(info1 => info1.GetValue(pivotGridControl, null)).Single()).ToList();
        }

        void AddItems(SingleChoiceAction singleChoiceAction) {
            foreach (var keyValuePair in PivotGridOptionMapper.Instance.Dictionary){
                singleChoiceAction.Items.Add(new ChoiceActionItem(keyValuePair.Key.Name, keyValuePair.Key));
            }
        }




    }
}


