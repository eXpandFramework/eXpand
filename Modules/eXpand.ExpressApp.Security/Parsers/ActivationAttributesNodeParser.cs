using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.NodeWrappers;
using eXpand.ExpressApp.Security.Attributes;

namespace eXpand.ExpressApp.Security.Parsers
{
    public abstract class ActivationAttributesNodeParser
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
                    ActivationRuleAttribute controllerActivationRuleAttribute;
                    if (viewType == ViewType.Any)
                    {
                        
                        controllerActivationRuleAttribute = GetActivationRuleAttribute(childNode, ViewType.
                                                                                                      DetailView,
                                                                                       windowController.Application);
                        classInfoNodeWrapper.ClassTypeInfo.AddAttribute(controllerActivationRuleAttribute);
                        controllerActivationRuleAttribute = GetActivationRuleAttribute(childNode, ViewType.ListView,
                                                                                       windowController.Application);
                        classInfoNodeWrapper.ClassTypeInfo.AddAttribute(controllerActivationRuleAttribute);
                    }
                    else
                    {
                        controllerActivationRuleAttribute = GetActivationRuleAttribute(childNode, viewType,
                                                                                       windowController.Application);
                        classInfoNodeWrapper.ClassTypeInfo.AddAttribute(controllerActivationRuleAttribute);
                    }
                }
            }
        }

        protected abstract ActivationRuleAttribute GetActivationRuleAttribute(DictionaryNode childNode,
                                                                              ViewType viewType,
                                                                              XafApplication xafApplication);
    }
}