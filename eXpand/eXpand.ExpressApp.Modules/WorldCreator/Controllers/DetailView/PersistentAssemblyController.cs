using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using eXpand.Persistent.Base.PersistentMetaData;
using eXpand.ExpressApp.WorldCreator.Core;
using System.Linq;

namespace eXpand.ExpressApp.WorldCreator.Controllers.DetailView
{
    public class PersistentAssemblyController : ViewController<DevExpress.ExpressApp.DetailView>
    {
        public PersistentAssemblyController()
        {
            var simpleAction = new SimpleAction { Caption = "Validate", ToolTip = "Validate assembly", Id = "AssemblyValidation" };
            Actions.Add(simpleAction);
            simpleAction.Execute+=SimpleActionOnExecute;
            TargetObjectType = typeof (IPersistentAssemblyInfo);
            
        }

        void SimpleActionOnExecute(object sender, SimpleActionExecuteEventArgs simpleActionExecuteEventArgs) {
            var worldCreatorModuleBase = Application.Modules.OfType<WorldCreatorModuleBase>().Single();
            ((IPersistentAssemblyInfo)simpleActionExecuteEventArgs.CurrentObject).Validate(worldCreatorModuleBase.GetPath());
        }
    }
}
