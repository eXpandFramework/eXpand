using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.NodeWrappers;
using eXpand.ExpressApp.Security.Interfaces;
using eXpand.ExpressApp.Security.Permissions;

namespace eXpand.ExpressApp.Security.NodeWrappers
{
    public abstract class DictionaryStateNodeWrapperBase:NodeWrapper,IStateRule
    {
        private ITypeInfo typeInfo;
        public const string IDAttribute = "ID";
        public const string DescriptionAttribute = "Description";
        public const string TypeInfoAttribute = "TypeInfo";
        public const string StateAttribute = "State";
        public const string NormalCriteriaAttribute = "NormalCriteria";
        public const string EmptyCriteriaAttribute = "EmptyCriteria";
        public const string ViewTypeAttribute = "ViewType";
        public const string NestingAttribute = "Nesting";
        public const string ViewIdAttribute = "ViewId";
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
        public string ViewId
        {
            get { return Node.GetAttributeValue(ViewIdAttribute); }
            set { Node.SetAttribute(ViewIdAttribute, value); }
        }
        public string Description
        {
            get { return Node.GetAttributeValue(DescriptionAttribute); }
            set { Node.SetAttribute(DescriptionAttribute, value); }
        }
        public ITypeInfo TypeInfo
        {
            get
            {
                string typeName = Node.GetAttributeValue(TypeInfoAttribute);
                if (typeInfo == null && !string.IsNullOrEmpty(typeName))
                {
                    typeInfo = XafTypesInfo.Instance.FindTypeInfo(typeName);
                }
                return typeInfo;
            }
            set
            {
                typeInfo = value;
                Node.SetAttribute(TypeInfoAttribute, typeInfo != null ? typeInfo.FullName : string.Empty);
            }
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