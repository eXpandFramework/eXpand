using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using eXpand.ExpressApp.SystemModule;
using eXpand.ExpressApp.Win.ListEditors;

namespace eXpand.ExpressApp.Win.SystemModule
{
    public interface IModelListViewHidePopupMenu : IModelNode
    {
        bool HidePopupMenu { get; set; }
    }

    public class HideGridPopUpMenuViewController : BaseViewController<ListView>
    {
        public HideGridPopUpMenuViewController() { }

        public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders)
        {
            base.ExtendModelInterfaces(extenders);
            extenders.Add<IModelListView, IModelListViewHidePopupMenu>();
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            if ((View).Editor is IPopupMenuHider)
                ((IPopupMenuHider)(View).Editor).HidePopupMenu = ((IModelListViewHidePopupMenu)View.Model).HidePopupMenu;
        }
    }
}