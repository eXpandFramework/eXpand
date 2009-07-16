using System;
using System.Reflection;
using System.Collections.ObjectModel;

namespace DevExpress.ExpressApp.ConditionalEditorState.Core {
    public class EditorStateRule {
        private Guid instanceGUID;
        public EditorStateRule(Type objectType, string properties, EditorState editorState, string criteria, ViewType viewType, MethodInfo methodInfo) {
            objectTypeCore = objectType;
            propertiesCore = EditorStateRuleManager.ParseProperties(objectType, properties);
            viewTypeCore = viewType;
            editorStateCore = editorState;
            criteriaCore = criteria;
            methodInfoCore = methodInfo;
            this.instanceGUID = Guid.NewGuid();
        }
        private Type objectTypeCore = null;
        public Type ObjectType {
            get { return objectTypeCore; }
        }
        private ReadOnlyCollection<string> propertiesCore = null;
        public ReadOnlyCollection<string> Properties {
            get { return propertiesCore; }
        }
        private ViewType viewTypeCore = ViewType.Any;
        public ViewType ViewType {
            get { return viewTypeCore; }
        }
        private EditorState editorStateCore = EditorState.Default;
        public EditorState EditorState {
            get { return editorStateCore; }
        }
        private string criteriaCore = string.Empty;
        public string Criteria {
            get { return criteriaCore; }
        }
        private MethodInfo methodInfoCore = null;
        public MethodInfo MethodInfo {
            get { return methodInfoCore; }
        }
        private bool isDirtyCore = true;
        public bool IsDirty {
            get { return isDirtyCore; }
            set { isDirtyCore = value; }
        }
        public override string ToString() {
            return string.Format("{0}({1},{2},{3},{4},{5}-{6})", base.ToString(), ObjectType, Properties, ViewType, Criteria, MethodInfo, instanceGUID);
        }
    }
}