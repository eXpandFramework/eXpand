// Developer Express Code Central Example:
// How to show a hyper link (URL, email, etc.) for a business class property
// 
// In this example I have implemented two modules (HyperLinkPropertyEditor.Win and
// HyperLinkPropertyEditor.Web) containing custom PropertyEditors based on the
// HyperLinkEdit and ASPxHyperLink controls, respectively. Feel free to either use
// them "as is" in your solutions, or modify per your specific needs. Also track
// the corresponding suggestion below.
// 
// By default, the following basic
// functionality is implemented in the modules:
// 1. PropertyEditors, which can be
// used for representing object fields, containing email address or a URL.
// 2. To
// validate an input, a combined RexEx mask is used in both ListView and DetailView
// of Windows Forms and ASP.NET applications. The default regular expression is the
// following:
// (((http|https|ftp)\://)?[a-zA-Z0-9\-\.]+\.[a-zA-Z]{2,3}(:[a-zA-Z0-9]*)?/?([a-zA-Z0-9\-\._\?\,\'/\\\+&amp;amp;%\$#\=~])*)|([a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,6})
// You
// can use it as is or modify it per your specific needs in the
// HyperLinkPropertyEditor class. Look for Regular Expressions
// (http://msdn.microsoft.com/en-us/library/2k3te2cs%28VS.80%29.aspx) in MSDN for
// more information on how to do this.
// 3. The default email client or default
// browser window is opened after a single click on the hyper link if it represents
// a valid email or web address. For end-users convenience, in DetailView of
// Windows Forms projects, a double-click is necessary to be able to easily edit
// the field.
// 
// See Also:
// PropertyEditor Class
// (ms-help://DevExpress.Xaf/clsDevExpressExpressAppEditorsPropertyEditortopic.htm)
// Class
// HyperLinkEdit
// (ms-help://DevExpress.WindowsForms/clsDevExpressXtraEditorsHyperLinkEdittopic.htm)
// Class
// ASPxHyperLink
// (ms-help://DevExpress.AspNet/clsDevExpressWebASPxEditorsASPxHyperLinktopic.htm)
// Access
// Editor Settings
// (ms-help://DevExpress.Xaf/CustomDocument2729.htm)
// http://www.devexpress.com/scid=E1671
// http://www.devexpress.com/scid=S31007
// 
// You can find sample updates and versions for different programming languages here:
// http://www.devexpress.com/example=E2096

using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView;
using ListView = DevExpress.ExpressApp.ListView;

namespace Xpand.ExpressApp.Win.PropertyEditors {

    public class HyperLinkGridListViewController : ViewController {
        ColumnsListEditor gridListEditor;

        public HyperLinkGridListViewController() {
            TargetViewType = ViewType.ListView;
        }

        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            gridListEditor = ((ListView)View).Editor as ColumnsListEditor;
            if (gridListEditor != null) {
                GridView gridView = gridListEditor.GridView();
                if (gridView != null) gridView.MouseDown += GridView_MouseDown;
            }
        }

        protected override void OnDeactivated() {
            if (gridListEditor != null && gridListEditor.GridView() != null)
                gridListEditor.GridView().MouseDown -= GridView_MouseDown;
            base.OnDeactivated();
        }

        void GridView_MouseDown(object sender, MouseEventArgs e) {
            var gv = (GridView)sender;
            GridHitInfo hi = gv.CalcHitInfo(new Point(e.X, e.Y));
            if (hi.InRowCell) {
                var repositoryItemHyperLinkEdit = hi.Column.ColumnEdit as RepositoryItemHyperLinkEdit;
                if (repositoryItemHyperLinkEdit != null) {
                    var editor = (HyperLinkEdit)repositoryItemHyperLinkEdit.CreateEditor();
                    editor.ShowBrowser(
                        HyperLinkPropertyEditor.GetResolvedUrl(gv.GetRowCellValue(hi.RowHandle, hi.Column)));
                }
            }
        }
    }

    [PropertyEditor(typeof(String), "HyperLinkPropertyEditor", false)]
    public class HyperLinkPropertyEditor : StringPropertyEditor {
        public const string UrlEmailMask =
            @"(((http|https|ftp)\://)?[a-zA-Z0-9\-\.]+\.[a-zA-Z]{2,3}(:[a-zA-Z0-9]*)?/?([a-zA-Z0-9\-\._\?\,\'/\\\+&amp;amp;%\$#\=~])*)|([a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,6})";

        HyperLinkEdit hyperlinkEditCore;

        public HyperLinkPropertyEditor(Type objectType, IModelMemberViewItem info)
            : base(objectType, info) {
        }

        public new HyperLinkEdit Control {
            get { return hyperlinkEditCore; }
        }

        protected override RepositoryItem CreateRepositoryItem() {
            return new RepositoryItemHyperLinkEdit();
        }

        protected override object CreateControlCore() {
            hyperlinkEditCore = new HyperLinkEdit();
            return hyperlinkEditCore;
        }

        protected override void SetupRepositoryItem(RepositoryItem item) {
            base.SetupRepositoryItem(item);
            var hyperLinkProperties = (RepositoryItemHyperLinkEdit)item;
            hyperLinkProperties.SingleClick = View is ListView;
            hyperLinkProperties.TextEditStyle = TextEditStyles.Standard;
            hyperLinkProperties.OpenLink += hyperLinkProperties_OpenLink;
            EditMaskType = EditMaskType.RegEx;
            hyperLinkProperties.Mask.MaskType = MaskType.RegEx;
            hyperLinkProperties.Mask.EditMask = UrlEmailMask;
        }

        void hyperLinkProperties_OpenLink(object sender, OpenLinkEventArgs e) {
            e.EditValue = GetResolvedUrl(e.EditValue);
        }

        public static string GetResolvedUrl(object value) {
            string url = Convert.ToString(value);
            if (!string.IsNullOrEmpty(url)) {
                if (url.Contains("@") && IsValidUrl(url))
                    return string.Format("mailto:{0}", url);
                if (!url.Contains("://"))
                    url = string.Format("http://{0}", url);
                if (IsValidUrl(url))
                    return url;
            }
            return string.Empty;
        }

        static bool IsValidUrl(string url) {
            return Regex.IsMatch(url, UrlEmailMask);
        }
    }
}