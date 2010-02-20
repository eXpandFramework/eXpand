using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.NodeWrappers;
using eXpand.Persistent.Base.General;

namespace eXpand.ExpressApp.Logic {
    public abstract class LogicRuleNodeWrapper : NodeWrapper, ILogicRule {
        public const string IDAttribute = "ID";
        public const string DescriptionAttribute = "Description";
        public const string TypeInfoAttribute = "TypeInfo";
        
        public const string ViewTypeAttribute = "ViewType";
        public const string NestingAttribute = "Nesting";
        public const string ViewIdAttribute = "ViewId";
        ITypeInfo _typeInfo;

        protected LogicRuleNodeWrapper(DictionaryNode ruleNode) : base(ruleNode) {
        }

        public DictionaryNode DictionaryNode { get; set; }

        #region ILogicRule Members

        public string ViewId {
            get { return Node.GetAttributeValue(ViewIdAttribute); }
            set { Node.SetAttribute(ViewIdAttribute, value); }
        }

        public string Description {
            get { return Node.GetAttributeValue(DescriptionAttribute); }
            set { Node.SetAttribute(DescriptionAttribute, value); }
        }

        public ITypeInfo TypeInfo {
            get {
                string typeName = Node.GetAttributeValue(TypeInfoAttribute);
                if (_typeInfo == null && !string.IsNullOrEmpty(typeName)) {
                    _typeInfo = XafTypesInfo.Instance.FindTypeInfo(typeName);
                }
                return _typeInfo;
            }
            set {
                _typeInfo = value;
                Node.SetAttribute(TypeInfoAttribute, _typeInfo != null ? _typeInfo.FullName : string.Empty);
            }
        }

        public string ID {
            get { return Node.GetAttributeValue(IDAttribute); }
            set { Node.SetAttribute(IDAttribute, value); }
        }


        public ViewType ViewType {
            get { return GetEnumValue(ViewTypeAttribute, ViewType.Any); }
            set { Node.SetAttribute(ViewTypeAttribute, value.ToString()); }
        }

        public Nesting Nesting {
            get { return GetEnumValue(NestingAttribute, Nesting.Any); }
            set { Node.SetAttribute(NestingAttribute, value.ToString()); }
        }
        #endregion

    }

}