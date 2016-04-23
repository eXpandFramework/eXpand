using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.Persistent.Base;

namespace Xpand.Persistent.Base.General.Controllers{
    public class EasyTestController:WindowController{
        private const string LoadModel = "LoadModel";
        private const string MergeModel = "MergeModel";
        private readonly ParametrizedAction _parametrizedAction;
        private readonly SingleChoiceAction _easyTestAction;

        public EasyTestController(){
            _parametrizedAction = new ParametrizedAction(this,"Parameter",PredefinedCategory.View, typeof(string));
            _easyTestAction = new SingleChoiceAction(this,"EasyTestAction",PredefinedCategory.View) {Caption = "EasyTestAction" };
            _easyTestAction.Items.Add(new ChoiceActionItem(LoadModel, null));
            _easyTestAction.Items.Add(new ChoiceActionItem(MergeModel, null));
            _easyTestAction.ItemType=SingleChoiceActionItemType.ItemIsOperation;
            _easyTestAction.Execute+=SingleChoiceActionOnExecute;
        }

        public SingleChoiceAction EasyTestAction{
            get { return _easyTestAction; }
        }

        public ParametrizedAction ParametrizedAction{
            get { return _parametrizedAction; }
        }

        private void SingleChoiceActionOnExecute(object sender, SingleChoiceActionExecuteEventArgs e){
            if (e.SelectedChoiceActionItem.Caption == LoadModel|| e.SelectedChoiceActionItem.Caption == MergeModel) {
                var modelApplicationBase = ((ModelApplicationBase)Application.Model);
                var creator = modelApplicationBase.CreatorInstance;
                var modelApplication = creator.CreateModelApplication();
                modelApplication.Id = _parametrizedAction.Value.ToString();
                var fileNameTemplate = _parametrizedAction.Value.ToString();
                var fileModelStore = new FileModelStore(XpandModuleBase.BinDirectory, fileNameTemplate);
                if (e.SelectedChoiceActionItem.Caption == MergeModel)
                    fileModelStore.Load(((ModelApplicationBase)Application.Model).LastLayer);
                else{
                    fileModelStore.Load(modelApplication);
                    modelApplicationBase.AddLayerBeforeLast(modelApplication);                    
                }

            }
        }
    }
}
