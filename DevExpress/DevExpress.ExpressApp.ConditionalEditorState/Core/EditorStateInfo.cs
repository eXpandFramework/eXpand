using System;

namespace DevExpress.ExpressApp.ConditionalEditorState.Core {
    /// <summary>
    /// A helper class that is used to store the information about the editor
    /// </summary>
    public sealed class EditorStateInfo {
        public EditorStateInfo(bool active, object obj, string property, EditorState editorState, EditorStateRule rule) {
            activeCore = active;
            objectCore = obj;
            propertyCore = property;
            editorStateCore = editorState;
            ruleCore = rule;
        }
        private EditorStateRule ruleCore;
        /// <summary>
        /// Represents a string that describes the current rule.
        /// </summary>
        public EditorStateRule Rule {
            get { return ruleCore; }
        }
        private object objectCore = null;
        /// <summary>
        /// Currently processed object in the View.
        /// </summary>
        public object Object {
            get {
                return objectCore;
            }
        }
        private string propertyCore = null;
        /// <summary>
        /// Determines a property whose editor state would be changed.
        /// </summary>
        public string Property {
            get {
                return propertyCore;
            }
        }
        private EditorState editorStateCore = EditorState.Default;
        /// <summary>
        /// Gets or sets the state of editors.
        /// </summary>
        public EditorState EditorState {
            get { return editorStateCore; }
            set { editorStateCore = value; }
        }
        private bool activeCore = false;
        /// <summary>
        /// Gets or sets whether the selected customization should be applied to the selected editors.
        /// </summary>
        public bool Active {
            get { return activeCore; }
            set { activeCore = value; }
        }
    }
}