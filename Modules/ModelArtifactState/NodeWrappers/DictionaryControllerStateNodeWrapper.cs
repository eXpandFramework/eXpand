using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Core;
using System.Linq;

namespace eXpand.ExpressApp.ModelArtifactState.NodeWrappers
{
    public class DictionaryControllerStateNodeWrapper : ModelArtifactDictionaryStateNodeWrapper
    {
        private Type controllerType;
        private readonly ControllersManager controllersManager;

        public DictionaryControllerStateNodeWrapper(DictionaryNode dictionaryNode,ControllersManager controllersManager)
        {
            DictionaryNode = dictionaryNode;
            this.controllersManager = controllersManager;
        }

        public Type ControllerType
        {
            get
            {
                if (controllerType == null)
                    controllerType = controllersManager.CollectControllers(
                        typeInfo => typeInfo.FullName == DictionaryNode.GetAttributeValue("Name")).Single().GetType();


                return controllerType;
            }
        }
    }
}