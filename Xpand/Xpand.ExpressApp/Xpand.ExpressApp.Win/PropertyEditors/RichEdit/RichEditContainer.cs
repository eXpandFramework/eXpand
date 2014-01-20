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

        public override string Text{
            get { return richEditControl1.Text; }
            set { richEditControl1.Text = value; }
        }

        public string RtfText{
            get { return RichEditControl.RtfText; }
            set { RichEditControl.RtfText = value; }
        }

        private void btnFullSizeEditor_ItemClick(object sender, ItemClickEventArgs e) {
            var fullEditor = new FullWinRichtEdit { RtfText = RtfText };
            fullEditor.ShowDialog();
            RtfText = fullEditor.RtfText;
        }
    }
}
