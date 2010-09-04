using DevExpress.XtraLayout;
using Xpand.ExpressApp.SystemModule;

namespace Xpand.ExpressApp.Win.SystemModule {
    public class HighlightFocusedLayoutItemDetailViewController : HighlightFocusedLayoutItemDetailViewControllerBase
    {
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            AssignStyle(View.LayoutManager.Container);
        }

        protected override void AssignStyle(object control)
        {
            var layoutControl = control as LayoutControl;
            if (layoutControl != null)
            {
                layoutControl.BeginUpdate();
                layoutControl.OptionsView.HighlightFocusedItem = true;
                layoutControl.OptionsView.AllowItemSkinning = true;
                layoutControl.OptionsView.AllowHotTrack = true;
                layoutControl.EndUpdate();
            }
        }
    }
}