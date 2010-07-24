using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;

namespace eXpand.ExpressApp.ModelDifference.Controllers{
    public partial class CombineDifferencesController : ViewController<ListView>{

        public CombineDifferencesController(){
            InitializeComponent();
            RegisterActions(components);
            TargetObjectType = typeof (ModelDifferenceObject);
        }

        public SimpleAction CombineSimpleAction{
            get { return combineSimpleAction; }
        }


        private void combineSimpleAction_Execute(object sender, SimpleActionExecuteEventArgs e){
            e.ShowViewParameters.CreatedView = Application.CreateListView(Application.CreateObjectSpace(),typeof (ModelDifferenceObject),true);
            e.ShowViewParameters.TargetWindow=TargetWindow.NewModalWindow;
            var dialogController = new DialogController();
            e.ShowViewParameters.Controllers.Add(dialogController);
            dialogController.AcceptAction.Execute+=AcceptActionOnExecute;
            
        }

        void AcceptActionOnExecute(object sender, SimpleActionExecuteEventArgs e) {
            List<ModelDifferenceObject> selectedModelAspectObjects =
                e.SelectedObjects.Cast<ModelDifferenceObject>().ToList();
            CombineAndSave(selectedModelAspectObjects);
        }



        public void CombineAndSave(List<ModelDifferenceObject> selectedModelAspectObjects){
            foreach (var modelDifferenceObject in View.SelectedObjects.OfType<ModelDifferenceObject>()) {
                var modelApplicationBase = (modelDifferenceObject).Model;
                modelApplicationBase = modelApplicationBase.Clone();
                foreach (var selectedModelAspectObject in selectedModelAspectObjects) {
                    var modelReader = new ModelXmlReader();
                    modelReader.ReadFromString(modelApplicationBase, Application.CurrentAspectProvider.CurrentAspect, selectedModelAspectObject.Model.Xml);
                    modelDifferenceObject.Model = modelApplicationBase;
                }
            }
            ObjectSpace.CommitChanges();
        }
    }
}