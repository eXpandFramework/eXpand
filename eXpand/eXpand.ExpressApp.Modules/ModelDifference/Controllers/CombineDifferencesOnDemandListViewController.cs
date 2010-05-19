using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using eXpand.ExpressApp.ModelDifference.DataStore.Queries;

namespace eXpand.ExpressApp.ModelDifference.Controllers{
    public partial class CombineDifferencesOnDemandListViewController : ViewController<ListView>{

        public CombineDifferencesOnDemandListViewController(){
            InitializeComponent();
            RegisterActions(components);
            TargetObjectType = typeof (ModelDifferenceObject);
            combineSimpleAction.SelectionDependencyType = SelectionDependencyType.RequireSingleObject;
            combineSimpleAction.ConfirmationMessage =
                "Do you want to combine the selected models with the active application model?";
        }

        public SimpleAction CombineSimpleAction{
            get { return combineSimpleAction; }
        }

        private void combineSimpleAction_Execute(object sender, SimpleActionExecuteEventArgs e){
            List<ModelDifferenceObject> selectedModelAspectObjects =
                e.SelectedObjects.Cast<ModelDifferenceObject>().ToList();
            CheckObjectCompatibility(selectedModelAspectObjects);
            CombineAndSave(selectedModelAspectObjects);
        }

        internal void CheckObjectCompatibility(List<ModelDifferenceObject> selectedObjects){
            if (selectedObjects.GroupBy(o => o.PersistentApplication.UniqueName).Select(grouping => grouping.Key).Count() > 1)
                throw new UserFriendlyException(
                    new Exception(
                        "Mixing applications is not supported. Select objects with the same application"));
        }

        public ModelDifferenceObject GetActiveApplicationModelDifference(
            List<ModelDifferenceObject> selectedModelAspectObjects){
            ModelDifferenceObject selectedModelDifferenceObject = selectedModelAspectObjects[0];
            return new QueryModelDifferenceObject(ObjectSpace.Session).GetActiveModelDifference(selectedModelDifferenceObject.PersistentApplication.UniqueName);
        }

        public void CombineAndSave(List<ModelDifferenceObject> selectedModelAspectObjects){
            ModelDifferenceObject activeApplicationModelDifferenceObject =
                GetActiveApplicationModelDifference(selectedModelAspectObjects);

            foreach (ModelDifferenceObject selectedObject in selectedModelAspectObjects)
                activeApplicationModelDifferenceObject.Model.AddLayer(selectedObject.Model);

            ObjectSpace.CommitChanges();
        }
    }
}