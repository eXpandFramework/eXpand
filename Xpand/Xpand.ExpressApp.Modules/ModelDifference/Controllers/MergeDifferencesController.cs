using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.Base;
using Xpand.ExpressApp.ModelDifference.Core;
using Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.ModelDifference.Controllers {
    public class MergeDifferencesController : ViewController<ListView> {

        public MergeDifferencesController() {
            var combineAction = new SimpleAction(this, "MergeModels", PredefinedCategory.ObjectsCreation);
            combineAction.Execute += combineSimpleAction_Execute;
            TargetObjectType = typeof(ModelDifferenceObject);
        }

        private void combineSimpleAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
            var modelDifferenceObjects = e.SelectedObjects.OfType<ModelDifferenceObject>();
            CheckIfMixingApplications(modelDifferenceObjects);
            e.ShowViewParameters.CreatedView = Application.CreateListView(Application.CreateObjectSpace(typeof(ModelDifferenceObject)), typeof(ModelDifferenceObject), true);
            e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
            var dialogController = new DialogController();
            e.ShowViewParameters.Controllers.Add(dialogController);
            dialogController.AcceptAction.Execute += AcceptActionOnExecute;
        }

        void CheckIfMixingApplications(IEnumerable<ModelDifferenceObject> modelDifferenceObjects) {
            var differenceObjects = modelDifferenceObjects;
            if (differenceObjects.GroupBy(o => o.PersistentApplication.UniqueName).Count() > 1)
                throw new NotSupportedException("Mixing applications is not supporrted");
        }

        void AcceptActionOnExecute(object sender, SimpleActionExecuteEventArgs e) {
            CombineAndSave(e.SelectedObjects.OfType<ModelDifferenceObject>().Select(o => View.ObjectSpace.GetObject(o)).ToList());
        }

        public void CombineAndSave(List<ModelDifferenceObject> selectedModelAspectObjects) {
            var selectedObjects = View.SelectedObjects.OfType<ModelDifferenceObject>().ToList();
            CheckIfMixingApplications(selectedObjects);
            foreach (var differenceObject in selectedModelAspectObjects) {
                InterfaceBuilder.SkipAssemblyCleanup = true;
                var masterModel = new ModelLoader(differenceObject.PersistentApplication.ExecutableName, XafTypesInfo.Instance).GetMasterModel(true,info => info.AssignAsInstance());
                InterfaceBuilder.SkipAssemblyCleanup = false;
                var model = differenceObject.GetModel(masterModel);
                foreach (var selectedModelAspectObject in selectedObjects) {
                    foreach (var aspectObject in selectedModelAspectObject.AspectObjects) {
                        var xml = aspectObject.Xml;
                        if (!(string.IsNullOrEmpty(xml))) {
                            var aspectName = differenceObject.GetAspectName(aspectObject);
                            if (aspectName != "" && !(model.HasAspect(aspectName))) {
                                model.AddAspect(aspectName);
                            }
                            new ModelXmlReader().ReadFromString(model, aspectName, xml);
                        }
                    }
                }
                ObjectSpace.Delete(differenceObject.AspectObjects);
                differenceObject.CreateAspects(model);
            }
            ObjectSpace.CommitChanges();
        }
    }
}