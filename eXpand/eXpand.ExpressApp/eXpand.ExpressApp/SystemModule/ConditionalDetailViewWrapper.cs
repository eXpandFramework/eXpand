using DevExpress.ExpressApp;
using DevExpress.ExpressApp.NodeWrappers;

namespace eXpand.ExpressApp.SystemModule {
    public class ConditionalDetailViewWrapper : NodeWrapper {
        public const string CriteriaAttributeName = "Criteria";
        public const string IDAttributeName = "ID";
        public const string ClassNameAttributeName = "ClassName";
        public const string ModeAttributeName = "Mode";
        public const string NewModeBehaviorAttributeName = "NewModeBehavior";
        public const string DetailViewIDAttributeName = "DetailViewID";
        public const string IndexAttributeName = "Index";

        public ConditionalDetailViewWrapper(DictionaryNode node) : base(node) {
        }

        public string Criteria {
            get { return Node.GetAttributeValue(CriteriaAttributeName); }
            set { Node.SetAttribute(CriteriaAttributeName, value); }
        }

        public string ID {
            get { return Node.GetAttributeValue(IDAttributeName); }
            set { Node.SetAttribute(IDAttributeName, value); }
        }

        public string ClassName {
            get { return Node.GetAttributeValue(ClassNameAttributeName); }
            set { Node.SetAttribute(ClassNameAttributeName, value); }
        }

        public string Mode {
            get { return Node.GetAttributeValue(ModeAttributeName); }
            set { Node.SetAttribute(ModeAttributeName, value); }
        }

        public string NewModeBehavior {
            get { return Node.GetAttributeValue(NewModeBehaviorAttributeName); }
            set { Node.SetAttribute(NewModeBehaviorAttributeName, value); }
        }

        public string DetailViewID {
            get { return Node.GetAttributeValue(DetailViewIDAttributeName); }
            set { Node.SetAttribute(DetailViewIDAttributeName, value); }
        }

        public string Index {
            get { return Node.GetAttributeValue(IndexAttributeName); }
            set { Node.SetAttribute(IndexAttributeName, value); }
        }
    }
}