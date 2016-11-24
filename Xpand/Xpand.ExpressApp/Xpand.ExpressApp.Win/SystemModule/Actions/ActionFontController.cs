using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.ExpressApp.Win.Templates.ActionContainers;
using DevExpress.XtraEditors;
using Fasterflect;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.Win.SystemModule.Actions{
    public interface IModelActionLinkFont{
        IModelAppearanceFont Font { get;  }
    }
    public class ActionFontController : ViewController<DetailView>,IModelExtender{
        protected override void OnActivated() {
            base.OnActivated();
            foreach (WinActionContainerViewItem item in View.GetItems<WinActionContainerViewItem>()) {
                item.ControlCreated += item_ControlCreated;
            }
        }

        private void item_ControlCreated(object sender, EventArgs e){
            var viewItem = (WinActionContainerViewItem)(sender);
            ((ButtonsContainer)viewItem.Control).ActionItemAdding += (o, args) => ChangeFont(viewItem, args);
        }

        private void ChangeFont(WinActionContainerViewItem winActionContainerViewItem, ActionItemEventArgs e) {
            var buttonsContainersSimpleActionItem = e.Item as ButtonsContainersSimpleActionItem;
            if (buttonsContainersSimpleActionItem != null) {
                SimpleButton simpleButton = (buttonsContainersSimpleActionItem.Control);
                if (simpleButton != null) {
                    if (winActionContainerViewItem.Model.ActionContainer != null) {
                        var actionLink = (IModelActionLinkFont)winActionContainerViewItem.Model.ActionContainer.FirstOrDefault(link => link.ActionId == e.Item.Action.Id);
                        if (actionLink != null)
                            simpleButton.Font = GetFont(actionLink.Font, simpleButton.Font);
                    }
                }
            }
        }

        private Font GetFont(IModelAppearanceFont modelAppearanceFont,Font font){
            var fontBuilder = new FontBuilder(modelAppearanceFont,font);
            return fontBuilder.GetFont();
        }

        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders){
            extenders.Add<IModelActionLink,IModelActionLinkFont>();
        }
    }
}