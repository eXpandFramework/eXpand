using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.Web;
using Xpand.Extensions.XAF.ModelExtensions;
using Xpand.XAF.Modules.ModelMapper.Services.Predefined;
using EditorAliases = Xpand.Persistent.Base.General.EditorAliases;

namespace Xpand.ExpressApp.Web.PropertyEditors {

    [PropertyEditor(typeof(String), EditorAliases.HyperLinkPropertyEditor, false)]
    public class HyperLinkPropertyEditor : ASPxPropertyEditor {
        private bool _cancelClickEventPropagation;

		public const string UrlEmailMask =
			@"(((http|https|ftp)\://)?[a-zA-Z0-9\-\.]+\.[a-zA-Z]{2,3}(:[a-zA-Z0-9]*)?/?([a-zA-Z0-9\-\._\?\,\'/\\\+&amp;%\$#\=~])*)|([a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,100})";

		public HyperLinkPropertyEditor(Type objectType, IModelMemberViewItem info)
            : base(objectType, info){
            _cancelClickEventPropagation = true;
        }

        public override bool CancelClickEventPropagation{
            get => _cancelClickEventPropagation;
            set => _cancelClickEventPropagation = value;
        }

        protected override WebControl CreateEditModeControlCore(){
            if (AllowEdit) {
                var textBox = RenderHelper.CreateASPxTextBox();
                textBox.MaxLength = MaxLength;
                var validationSettings = textBox.ValidationSettings;
                validationSettings.ValidateOnLeave = true;
                validationSettings.CausesValidation = true;
                validationSettings.RegularExpression.ValidationExpression = UrlEmailMask;
                textBox.TextChanged += EditValueChangedHandler;
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
            SetupHyperLink(Editor);
        }

        protected override WebControl CreateViewModeControlCore() {
            return CreateHyperLink();
        }

        protected override void ReadViewModeValueCore() {
            base.ReadViewModeValueCore();    
            SetupHyperLink(InplaceViewModeEditor);
        }

        string GetResolvedUrl(object value, IModelASPxHyperLinkControl modelASPxHyperLinkControl) {
            string url = Convert.ToString(value);
            if (!string.IsNullOrEmpty(url)){
                var hyperLinkFormat = modelASPxHyperLinkControl?.HyperLinkFormat;
                if (string.IsNullOrEmpty(hyperLinkFormat)){
                    if (url.Contains("@") && IsValidUrl(url))
                        return $"mailto:{url}";
                    if (!url.Contains("://"))
                        url = $"http://{url}";
                    if (IsValidUrl(url))
                        return url;
                }
                else{
                    return string.Format(hyperLinkFormat, url);
                }
            }
            return string.Empty;
        }

        static bool IsValidUrl(string url) {
            return Regex.IsMatch(url, UrlEmailMask);
        }

        protected override string GetPropertyDisplayValue(){
            return GetFormattedValue();
        }


        ASPxHyperLink CreateHyperLink() {
            return RenderHelper.CreateASPxHyperLink();
        }

        public override bool CanFormatPropertyValue => true;

        void SetupHyperLink(object editor) {
            if (editor is ASPxHyperLink hyperlink) {
                string url = GetFormattedValue();
                hyperlink.Text = url;
                var modelASPxHyperLinkControl = Model
                    .GetNode(ViewItemService.PropertyEditorControlMapName)?.Nodes()
                    .OfType<IModelASPxHyperLinkControl>().FirstOrDefault();
                hyperlink.NavigateUrl = GetResolvedUrl(PropertyValue,modelASPxHyperLinkControl);
                var hyperlinkTarget = modelASPxHyperLinkControl?.GetValue<string>(nameof(ASPxHyperLink.Target));
                if (string.IsNullOrEmpty(hyperlinkTarget))
                    hyperlinkTarget = "_blank";
                hyperlink.Target = hyperlinkTarget;
            }
        }
    }
}
