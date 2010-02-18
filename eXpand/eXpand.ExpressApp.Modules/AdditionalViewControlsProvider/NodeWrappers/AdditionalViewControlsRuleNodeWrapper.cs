using System;
using DevExpress.ExpressApp;
using eXpand.ExpressApp.Logic;

namespace eXpand.ExpressApp.AdditionalViewControlsProvider.NodeWrappers {
    public class AdditionalViewControlsRuleNodeWrapper : ModelRuleNodeWrapper, IAdditionalViewControlsRule {
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
            get { return Type.GetType(Node.GetAttributeValue("DecoratorType")); }
            set {
                string assemblyQualifiedName = null;
                if (value != null) assemblyQualifiedName = value.AssemblyQualifiedName;
                Node.SetAttribute("DecoratorType", assemblyQualifiedName);
            }
        }

        public Type ControlType {
            get { return Type.GetType(Node.GetAttributeValue("ControlType")); }
            set {
                string assemblyQualifiedName = null;
                if (value != null) assemblyQualifiedName = value.AssemblyQualifiedName;
                Node.SetAttribute("ControlType", assemblyQualifiedName);
            }
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