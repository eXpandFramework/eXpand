using System;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.Web.ASPxEditors;

namespace eXpand.ExpressApp.Web.PropertyEditors{
    [PropertyEditor(typeof(Uri), true)]
    public class UriPropertyEditor : ASPxPropertyEditor{
        public UriPropertyEditor(Type objectType, DictionaryNode info)
            : base(objectType, info) {}

        protected override WebControl CreateViewModeControlCore(){
            ASPxHyperLink hlink = RenderHelper.CreateASPxHyperLink();
            return hlink;
        }

        protected override WebControl CreateEditModeControlCore(){
            ASPxTextBox textBox = RenderHelper.CreateASPxTextBox();
            textBox.TextChanged += ((sender, e) => WriteValue());
            return textBox;
        }

        protected override object GetControlValueCore()
        {
            return ((ASPxEditBase)Editor).Value;
        }

        protected override void ReadEditModeValueCore()
        {
            ((ASPxEditBase)Editor).Value = Convert.ToString(PropertyValue);
        }

        protected override void ReadViewModeValueCore()
        {
            ((ASPxHyperLink)InplaceViewModeEditor).NavigateUrl = Convert.ToString(PropertyValue);
            ((ASPxHyperLink)InplaceViewModeEditor).Text = Convert.ToString(PropertyValue);
        }

        protected override void WriteValueCore()
        {
            PropertyValue = new Uri(((ASPxEditBase)Editor).Value.ToString());
        }
    }
}