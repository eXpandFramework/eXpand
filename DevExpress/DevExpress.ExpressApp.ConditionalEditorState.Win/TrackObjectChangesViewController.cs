using DevExpress.ExpressApp;
using System;
using DevExpress.ExpressApp.ConditionalEditorState.Core;

namespace DevExpress.ExpressApp.ConditionalEditorState.Win {
    /// <summary>
    /// A window controller for the Windows Forms platform that tracks the changes with external objects and forces recalculating the rules.
    /// </summary>
    public class TrackObjectChangesViewController : ViewController {
        protected override void OnActivated() {
            base.OnActivated();
            if (EditorStateRuleManager.AllowTrackObjectChanges) {
                if (!(View.ObjectSpace is NestedObjectSpace)) {
                    Application.ViewShown += OnApplicationViewShown;
                }
            }
        }
        private void OnApplicationViewShown(object sender, ViewShownEventArgs e) {
            if (e.TargetFrame != null && e.TargetFrame.View != null) {
                if (!(e.TargetFrame.View.ObjectSpace is NestedObjectSpace)) {
                    e.TargetFrame.View.ObjectSpace.Committed += OnObjectSpaceCommitted;
                }
            }
        }
        protected virtual void OnObjectSpaceCommitted(object sender, EventArgs e) {
            if (Frame != null && Frame.View != null) {
                if (EditorStateRuleManager.NeedsCustomization(Frame.View.ObjectTypeInfo.Type) && !Frame.View.ObjectSpace.IsModified && (Frame.View.IsRoot || (!Frame.View.IsRoot && Frame.View is DetailView))) {
                    Frame.View.ObjectSpace.Refresh();
                }
            }
        }
        protected override void OnDeactivating() {
            if (EditorStateRuleManager.AllowTrackObjectChanges) {
                if (!(View.ObjectSpace is NestedObjectSpace)) {
                    Application.ViewShown -= OnApplicationViewShown;
                }
            }
            base.OnDeactivating();
        }
    }
}