using DevExpress.ExpressApp;
using DevExpress.ExpressApp.NodeWrappers;
using eXpand.ExpressApp.Security.Interfaces;
using eXpand.ExpressApp.Security.Permissions;

namespace eXpand.ExpressApp.Security.NodeWrappers
{
    public abstract class DictionaryStateNodeWrapperBase:NodeWrapper,IStateRule
    {
        public const string IDAttribute = "ID";
        public const string StateAttribute = "State";
        public const string NormalCriteriaAttribute = "NormalCriteria";
        public const string EmptyCriteriaAttribute = "EmptyCriteria";
        public const string ViewTypeAttribute = "ViewType";
        public const string NestingAttribute = "Nesting";
        protected DictionaryStateNodeWrapperBase(DictionaryNode ruleNode) : base(ruleNode) { }
        public DictionaryNode DictionaryNode { get; set; }

        public State State
        {
            get { return GetEnumValue(StateAttribute, State.Default); }
            set { Node.SetAttribute(StateAttribute, value.ToString()); }
        }
        public string NormalCriteria
        {
            get { return Node.GetAttributeValue(NormalCriteriaAttribute); }
            set { Node.SetAttribute(NormalCriteriaAttribute, value); }
        }
        public string ID
        {
            get { return Node.GetAttributeValue(IDAttribute); }
            set { Node.SetAttribute(IDAttribute, value); }
        }
        public string EmptyCriteria
        {
            get { return Node.GetAttributeValue(EmptyCriteriaAttribute); }
            set { Node.SetAttribute(EmptyCriteriaAttribute, value); }
        }
        public ViewType ViewType
        {
            get { return GetEnumValue(ViewTypeAttribute, ViewType.Any); }
            set { Node.SetAttribute(ViewTypeAttribute, value.ToString()); }
        }
        public Nesting Nesting
        {
            get { return GetEnumValue(NestingAttribute, Nesting.Any); }
            set { Node.SetAttribute(NestingAttribute, value.ToString()); }
        }
        
    }
}