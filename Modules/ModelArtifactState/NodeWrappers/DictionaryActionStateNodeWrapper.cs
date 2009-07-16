using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Core;
using System.Linq;

namespace eXpand.ExpressApp.ModelArtifactState.NodeWrappers
{
    public class DictionaryActionStateNodeWrapper : ModelArtifactDictionaryStateNodeWrapper
    {
        private readonly ControllersManager controllersManager;
        private ActionBase action;

        public DictionaryActionStateNodeWrapper(DictionaryNode dictionaryNode, ControllersManager controllersManager)
        {
            this.controllersManager = controllersManager;
            DictionaryNode=dictionaryNode;
        }

        public ActionBase Action
        {
            get
            {
                if (action == null)
                {
                    var attributeValue = DictionaryNode.GetAttributeValue("Name");
                    IList<Controller> controllers = controllersManager.CollectControllers(info => true);
                    action =
                        (controllers.Where(
                            controller =>
                            controller.Actions.Any(
                                actionBase => actionBase.Id == attributeValue)).FirstOrDefault()).Actions[attributeValue];
                }
                return action;
            }
        }
    }
}