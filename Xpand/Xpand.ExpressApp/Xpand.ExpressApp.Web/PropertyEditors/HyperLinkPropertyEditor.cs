using System;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.Web.ASPxEditors;
using EditorAliases = Xpand.Persistent.Base.General.EditorAliases;

namespace Xpand.ExpressApp.Web.PropertyEditors {
    [PropertyEditor(typeof(String), EditorAliases.HyperLinkPropertyEditor, false)]
    [CancelClickEventPropagation]
    public class HyperLinkPropertyEditor : ASPxPropertyEditor {
        public const string UrlEmailMask =
            @"(((http|https|ftp)\://)?[a-zA-Z0-9\-\.]+\.[a-zA-Z]{2,3}(:[a-zA-Z0-9]*)?/?([a-zA-Z0-9\-\._\?\,\'/\\\+&amp;amp;%\$#\=~])*)|([a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,6})";

        public HyperLinkPropertyEditor(Type objectType, IModelMemberViewItem info)
            : base(objectType, info) {
        }

        protected override WebControl CreateEditModeControlCore(){
            if (AllowEdit) {
                var textBox = RenderHelper.CreateASPxTextBox();
                textBox.MaxLength = MaxLength;
                textBox.ValidationSettings.RegularExpression.ValidationExpression = UrlEmailMask;
                textBox.TextChanged += ExtendedEditValueChangedHandler;
                return textBox;
            }
            return CreateHyperLink();
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