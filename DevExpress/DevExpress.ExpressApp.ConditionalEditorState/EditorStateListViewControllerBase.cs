using System;
using DevExpress.ExpressApp.ConditionalEditorState.Core;
using DevExpress.ExpressApp;

namespace DevExpress.ExpressApp.ConditionalEditorState {
    /// <summary>
    /// A base controller for all the ListView controllers that provides the capability to customize the view's editors.
    /// </summary>
    public abstract class EditorStateListViewControllerBase : EditorStateViewControllerBase {
        public EditorStateListViewControllerBase(): base() {
            TargetViewType = ViewType.ListView;
        }
        private ListView listViewCore;
        protected ListView ListView {
            get {
                return listViewCore;
            }
        }
        protected override void OnActivated() {
            base.OnActivated();
            if (NeedsCustomization) {
                if (Frame.View is ListView) {
                    listViewCore = (ListView)Frame.View;
                }
            }
        }
        protected override void CustomizeItem(EditorStateInfo info) {
            EditorStateInfoCustomizingEventArgs args = new EditorStateInfoCustomizingEventArgs(info, false);
            RaiseEditorStateCustomizing(args);
            if (!args.Cancel) {
                switch (info.EditorState) {
                    case EditorState.Hidden: {
                            HideShowColumn(info.Property, info.Active);
                            break;
                        }
                    case EditorState.Disabled:
                    case EditorState.Default:
                    default: break;
                }
                RaiseEditorStateCustomized(new EditorStateInfoCustomizedEventArgs(info));
            }
        }
        protected abstract void HideShowColumn(string name, bool hidden);
        protected abstract object GetColumn(string property);
        protected abstract string GetColumnFieldName(string property);
    }
}