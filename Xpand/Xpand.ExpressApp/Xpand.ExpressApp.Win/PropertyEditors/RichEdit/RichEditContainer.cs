using System.ComponentModel;
using DevExpress.XtraBars;
using DevExpress.XtraRichEdit;

namespace Xpand.ExpressApp.Win.PropertyEditors.RichEdit {
    public partial class RichEditContainer : DevExpress.XtraEditors.XtraUserControl {
        public RichEditContainer() {
            InitializeComponent();
        }

        public RichEditControl RichEditControl{
            get { return richEditControl1; }
        }

        public void HideToolBars(){
            foreach (Bar bar in barManager1.Bars){
                bar.Visible = false;
            }    
        }
        [Bindable(true)]
        public string ControlText{
            get { return richEditControl1.Text; }
            set { richEditControl1.Text = value; }
        }
        [Bindable(true)]
        public string RtfText{
            get { return RichEditControl.RtfText; }
            set { RichEditControl.RtfText = value; }
        }

        private void btnFullSizeEditor_ItemClick(object sender, ItemClickEventArgs e) {
            var fullEditor = new FullWinRichtEdit { RtfText = RtfText, Text = ControlText };
            fullEditor.ShowDialog();
            RtfText = fullEditor.RtfText;
            ControlText = fullEditor.Text;
        }
    }
}
