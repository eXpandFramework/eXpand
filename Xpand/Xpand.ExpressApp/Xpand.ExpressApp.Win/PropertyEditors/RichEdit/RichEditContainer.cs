using DevExpress.XtraRichEdit;

namespace Xpand.ExpressApp.Win.PropertyEditors.RichEdit {
    public partial class RichEditContainer : DevExpress.XtraEditors.XtraUserControl {
        public RichEditContainer() {
            InitializeComponent();
        }

        public RichEditControl RichEditControl {
            get {
                return richEditControl1;
            }
        }

        public string RtfText {
            get {
                return RichEditControl.RtfText;
            }
            set {
                RichEditControl.RtfText = value;
            }
        }

        private void btnFullSizeEditor_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) {
            var fullEditor = new FullWinRichtEdit { RtfText = RtfText };
            fullEditor.ShowDialog();
            RtfText = fullEditor.RtfText;

        }
    }
}
