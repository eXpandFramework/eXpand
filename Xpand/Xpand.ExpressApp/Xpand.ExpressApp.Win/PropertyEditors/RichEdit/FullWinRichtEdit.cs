using DevExpress.XtraRichEdit;

namespace Xpand.ExpressApp.Win.PropertyEditors.RichEdit {
    public partial class FullWinRichtEdit : DevExpress.XtraBars.Ribbon.RibbonForm {
        public FullWinRichtEdit() {
            InitializeComponent();
        }

        public RichEditControl RichEditControl {
            get {
                return richEditControl1;
            }
        }

        public string RtfText {
            get {
                return richEditControl1.RtfText;
            }
            set {
                richEditControl1.RtfText = value;
            }
        }


    }
}