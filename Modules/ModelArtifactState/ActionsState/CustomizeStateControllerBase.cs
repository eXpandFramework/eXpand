using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using eXpand.ExpressApp.ModelArtifactState.Controllers;

namespace eXpand.ExpressApp.ModelArtifactState.ActionsState
{
    public abstract partial class CustomizeStateControllerBase : Controllers.CustomizeStateControllerBase
    {
        protected CustomizeStateControllerBase()
        {
            InitializeComponent();
            RegisterActions(components);
        }

        public static ActionBase GetAction(string actionId,Frame frame)
        {
            return
                (frame.Controllers.Values.Where(
                    controller => controller.Actions.Any(actionBase => actionBase.Id == actionId))).Single().Actions[actionId];

        }

    }
}