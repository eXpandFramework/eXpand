using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win;
using DevExpress.ExpressApp.Win.Controls;
using DevExpress.ExpressApp.Win.Layout;
using DevExpress.XtraBars;
using DevExpress.XtraLayout;
using ListView = DevExpress.ExpressApp.ListView;

namespace Xpand.ExpressApp.Win.SystemModule {
    public interface IModelMemberFocusControlByShortcut : IModelNode {
        [Category("eXpand")]
        [Description("Focus associated editor when this keyboard shortcut is detected")]
        string FocusShortcut { get; set; }
    }
    [ModelInterfaceImplementor(typeof(IModelMemberFocusControlByShortcut), "ModelMember")]
    public interface IModelPropertyEditorFocusControlByShortcut : IModelMemberFocusControlByShortcut {
    }
    public class FocusControlByShortcutController : ViewController<DetailView>, IModelExtender {
        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelMember, IModelMemberFocusControlByShortcut>();
            extenders.Add<IModelPropertyEditor, IModelPropertyEditorFocusControlByShortcut>();
        }

        Dictionary<Keys, ViewItem> _shortCuts;

        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            _shortCuts = new Dictionary<Keys, ViewItem>();
            var barManagerHolder = Frame.Template as IBarManagerHolder;
            if (barManagerHolder != null && barManagerHolder.BarManager != null && barManagerHolder.BarManager.MainMenu != null) {
                var barManager = barManagerHolder.BarManager;
                BarSubItem rootMenu = GetRootMenu(barManager);
                FindShortcuts();
                CreateBarShortcuts(barManager, rootMenu);
            }
        }

        void CreateBarShortcuts(BarManager barManager, BarSubItem rootMenu) {
            foreach (var barSubItem in _shortCuts.Select(GetBarItem(barManager))) {
                barSubItem.ItemClick += BarSubItemOnItemClick;
                rootMenu.AddItem(barSubItem);
            }
        }

        Func<KeyValuePair<Keys, ViewItem>, BarSubItem> GetBarItem(BarManager barManager) {
            int newItemId = barManager.GetNewItemId();
            return shortCut => new BarSubItem {
                Id = newItemId,
                Name = newItemId.ToString(CultureInfo.InvariantCulture),
                Caption = shortCut.Value.Id,
                ItemShortcut = new BarShortcut(shortCut.Key)
            };
        }

        BarSubItem GetRootMenu(BarManager barManager) {
            var rootMenu = new BarSubItem { Name = "FocusShortCut", Caption = "FocusShortcuts", Id = barManager.GetNewItemId(), Visibility = BarItemVisibility.Never };
            barManager.ProcessShortcutsWhenInvisible = true;
            if (barManager.MainMenu != null) {
                barManager.MainMenu.AddItem(rootMenu);
                barManager.MainMenu.LinksPersistInfo.Add(new LinkPersistInfo(rootMenu));
            }
            return rootMenu;
        }

        void FindShortcuts() {
            var detailViewItems = View.GetItems<PropertyEditor>();
            AddShortcuts(detailViewItems);
        }

        void AddShortcuts(IEnumerable<PropertyEditor> propertyEditors) {
            foreach (PropertyEditor propertyEditor in propertyEditors) {
                var shortcut = ((IModelPropertyEditorFocusControlByShortcut)propertyEditor.Model).FocusShortcut;
                if (!string.IsNullOrEmpty(shortcut))
                    _shortCuts.Add(ShortcutHelper.ParseBarShortcut(shortcut).Key, propertyEditor);
                var listEditor = propertyEditor as ListPropertyEditor;
                if (listEditor != null && listEditor.Frame != null) {
                    var listView = ((ListView)((ListPropertyEditor)propertyEditor).Frame.View);
                    if (listView != null) {
                        var listViewInfoNodeWrapper = listView.Model;
                        if (listViewInfoNodeWrapper.MasterDetailMode == MasterDetailMode.ListViewAndDetailView)
                            AddShortcuts(listView.EditView.GetItems<PropertyEditor>());
                    }
                }
            }
        }

        void BarSubItemOnItemClick(object sender, ItemClickEventArgs itemClickEventArgs) {
            var control = ((Control)_shortCuts[itemClickEventArgs.Item.ItemShortcut.Key].Control);
            var layout = (LayoutControl)((WinLayoutManager)(View.LayoutManager)).Container;
            BaseLayoutItem layoutItem = layout.GetItemByControl(control);
            layout.FocusHelper.PlaceItemIntoView(layoutItem);
            control.Focus();
        }



    }
}