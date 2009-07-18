using System.Collections.Generic;
using DevExpress.ExpressApp.NodeWrappers;

namespace DevExpress.ExpressApp.ConditionalEditorState {
    public class EditorStateRuleNodeWrapper : NodeWrapper {
        public const string NodeName = "EditorStateRule";

        public const string IDAttribute = "ID";
        public const string PropertiesAttribute = "Properties";
        public const string EditorStateAttribute = "EditorState";
        public const string CriteriaAttribute = "Criteria";
        public const string ViewTypeAttribute = "ViewType";
        public const string DescriptionAttribute = "Description";

        public EditorStateRuleNodeWrapper() : this(new DictionaryNode(NodeName)) { }
        public EditorStateRuleNodeWrapper(DictionaryNode ruleNode) : base(ruleNode) { }

        public string ID {
            get { return Node.GetAttributeValue(IDAttribute); }
            set { Node.SetAttribute(IDAttribute, value); }
        }
        public string Properties {
            get { return Node.GetAttributeValue(PropertiesAttribute); }
            set { Node.SetAttribute(PropertiesAttribute, value); }
        }
        public EditorState EditorState {
            get { return GetEnumValue<EditorState>(EditorStateAttribute, EditorState.Default); }
            set { Node.SetAttribute(EditorStateAttribute, value.ToString()); }
        }
        public string Criteria {
            get { return Node.GetAttributeValue(CriteriaAttribute); }
            set { Node.SetAttribute(CriteriaAttribute, value); }
        }
        public ViewType ViewType {
            get { return GetEnumValue<ViewType>(ViewTypeAttribute, ViewType.Any); }
            set { Node.SetAttribute(ViewTypeAttribute, value.ToString()); }
        }
        public string Description {
            get { return Node.GetAttributeValue(DescriptionAttribute); }
            set { Node.SetAttribute(DescriptionAttribute, value); }
        }
        public ConditionalEditorStateNodeWrapper ConditionalEditorState {
            get {
                if (Node.Parent != null) {
                    return new ConditionalEditorStateNodeWrapper(Node.Parent);
                } else {
                    return null;
                }
            }
        }
        public override string ToString() {
            if (ConditionalEditorState != null) {
                return string.Format("{0}({1},{2},{3},{4},{5})[{6}]", NodeName, Properties, EditorState, Criteria, ViewType, Description, ConditionalEditorState);
            } else {
                return base.ToString();
            }
        }
    }
    public class ConditionalEditorStateNodeWrapper : NodeWrapper {
        public const string NodeName = "ConditionalEditorState";

        public ConditionalEditorStateNodeWrapper() : this(new DictionaryNode(NodeName)) { }
        public ConditionalEditorStateNodeWrapper(DictionaryNode conditionalEditorStateNode) : base(conditionalEditorStateNode) { }

        public ICollection<EditorStateRuleNodeWrapper> Rules {
            get { return GetChildWrappers<EditorStateRuleNodeWrapper>(EditorStateRuleNodeWrapper.NodeName); }
        }
        public EditorStateRuleNodeWrapper FindRuleByID(string id) {
            DictionaryNode node = Node.FindChildNode(EditorStateRuleNodeWrapper.NodeName, "Name", id);
            if (node != null) {
                return new EditorStateRuleNodeWrapper(node);
            } else {
                return null;
            }
        }
        public EditorStateRuleNodeWrapper AddRule() {
            return new EditorStateRuleNodeWrapper(Node.AddChildNode(EditorStateRuleNodeWrapper.NodeName));
        }
        public EditorStateRuleNodeWrapper AddRule(string id, string properties, EditorState editorState, string criteria, ViewType viewType, string description) {
            EditorStateRuleNodeWrapper result = new EditorStateRuleNodeWrapper(Node.AddChildNode(EditorStateRuleNodeWrapper.NodeName));
            result.ID = id;
            result.Properties = properties;
            result.EditorState = editorState;
            result.Criteria = criteria;
            result.ViewType = viewType;
            result.Description = description;
            return result;
        }
        public ClassInfoNodeWrapper Class {
            get {
                if (Node.Parent != null) {
                    return new ClassInfoNodeWrapper(Node.Parent);
                } else {
                    return null;
                }
            }
        }
        public override string ToString() {
            if (Class != null) {
                return string.Format("{0}[{1}]", NodeName, Class.ClassTypeInfo.FullName);
            } else {
                return base.ToString();
            }
        }
    }
}