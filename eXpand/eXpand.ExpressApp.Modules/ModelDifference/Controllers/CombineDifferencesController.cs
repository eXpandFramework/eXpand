using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.Base;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;

namespace eXpand.ExpressApp.ModelDifference.Controllers{
    public class CombineDifferencesController : ViewController<ListView>{

        public CombineDifferencesController(){
            var combineAction = new SimpleAction(this,"Combine",PredefinedCategory.ObjectsCreation);
            combineAction.Execute+=combineSimpleAction_Execute;
            TargetObjectType = typeof (ModelDifferenceObject);
        }



        private void combineSimpleAction_Execute(object sender, SimpleActionExecuteEventArgs e){
            var modelDifferenceObjects = e.SelectedObjects.OfType<ModelDifferenceObject>();
            CheckIfMixingApplications(modelDifferenceObjects);
            e.ShowViewParameters.CreatedView = Application.CreateListView(Application.CreateObjectSpace(),typeof (ModelDifferenceObject),true);
            e.ShowViewParameters.TargetWindow=TargetWindow.NewModalWindow;
            var dialogController = new DialogController();
            e.ShowViewParameters.Controllers.Add(dialogController);
            dialogController.AcceptAction.Execute+=AcceptActionOnExecute;
            
        }

        void CheckIfMixingApplications(IEnumerable<ModelDifferenceObject> modelDifferenceObjects) {
            if (modelDifferenceObjects.GroupBy(o => o.PersistentApplication.UniqueName).Count() > 1)
                throw new NotSupportedException("Mixing applications is not supporrted");
        }

        void AcceptActionOnExecute(object sender, SimpleActionExecuteEventArgs e) {
            var modelDifferenceObjects = e.SelectedObjects.Cast<ModelDifferenceObject>();
            List<ModelDifferenceObject> selectedModelAspectObjects =modelDifferenceObjects.ToList();
            CombineAndSave(selectedModelAspectObjects);
        }



        public void CombineAndSave(List<ModelDifferenceObject> selectedModelAspectObjects){
            var selectedObjects = View.SelectedObjects.OfType<ModelDifferenceObject>();
            CheckIfMixingApplications(selectedObjects);
            foreach (var differenceObject in selectedObjects) {
                foreach (var selectedModelAspectObject in selectedModelAspectObjects) {
                    var selectedModel = selectedModelAspectObject.Model;
                    for (int i = 0; i < selectedModelAspectObject.Model.AspectCount; i++) {
                        var xml = new ModelXmlWriter().WriteToString(selectedModel, i);
                        if (!(string.IsNullOrEmpty(xml))){
                            new ModelXmlReader().ReadFromString(differenceObject.Model, selectedModel.GetAspect(i), xml);
                            ObjectSpace.SetModified(differenceObject);
                        }
                    }
                }
            }
            ObjectSpace.CommitChanges();
        }
    }
}