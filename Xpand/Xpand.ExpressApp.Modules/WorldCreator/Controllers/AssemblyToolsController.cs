using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using Xpand.ExpressApp.WorldCreator.Core;
using Xpand.Persistent.Base.PersistentMetaData;

namespace Xpand.ExpressApp.WorldCreator.Controllers {
    public class AssemblyToolsController:ViewController<XpandDetailView> {
        readonly SingleChoiceAction _singleChoiceAction;
        


        public AssemblyToolsController() {
            TargetObjectType = typeof(IPersistentAssemblyInfo);
            _singleChoiceAction = new SingleChoiceAction(this, "Tools",PredefinedCategory.ObjectsCreation);
            _singleChoiceAction.Items.Add(new ChoiceActionItem("Validate Assembly", "Validate"));
            _singleChoiceAction.Execute+=SingleChoiceActionOnExecute;
            _singleChoiceAction.ItemType=SingleChoiceActionItemType.ItemIsOperation;            
        }


        void SingleChoiceActionOnExecute(object sender, SingleChoiceActionExecuteEventArgs singleChoiceActionExecuteEventArgs) {
            if (ReferenceEquals(singleChoiceActionExecuteEventArgs.SelectedChoiceActionItem.Data, "Validate")){
                var worldCreatorModuleBase = Application.Modules.OfType<WorldCreatorModuleBase>().Single();
                ((IPersistentAssemblyInfo)singleChoiceActionExecuteEventArgs.CurrentObject).Validate(worldCreatorModuleBase.GetPath());
            }
        }

        public SingleChoiceAction SingleChoiceAction {
            get { return _singleChoiceAction; }
        }
    }
}