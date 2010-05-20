using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Win;
using DevExpress.ExpressApp.Win.Controls;
using DevExpress.ExpressApp.Win.Layout;
using DevExpress.XtraBars;
using DevExpress.XtraLayout;
using eXpand.ExpressApp.Core.DictionaryHelpers;
using ListView = DevExpress.ExpressApp.ListView;

namespace eXpand.ExpressApp.Win.SystemModule {
    public class FocusControlByShortcutController:ViewController<DetailView> {
        readonly Dictionary<Keys,DetailViewItem> _shortCuts = new Dictionary<Keys, DetailViewItem>();
        private const string FocusShortcut = "FocusShortcut";
        protected override void OnViewControlsCreated(){
            base.OnViewControlsCreated();
            var barManagerHolder = Frame.Template as IBarManagerHolder;
            if (barManagerHolder != null) {
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

        Func<KeyValuePair<Keys, DetailViewItem>, BarSubItem> GetBarItem(BarManager barManager) {
            return shortCut => new BarSubItem
            {
                Id = barManager.GetNewItemId(),
                Caption = shortCut.Value.Id,
                ItemShortcut = new BarShortcut(shortCut.Key)
            };
        }

        BarSubItem GetRootMenu(BarManager barManager) {
            var rootMenu = new BarSubItem { Caption = "FocusShortcuts", Id = barManager.GetNewItemId(),Visibility = BarItemVisibility.Never};
            barManager.ProcessShortcutsWhenInvisible = true;
            if (barManager.MainMenu== null)
                barManager.MainMenu=new Bar();
            barManager.MainMenu.AddItem(rootMenu);
            barManager.MainMenu.LinksPersistInfo.Add(new LinkPersistInfo(rootMenu));
            return rootMenu;
        }

        void FindShortcuts() {
            var detailViewItems = View.GetItems<DetailViewItem>();
            AddShortcuts(detailViewItems);
        }

        void AddShortcuts(IEnumerable<DetailViewItem> detailViewItems) {
            foreach (var detailViewItem in detailViewItems) {
                var shortcut = detailViewItem.Info.GetAttributeValue(FocusShortcut);
                if (!string.IsNullOrEmpty(shortcut))
                    _shortCuts.Add(ShortcutHelper.ParseBarShortcut(shortcut).Key,detailViewItem);
                if (detailViewItem is ListPropertyEditor ) {
                    var listView = ((ListView)((ListPropertyEditor)detailViewItem).Frame.View);
                    var listViewInfoNodeWrapper = listView.Model;
                    if (listViewInfoNodeWrapper.MasterDetailMode == MasterDetailMode.ListViewAndDetailView)
                        AddShortcuts(listView.EditView.GetItems<DetailViewItem>());
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

        public override Schema GetSchema()
        {
            var schemaBuilder = new SchemaBuilder();
            const string injectString = @"<Attribute Name=""" + FocusShortcut + @""" DefaultValueExpr=""{DevExpress.ExpressApp.Core.DictionaryHelpers.BOPropertyCalculator}ClassName=..\..\@ClassName""/>";
            var schema = new Schema(schemaBuilder.Inject(injectString, ModelElement.DetailViewPropertyEditors));
            schema.CombineWith(new Schema(schemaBuilder.Inject(@"<Attribute Name=""" + FocusShortcut + @""" />", ModelElement.Member)));
            return schema;
        }


    }
}