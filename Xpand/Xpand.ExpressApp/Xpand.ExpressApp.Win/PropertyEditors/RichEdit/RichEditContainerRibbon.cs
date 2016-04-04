using System;
using System.Linq;
using DevExpress.XtraBars;
using DevExpress.XtraRichEdit;
using Xpand.Persistent.Base.General.Win;

namespace Xpand.ExpressApp.Win.PropertyEditors.RichEdit {
    public partial class RichEditContainerRibbon : RichEditContainerBase {

        public RichEditContainerRibbon() {
            InitializeComponent();
            Load += OnLoad;
        }

        private void OnLoad(object sender, EventArgs e) {
            this.Execute(control => control.MergeRibbon(ribbonControl1));
        }

        public override void HideToolBars(){
            foreach (var bar in ribbonControl1.Categories.Cast<BarManagerCategory>()){
                bar.Visible = false;
            }    
        }

        public override RichEditControl RichEditControl{
            get { return richEditControl1; }
        }
    }
}
