using System;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using eXpand.ExpressApp.Logic.Conditional;

namespace eXpand.ExpressApp.AdditionalViewControlsProvider.Logic {
    public class AdditionalViewControlsRuleNodeWrapper : ConditionalLogicRuleNodeWrapper, IAdditionalViewControlsRule {
        public const string NodeNameAttribute = "AdditionalViewControlRule";



        public AdditionalViewControlsRuleNodeWrapper(DictionaryNode ruleNode) : base(ruleNode) {
        }


        public string Message {
            get { return Node.GetAttributeValue("Message"); }
            set { Node.SetAttribute("Message", value); }
        }

        public string MessagePropertyName {
            get { return Node.GetAttributeValue("MessagePropertyName"); }
            set { Node.SetAttribute("MessagePropertyName", value); }
        }

        public Type DecoratorType {
            get { return ReflectionHelper.GetType(Node.GetAttributeValue("DecoratorType")); }
            set { Node.SetAttribute("DecoratorType", value.AssemblyQualifiedName); }
        }

        public Type ControlType {
            get { return ReflectionHelper.GetType(Node.GetAttributeValue("ControlType")); }
            set { Node.SetAttribute("ControlType", value.AssemblyQualifiedName); }
        }


        public AdditionalViewControlsProviderPosition AdditionalViewControlsProviderPosition {
            get {
                return Node.GetAttributeEnumValue("AdditionalViewControlsProviderPosition",
                                                  AdditionalViewControlsProviderPosition.Bottom);
            }
            set { Node.SetAttribute("AdditionalViewControlsProviderPosition", value.ToString()); }
        }

        public bool UseSameIfFound {
            get { return Node.GetAttributeBoolValue("UseSameIfFound"); }
            set { Node.SetAttribute("UseSameIfFound",value); }
        }
    }
}