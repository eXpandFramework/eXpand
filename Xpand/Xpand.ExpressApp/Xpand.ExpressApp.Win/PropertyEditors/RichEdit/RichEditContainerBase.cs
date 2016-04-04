using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;
using DevExpress.XtraRichEdit;

namespace Xpand.ExpressApp.Win.PropertyEditors.RichEdit{
    public class RichEditContainerBase:XtraUserControl{
        private XafApplication _application;
        public virtual RichEditControl RichEditControl{
            get { throw new System.NotImplementedException(); }
        }

        public XafApplication Application{
            get { return _application; }
        }

        [Bindable(true)]
        public string ControlText {
            get { return RichEditControl.Text; }
            set { RichEditControl.Text = value; }
        }
        [Bindable(true)]
        public string RtfText {
            get { return RichEditControl.RtfText; }
            set { RichEditControl.RtfText = value; }
        }
        public static RichEditContainerBase Create(RibbonFormStyle formStyle, XafApplication application){
            var richEditContainerBase = formStyle == RibbonFormStyle.Standard
                ? (RichEditContainerBase) new RichEditContainer()
                : new RichEditContainerRibbon();
            richEditContainerBase._application = application;
            return richEditContainerBase;
        }

        public virtual void HideToolBars(){
            throw new System.NotImplementedException();
        }
    }
}