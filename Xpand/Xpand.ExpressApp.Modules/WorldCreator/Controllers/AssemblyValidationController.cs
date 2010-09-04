using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using eXpand.Persistent.Base.PersistentMetaData;
using eXpand.ExpressApp.WorldCreator.Core;
using System.Linq;

namespace eXpand.ExpressApp.WorldCreator.Controllers
{
    public class AssemblyValidationController : ViewController<DetailView>
    {
        public AssemblyValidationController()
        {
            TargetObjectType = typeof (IPersistentAssemblyInfo);   
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            var assemblyToolsController = Frame.GetController<AssemblyToolsController>();
            assemblyToolsController.SingleChoiceAction.Items.Add(new ChoiceActionItem("Validate Assembly", "Validate"));
            assemblyToolsController.ToolExecuted+=AssemblyToolsControllerOnToolExecuted;            
        }

        void AssemblyToolsControllerOnToolExecuted(object sender, SingleChoiceActionExecuteEventArgs singleChoiceActionExecuteEventArgs) {
            if (ReferenceEquals(singleChoiceActionExecuteEventArgs.SelectedChoiceActionItem.Data, "Validate")) {
                var worldCreatorModuleBase = Application.Modules.OfType<WorldCreatorModuleBase>().Single();
                ((IPersistentAssemblyInfo)singleChoiceActionExecuteEventArgs.CurrentObject).Validate(worldCreatorModuleBase.GetPath());
            }
        }

    }
}
