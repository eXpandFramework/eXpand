using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.ConditionalEditorState.Core;
using System.ComponentModel;

namespace DevExpress.ExpressApp.ConditionalEditorState {
    /// <summary>
    /// A base controller for all ViewType-specific controllers that provide the capability to customize the view's editors.
    /// </summary>
    public abstract class EditorStateViewControllerBase : ViewController {
        protected Dictionary<string, EditorState> editorStateInfoHolder = new Dictionary<string, EditorState>();
        protected bool isReadyCore = false;
        protected override void OnActivated() {
            base.OnActivated();
            if (NeedsCustomization) {
                View.ControlsCreated += OnViewControlsCreated;
            }
        }
        protected object controlCore = null;
        public virtual object Control {
            get { return controlCore; }
        }
        public virtual bool NeedsCustomization {
            get { return EditorStateRuleManager.NeedsCustomization(View.ObjectTypeInfo.Type); }
        }
        protected override void OnDeactivating() {
            ResourcesReleasing();
            base.OnDeactivating();
        }
        protected virtual void ResourcesReleasing() {
            if (NeedsCustomization) {
                isReadyCore = false;
                View.ControlsCreated -= OnViewControlsCreated;
            }
        }
        protected virtual void OnViewControlsCreated(object sender, EventArgs e) {
            controlCore = View.Control;
            isReadyCore = true;
            InvalidateRules(false);
        }
        protected void RaiseEditorStateCustomizing(EditorStateInfoCustomizingEventArgs args) {
            if (EditorStateCustomizing != null) {
                EditorStateCustomizing(this, args);
            }
        }
        protected void RaiseEditorStateCustomized(EditorStateInfoCustomizedEventArgs args) {
            if (EditorStateCustomized != null) {
                EditorStateCustomized(this, args);
            }
        }
        protected virtual void ForceCustomization() {
            if (IsReady) {
                Type objectType = View.ObjectTypeInfo.Type;
                object currentObject = TargetViewType == ViewType.DetailView ? View.CurrentObject : null;
                foreach (EditorStateRule rule in EditorStateRuleManager.Instance[objectType]) {
                    if (rule.IsDirty && (rule.ViewType == TargetViewType
                        || rule.ViewType == ViewType.Any)) {

                        ForceCustomizationCore(currentObject, rule);
                    }
                }
            }
        }
        protected virtual void ForceCustomizationCore(object currentObject, EditorStateRule rule) {
            foreach (string property in rule.Properties) {
                EditorStateInfo info = EditorStateRuleManager.CalculateEditorStateInfo(currentObject, property, rule);
                if (info != null) {
                    CustomizeItem(info);
                }
            }
            rule.IsDirty = false;
        }
        public virtual void InvalidateRules(bool isMandatory) {
            Type objectType = View.ObjectTypeInfo.Type;
            foreach (EditorStateRule rule in EditorStateRuleManager.Instance[objectType]) {
                if ((rule.ViewType == TargetViewType
                    || rule.ViewType == ViewType.Any)) {

                    rule.IsDirty = true;
                }
            }
            if (isMandatory) {
                ForceCustomization();    
            }
        }
        protected abstract void CustomizeItem(EditorStateInfo info);
        /// <summary>
        /// An event that can be used to be notified whenever editors begin customizing.
        /// </summary>
        public event EventHandler<EditorStateInfoCustomizingEventArgs> EditorStateCustomizing;
        /// <summary>
        /// An event that can be used to be notified whenever editors state has been customized.
        /// </summary>
        public event EventHandler<EditorStateInfoCustomizedEventArgs> EditorStateCustomized;

        public virtual bool IsReady {
            get {
                return isReadyCore;
            }
        }
    }
    /// <summary>
    /// Arguments of the EditorStateCustomizing event.
    /// </summary>
    public class EditorStateInfoCustomizingEventArgs : CancelEventArgs {
        public EditorStateInfoCustomizingEventArgs(EditorStateInfo info, bool cancel) {
            this.editorStateInfoCore = info;
            this.Cancel = cancel;
        }
        private EditorStateInfo editorStateInfoCore = null;
        /// <summary>
        /// Allows you to customize the information about the editor states.
        /// </summary>
        public EditorStateInfo EditorStateInfo {
            get { return editorStateInfoCore; }
            set { editorStateInfoCore = value; }
        }
    }
    /// <summary>
    /// Arguments of the EditorStateCustomized event.
    /// </summary>
    public class EditorStateInfoCustomizedEventArgs : EventArgs {
        public EditorStateInfoCustomizedEventArgs(EditorStateInfo info) {
            this.editorStateInfoCore = info;
        }
        private EditorStateInfo editorStateInfoCore = null;
        /// <summary>
        /// Allows you to know the information about the editor states.
        /// </summary>
        public EditorStateInfo EditorStateInfo {
            get { return editorStateInfoCore; }
            set { editorStateInfoCore = value; }
        }
    }
}