using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Core;
using eXpand.ExpressApp.Core;
using eXpand.ExpressApp.ModelArtifactState.ActionsState;
using eXpand.ExpressApp.ModelArtifactState.Interfaces;

namespace eXpand.ExpressApp.ModelArtifactState.Providers
{
    public class ActionProvider
    {
        private readonly IModuleRule rule;
        private readonly Controller controller;
        private readonly string actionId;

        public ActionProvider(IModuleRule rule, Controller controller, string actionId)
        {
            this.rule = rule;
            this.controller = controller;
            this.actionId = actionId;
        }

        public ICollection<ActionBase> Find()
        {
            var actionBases = new List<ActionBase>();
            if (String.IsNullOrEmpty(rule.Module))
                getCurrent(actionBases);
            else
                getAllFromModule(actionBases);
            return actionBases;    
        }

        private void getAllFromModule(List<ActionBase> actionBases)
        {
            var regex = new Regex(rule.Module);
            IEnumerable<ActionList> actionLists =
                controller.Application.Modules.Where(@base => regex.IsMatch(@base.Name)).Single().CollectControllers
                    ().Select(controller1 => controller1.Actions);
            foreach (ActionList actionList in actionLists)
                foreach (var action in actionList)
                    actionBases.Add(CustomizeStateControllerBase.GetAction(action.Id, controller.Frame));

        }
        private Controller GetAction(ControllersManager controllersManager)
        {
            return (controllersManager.CollectControllers(info => true).Where(
                controller1 =>
                controller1.Actions.Any(
                    actionBase => actionBase.Id == actionId)).Single());
        }

        private void getCurrent(List<ActionBase> actionBases)
        {
            ActionBase action = GetAction(controller.Application.Modules[0].ModuleManager.ControllersManager).Actions[actionId];
            actionBases.Add( CustomizeStateControllerBase.GetAction(action.Id, controller.Frame));
        }
    }
}