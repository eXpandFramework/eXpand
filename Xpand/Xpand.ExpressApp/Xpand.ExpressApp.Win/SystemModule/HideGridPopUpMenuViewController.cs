using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.Win.ListEditors;

namespace Xpand.ExpressApp.Win.SystemModule {
    public interface IModelClassHidePopupMenu : IModelNode {
        [Category("eXpand")]
        [Description("Hides the popup menu of a gridview")]
        bool HidePopupMenu { get; set; }
    }
    [ModelInterfaceImplementor(typeof(IModelClassHidePopupMenu), "ModelClass")]
    public interface IModelListViewHidePopupMenu : IModelClassHidePopupMenu {

    }

    public class HideGridPopUpMenuViewController : ViewController<ListView>, IModelExtender {
        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelClass, IModelClassHidePopupMenu>();
            extenders.Add<IModelListView, IModelListViewHidePopupMenu>();
        }

        protected override void OnActivated() {
            base.OnActivated();
            if ((View).Editor is IPopupMenuHider)
                ((IPopupMenuHider)(View).Editor).HidePopupMenu = ((IModelListViewHidePopupMenu)View.Model).HidePopupMenu;
        }
    }
}