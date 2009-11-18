using DevExpress.ExpressApp;
using DevExpress.ExpressApp.NodeWrappers;
using System.Linq;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Controllers;
using System;

namespace eXpand.ExpressApp.AdditionalViewControlsProvider.NodeWrappers {
    public class AdditionalViewControlsRuleWrapper : NodeWrapper
    {
        public AdditionalViewControlsRuleWrapper(View view)
        {
            ClassInfoNodeWrapper classInfoNodeWrappers = new ApplicationNodeWrapper(view.Info.Dictionary.RootNode).BOModel.Classes.Where(wrapper => wrapper.ClassTypeInfo==view.ObjectTypeInfo).FirstOrDefault();
            dictionaryNode = classInfoNodeWrappers.Node.GetChildNode(UpdateModelViewController.AdditionalViewControls);
        }

        private readonly DictionaryNode dictionaryNode;

        public string Message
        {
            get { return dictionaryNode.GetAttributeValue("Message"); }
            set
            {
                dictionaryNode.SetAttribute("Message", value);
            }
        }
        public string MessagePropertyName
        {
            get { return dictionaryNode.GetAttributeValue("MessagePropertyName"); }
            set
            {
                dictionaryNode.SetAttribute("MessagePropertyName", value);
            }
        }
        public Type DecoratorType
        {
            get { return Type.GetType(dictionaryNode.GetAttributeValue("DecoratorType")); }
            set
            {
                dictionaryNode.SetAttribute("DecoratorType", value.AssemblyQualifiedName);
            }
        }
        public Type ControlType
        {
            get { return Type.GetType(dictionaryNode.GetAttributeValue("ControlType")); }
            set
            {
                dictionaryNode.SetAttribute("ControlType", value.AssemblyQualifiedName);
            }
        }
        public ViewType ViewType
        {
            get { return dictionaryNode.GetAttributeEnumValue("ViewType", ViewType.Any); }
            set
            {
                dictionaryNode.SetAttribute("ViewType", value.ToString());
            }
        }
        public AdditionalViewControlsProviderPosition AdditionalViewControlsProviderPosition
        {
            get { return dictionaryNode.GetAttributeEnumValue("AdditionalViewControlsProviderPosition", AdditionalViewControlsProviderPosition.Bottom); }
            set
            {
                dictionaryNode.SetAttribute("AdditionalViewControlsProviderPosition", value.ToString());
            }
        }
    }
}