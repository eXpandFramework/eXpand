using System;
using DevExpress.ExpressApp;
using eXpand.ExpressApp.ModelArtifactState.Controllers;

namespace eXpand.ExpressApp.ModelArtifactState.ControllersState
{
    public abstract partial class CustomizeControllerStateControllerBase : CustomizeStateControllerBase
    {
        protected CustomizeControllerStateControllerBase()
        {
            InitializeComponent();
            RegisterActions(components);
        }

        /// <summary>
        /// Emulate GetController<> because I need to use a non strong typing GetController because the type of controller
        /// is property of ControllerStateRuleAttribute
        /// </summary>
        /// <param name="ControllerToFind"></param>
        /// <param name="ControllerFound"></param>
        /// <returns></returns>
        public static bool GetController(Type ControllerToFind, out Controller ControllerFound,Frame frame)
        {
            ControllerFound = null;
            foreach (Controller controller in frame.Controllers.Values)
                if (ControllerToFind.IsAssignableFrom(controller.GetType()))
                {
                    ControllerFound = controller;
                    break;
                }
            return ControllerFound != null;
        }

    }
}