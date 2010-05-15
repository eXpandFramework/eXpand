using System.Collections.Generic;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Win;
using DevExpress.ExpressApp.Win.Controls;
using DevExpress.XtraBars;
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
            foreach (var shortCut in _shortCuts) {
                var barSubItem = new BarSubItem
                {
                    Id = barManager.GetNewItemId(),
                    Caption = shortCut.Value.Id,
                    ItemShortcut = new BarShortcut(shortCut.Key)
                };        
                barSubItem.ItemClick += BarSubItemOnItemClick;
                rootMenu.AddItem(barSubItem);    
            }
            
        }

        BarSubItem GetRootMenu(BarManager barManager) {
            var rootMenu = new BarSubItem { Caption = "FocusShortcuts", Id = barManager.GetNewItemId(),Visibility = BarItemVisibility.Never};
            barManager.ProcessShortcutsWhenInvisible = true;
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
            ((Control) _shortCuts[itemClickEventArgs.Item.ItemShortcut.Key].Control).Focus();
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