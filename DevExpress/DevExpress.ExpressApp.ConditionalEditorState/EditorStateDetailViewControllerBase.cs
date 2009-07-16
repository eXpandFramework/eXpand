using System;
using DevExpress.ExpressApp.ConditionalEditorState.Core;
using DevExpress.ExpressApp.Editors;
using DevExpress.Persistent.Base;

namespace DevExpress.ExpressApp.ConditionalEditorState {
    /// <summary>
    /// A base controller for all the DetailView controllers that provides the capability to customize the view's editors.
    /// </summary>
    public abstract class EditorStateDetailViewControllerBase : EditorStateViewControllerBase {
        public EditorStateDetailViewControllerBase()
            : base() {
            TargetViewType = ViewType.DetailView;
        }
        private DetailView detailViewCore;
        protected DetailView DetailView {
            get {
                return detailViewCore;
            }
        }
        protected override void OnActivated() {
            base.OnActivated();
            if (NeedsCustomization) {
                if (Frame.View is DetailView) {
                    detailViewCore = (DetailView)Frame.View;
                    detailViewCore.CurrentObjectChanged += OnViewCurrentObjectChanged;
                    detailViewCore.ObjectSpace.ObjectChanged += OnObjectSpaceObjectChanged;
                    detailViewCore.ObjectSpace.Reloaded += OnObjectSpaceReloaded;
                    detailViewCore.ObjectSpace.Refreshing += OnObjectSpaceRefreshing;
                }
            }
        }
        protected override void ResourcesReleasing() {
            if (NeedsCustomization) {
                if (Frame.View is DetailView) {
                    detailViewCore.CurrentObjectChanged -= OnViewCurrentObjectChanged;
                    detailViewCore.ObjectSpace.ObjectChanged -= OnObjectSpaceObjectChanged;
                    detailViewCore.ObjectSpace.Reloaded -= OnObjectSpaceReloaded;
                    detailViewCore.ObjectSpace.Refreshing -= OnObjectSpaceRefreshing;
                }
            }
            base.ResourcesReleasing();
        }
        protected bool isRefreshing = false;
        protected virtual void OnViewCurrentObjectChanged(object sender, EventArgs e) {
            if (!isRefreshing) {
                InvalidateRules(!View.IsRoot);
            }
        }
        protected virtual void OnObjectSpaceObjectChanged(object sender, ObjectChangedEventArgs e) {
            if (!String.IsNullOrEmpty(e.PropertyName)) {
                InvalidateRules(false);
            }
        }
        protected virtual void OnObjectSpaceRefreshing(object sender, System.ComponentModel.CancelEventArgs e) {
            isRefreshing = true;
        }
        protected virtual void OnObjectSpaceReloaded(object sender, EventArgs e) {
            InvalidateRules(true);
            isRefreshing = false;
        }
        protected override void CustomizeItem(EditorStateInfo info) {
            EditorStateInfoCustomizingEventArgs args = new EditorStateInfoCustomizingEventArgs(info, false);
            RaiseEditorStateCustomizing(args);
            if (!args.Cancel) {
                switch (info.EditorState) {
                    case EditorState.Hidden: {
                            HideShowEditor(info.Property, info.Active);
                            break;
                        }
                    case EditorState.Disabled: {
                            if (DetailView.ViewEditMode == ViewEditMode.Edit) {
                                DisableEnableEditor(info.Property, info.Active);
                            }
                            break;
                        }
                    case EditorState.Default:
                    default: break;
                }
                RaiseEditorStateCustomized(new EditorStateInfoCustomizedEventArgs(info));
            }
        }
        protected PropertyEditor GetPropertyEditor(string property) {
            return DetailView.FindItem(property) as PropertyEditor;
        }
        protected virtual void DisableEnableEditor(string property, bool disabled) {
            PropertyEditor editor = GetPropertyEditor(property);
            if (editor != null) {
                if (editor.ReadOnly.ResultValue == disabled) {
                    return;
                } else {
                    editor.ReadOnly.SetItemValue(ToString(), disabled);
                    CustomizeEditor(editor);
                }
            } else {
                Tracing.Tracer.LogWarning(string.Format(EditorStateLocalizer.Active.GetLocalizedString("CannotFindInfoForProperty"), "PropertyEditor", property));
            }
        }
        protected virtual void CustomizeEditor(PropertyEditor editor) { }
        protected abstract void HideShowEditor(string property, bool hidden);
    }
}