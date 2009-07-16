using System;
using DevExpress.ExpressApp;

namespace DevExpress.ExpressApp.ConditionalEditorState {
    /// <summary>
    /// This is a regular .NET attribute that should be used to define rules that determine the state of editors for properties of business classes against a business rule.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public sealed class EditorStateRuleAttribute : Attribute {
        private Guid instanceGUID;
        public EditorStateRuleAttribute(string id, string properties, EditorState editorState, string criteria, ViewType viewType, string description) {
            this.idCore = id;
            this.propertiesCore = properties;
            this.editorStateCore = editorState;
            this.criteriaCore = criteria;
            this.viewTypeCore = viewType;
            this.descriptionCore = description;
            this.instanceGUID = Guid.NewGuid();
        }
        public EditorStateRuleAttribute(string id, string targetProperties, EditorState editorState, string criteria, ViewType targetViewType)
            : this(id, targetProperties, editorState, criteria, targetViewType, null) {
        }
        public EditorStateRuleAttribute(string id, string targetProperties, ViewType targetViewType, string description)
            : this(id, targetProperties, EditorState.Default, null, targetViewType, description) {
        }
        public override object TypeId {
            get { return (object)instanceGUID; }
        }
        public override string ToString() {
            return base.ToString() + "." + instanceGUID.ToString();
        }
        private string idCore = string.Empty;
        public string ID {
            get {
                return idCore;
            }
        }
        private string propertiesCore = string.Empty;
        /// <summary>
        /// Determines a string with target properties separated by semicolons those editor states would be changed.
        /// </summary>
        public string Properties {
            get { return propertiesCore; }
        }
        private ViewType viewTypeCore = ViewType.Any;
        /// <summary>
        /// Determines a DevExpress.ExpressApp.ViewType, in which editor states may be changed.
        /// </summary>
        public ViewType ViewType {
            get { return viewTypeCore; }
        }
        private EditorState editorStateCore;
        /// <summary>
        /// Determines the state of editors.
        /// </summary>
        public EditorState EditorState {
            get { return editorStateCore; }
        }
        private string criteriaCore = string.Empty;
        /// <summary>
        /// Represents a criteria string that must determine whether the editor states should be customized.
        /// </summary>
        public string Criteria {
            get { return criteriaCore; }
        }
        private string descriptionCore = string.Empty;
        /// <summary>
        /// Represents a string that describes the current rule.
        /// </summary>
        public string Description {
            get { return descriptionCore; }
        }
    }
    /// <summary>
    /// Determines the state of editors.
    /// </summary>
    public enum EditorState {
        Default,
        Disabled,
        Hidden
    }
}