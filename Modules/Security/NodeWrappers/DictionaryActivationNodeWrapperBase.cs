using DevExpress.ExpressApp;
using DevExpress.ExpressApp.NodeWrappers;
using eXpand.ExpressApp.Security.Interfaces;

namespace eXpand.ExpressApp.Security.NodeWrappers
{
    public abstract class DictionaryActivationNodeWrapperBase:NodeWrapper,IActivationRule
    {
        private DictionaryNode dictionaryNode;

        public DictionaryNode DictionaryNode
        {
            get { return dictionaryNode; }
            set { dictionaryNode = value; }
        }

        public string EmptyCriteria
        {
            get { return dictionaryNode.GetAttributeValue("EmptyCriteria"); }
            set { dictionaryNode.SetAttribute("EmptyCriteria", value); }
        }


        public string NormalCriteria
        {
            get { return dictionaryNode.GetAttributeValue("NormalCriteria"); }
            set { dictionaryNode.SetAttribute("NormalCriteria", value); }
        }

        public ViewType ViewType
        {
            get { return dictionaryNode.GetAttributeEnumValue("ViewType", ViewType.Any); }
            set { dictionaryNode.SetAttribute("ViewType", value.ToString()); }
        }

        public Nesting Nesting
        {
            get { return dictionaryNode.GetAttributeEnumValue("Nesting", Nesting.Any); }
            set { dictionaryNode.SetAttribute("Nesting", value.ToString()); }
        }
    }
}