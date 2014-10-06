using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.Persistent.Base;

namespace Xpand.Persistent.Base.General.Controllers{
    public class EasyTestController:WindowController{
        private readonly ParametrizedAction _parametrizedAction;

        public EasyTestController(){
            _parametrizedAction = new ParametrizedAction(this,"Parameter",PredefinedCategory.View, typeof(string));
            var singleChoiceAction = new SingleChoiceAction(this,"Action",PredefinedCategory.View);
            singleChoiceAction.Items.Add(new ChoiceActionItem("LoadModel", null));
            singleChoiceAction.Execute+=SingleChoiceActionOnExecute;
        }

        private void SingleChoiceActionOnExecute(object sender, SingleChoiceActionExecuteEventArgs e){
            if (e.SelectedChoiceActionItem.Caption == "LoadModel"){
                var modelApplicationBase = ((ModelApplicationBase)Application.Model);
                var creator = modelApplicationBase.CreatorInstance;
                var modelApplication = creator.CreateModelApplication();
                modelApplication.Id = _parametrizedAction.Value.ToString();
                var fileNameTemplate = _parametrizedAction.Value.ToString();
                var fileModelStore = new FileModelStore(XpandModuleBase.BinDirectory, fileNameTemplate);
                fileModelStore.Load(modelApplication);
                modelApplicationBase.AddLayerBeforeLast(modelApplication);
            }
        }
    }
}