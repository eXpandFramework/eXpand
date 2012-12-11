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
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.Web.ASPxEditors;

namespace Xpand.ExpressApp.Web.PropertyEditors {
    [PropertyEditor(typeof(String), "HyperLinkPropertyEditor", false)]
    [CancelClickEventPropagation]
    public class HyperLinkPropertyEditor : DevExpress.ExpressApp.Web.Editors.ASPx.ASPxPropertyEditor {
        public const string UrlEmailMask =
            @"(((http|https|ftp)\://)?[a-zA-Z0-9\-\.]+\.[a-zA-Z]{2,3}(:[a-zA-Z0-9]*)?/?([a-zA-Z0-9\-\._\?\,\'/\\\+&amp;amp;%\$#\=~])*)|([a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,6})";

        public HyperLinkPropertyEditor(Type objectType, IModelMemberViewItem info)
            : base(objectType, info) {
        }

        protected override WebControl CreateEditModeControlCore() {

            if (AllowEdit) {
                var textBox = RenderHelper.CreateASPxTextBox();
                textBox.MaxLength = MaxLength;
                textBox.ValidationSettings.RegularExpression.ValidationExpression = UrlEmailMask;
                textBox.TextChanged += ExtendedEditValueChangedHandler;
                return textBox;
            }
            else {
                return CreateHyperLink();
            }
        }


        protected override void ApplyReadOnly() {
            if (Editor is ASPxTextBox)
                base.ApplyReadOnly();
        }

        protected override void ReadEditModeValueCore() {
            base.ReadEditModeValueCore();
            SetupHyperLink(PropertyValue, Editor);
        }
        protected override WebControl CreateViewModeControlCore() {
            return CreateHyperLink();
        }

        protected override void ReadViewModeValueCore() {
            base.ReadViewModeValueCore();
            SetupHyperLink(PropertyValue, InplaceViewModeEditor);
        }

        static string GetResolvedUrl(object value) {
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

        ASPxHyperLink CreateHyperLink() {
            ASPxHyperLink hyperlink = RenderHelper.CreateASPxHyperLink();
            return hyperlink;
        }

        void SetupHyperLink(object value, object editor) {
            
            var hyperlink = editor as ASPxHyperLink;
            if (hyperlink != null) {
                string url = Convert.ToString(value);
                hyperlink.Text = url;
                hyperlink.NavigateUrl = GetResolvedUrl(url);
                hyperlink.Target = "blank";
            }
        }
    }
}