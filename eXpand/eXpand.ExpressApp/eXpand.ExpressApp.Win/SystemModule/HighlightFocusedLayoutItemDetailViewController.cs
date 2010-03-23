using DevExpress.XtraLayout;
using eXpand.ExpressApp.SystemModule;

namespace eXpand.ExpressApp.Win.SystemModule {
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
                layoutControl.EndUpdate();
            }
        }
    }
}