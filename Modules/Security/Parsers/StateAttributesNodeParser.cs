using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.NodeWrappers;
using eXpand.ExpressApp.Security.Attributes;

namespace eXpand.ExpressApp.Security.Parsers
{
    public abstract class StateAttributesNodeParser
    {
        public void AddAttributesToXafTypesInfoFromBOModel(WindowController windowController, string nodeName)
        {
            IEnumerable<ClassInfoNodeWrapper> wrappers =
                new ApplicationNodeWrapper(windowController.Application.Info).BOModel.Classes.Where(
                    wrapper => wrapper.Node.ChildNodes.Any(node => node.Name == nodeName));
            foreach (ClassInfoNodeWrapper classInfoNodeWrapper in wrappers)
            {
                DictionaryNode node = classInfoNodeWrapper.Node.GetChildNode(nodeName);
                foreach (DictionaryNode childNode in node.ChildNodes)
                {
                    ViewType viewType = childNode.GetAttributeEnumValue(typeof (ViewType).Name, ViewType.Any);
                    StateRuleAttribute controllerStateRuleAttribute;
                    if (viewType == ViewType.Any)
                    {
                        
                        controllerStateRuleAttribute = GetActivationRuleAttribute(childNode, ViewType.
                                                                                                      DetailView,
                                                                                       windowController.Application);
                        classInfoNodeWrapper.ClassTypeInfo.AddAttribute(controllerStateRuleAttribute);
                        controllerStateRuleAttribute = GetActivationRuleAttribute(childNode, ViewType.ListView,
                                                                                       windowController.Application);
                        classInfoNodeWrapper.ClassTypeInfo.AddAttribute(controllerStateRuleAttribute);
                    }
                    else
                    {
                        controllerStateRuleAttribute = GetActivationRuleAttribute(childNode, viewType,
                                                                                       windowController.Application);
                        classInfoNodeWrapper.ClassTypeInfo.AddAttribute(controllerStateRuleAttribute);
                    }
                }
            }
        }

        protected abstract StateRuleAttribute GetActivationRuleAttribute(DictionaryNode childNode,
                                                                              ViewType viewType,
                                                                              XafApplication xafApplication);
    }
}