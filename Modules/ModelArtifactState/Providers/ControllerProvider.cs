using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Core;
using eXpand.ExpressApp.Core;
using eXpand.ExpressApp.ModelArtifactState.ControllersState;
using eXpand.ExpressApp.ModelArtifactState.Interfaces;

namespace eXpand.ExpressApp.ModelArtifactState.Providers
{
    internal class ControllerProvider
    {
        private readonly IModuleRule activationRule;
        private readonly Controller controller;
        private readonly string controllerType;

        public ControllerProvider(IModuleRule activationRule, Controller controller, string controllerType)
        {
            this.activationRule = activationRule;
            this.controller = controller;
            this.controllerType = controllerType;
        }

        public IList<Controller> Find()
        {
            var controllers = new List<Controller>();
            if (String.IsNullOrEmpty(activationRule.Module))
                getCurrent(controllers);
            else
                getAllFromModule(controllers);
            return controllers;
        }

        private void getAllFromModule(List<Controller> controllers)
        {
            var regex = new Regex(activationRule.Module);
            IEnumerable<ModuleBase> moduleBases =
                controller.Application.Modules.Where(@base => regex.IsMatch(@base.Name));
            foreach (var moduleBase in moduleBases)
                foreach (var collectController in moduleBase.CollectControllers())
                {
                    Controller controller1;
                    CustomizeControllerStateControllerBase.GetController(collectController.GetType(),
                                                                              out controller1, controller.Frame);
                    controllers.Add(controller1);

                }
            
            
        }

        private void getCurrent(List<Controller> controllers)
        {
            Controller controller1;
            ControllersManager controllersManager = controller.Application.Modules[0].ModuleManager.ControllersManager;
            CustomizeControllerStateControllerBase.GetController(
                controllersManager.CollectControllers(
                    typeInfo => typeInfo.FullName == controllerType).Single().GetType(),
                out controller1, controller.Frame);
            controllers.Add(controller1);

            
        }


    }
}