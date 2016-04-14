using DevExpress.XtraRichEdit;
using Xpand.Persistent.Base.General.Win;

namespace Xpand.ExpressApp.Win.PropertyEditors.RichEdit {
    public partial class RichEditContainerRibbon : RichEditContainerBase {

        public RichEditContainerRibbon() {
            InitializeComponent();
            ribbonControl1.Visible = false;
        }

        public override void DestroyToolBar(){
            if (!ToolBarsAreHidden) {
                this.Execute(control => {
                    control.UnMergeRibbon();
                    ribbonControl1.Visible = false;
                });
            }
        }

        public override void CreateToolBars(){
            if (!ToolBarsAreHidden) {
                this.Execute(control => {
                    ribbonControl1.Visible = true;
                    control.MergeRibbon(ribbonControl1);
                });
            }
        }

        public override void HideToolBars(){
            base.HideToolBars();
            ribbonControl1.Visible = false;
        }

        public override RichEditControl RichEditControl{
            get { return richEditControl1; }
        }
    }
}
