using System;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Xpo;

namespace Xpand.ExpressApp.Logic.Win {
    /// <summary>
    /// A window controller for the Windows Forms platform that tracks the changes with external objects and forces recalculating the rules.
    /// </summary>
    public class TrackObjectChangesViewController : ViewController {
        public const string ActiveKeyAllowTrackObjectChanges = "AllowTrackObjectChanges";
        public const string ActiveKeyIsRootObjectSpace = "ActiveKeyIsRootObjectSpace";

        public static bool AllowTrackObjectChanges { get; set; }

        protected override void OnViewChanging(View view) {
            base.OnViewChanging(view);
            Active[ActiveKeyAllowTrackObjectChanges] = AllowTrackObjectChanges;
            Active[ActiveKeyIsRootObjectSpace] = !(view.ObjectSpace is XPNestedObjectSpace);
        }

        protected override void OnActivated() {
            base.OnActivated();
            Application.ViewShown += Application_ViewShown;
        }

        private void Application_ViewShown(object sender, ViewShownEventArgs e) {
            if (e.TargetFrame != null && e.TargetFrame.View != null) {
                if (!(e.TargetFrame.View.ObjectSpace is XPNestedObjectSpace)) {
                    e.TargetFrame.View.ObjectSpace.Committed += ObjectSpace_Committed;
                }
            }
        }

        protected virtual void ObjectSpace_Committed(object sender, EventArgs e) {
            if (View != null && View.ObjectSpace != null && !View.ObjectSpace.IsDisposed) {
                if (!View.ObjectSpace.IsModified && (View.IsRoot || (!View.IsRoot && View is DetailView)) &&
                    Application.Modules.OfType<IRuleHolder>().Any(holder => holder.HasRules(View))) {
                    View.ObjectSpace.Refresh();
                }
            }
        }

        protected override void OnDeactivated() {
            Application.ViewShown -= Application_ViewShown;
            base.OnDeactivated();
        }
    }
}