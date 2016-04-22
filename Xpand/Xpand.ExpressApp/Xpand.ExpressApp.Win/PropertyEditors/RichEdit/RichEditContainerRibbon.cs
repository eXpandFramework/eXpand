using System;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraRichEdit;
using Xpand.Persistent.Base.General.Win;

namespace Xpand.ExpressApp.Win.PropertyEditors.RichEdit {
    public partial class RichEditContainerRibbon : RichEditContainerBase {
        private RibbonForm _parentRibbonForm;

        public RichEditContainerRibbon() {
            InitializeComponent();
        }

        protected override void OnHandleCreated(EventArgs e){
            base.OnHandleCreated(e);
            _parentRibbonForm = ((RibbonForm) FindForm());
            if (_parentRibbonForm != null){
                ribbonControl1.Visible = false;
                if (_parentRibbonForm.MdiParent!=null){
                    _parentRibbonForm.Ribbon.Visible = false;
                    ribbonControl1.MdiMergeStyle = RibbonMdiMergeStyle.Never;
                    _parentRibbonForm.Activated+=ParentRibbonFormOnActivated;
                    _parentRibbonForm.Deactivate+=ParentRibbonFormOnDeactivate;
                }
            }
        }

        private void ParentRibbonFormOnDeactivate(object sender, EventArgs eventArgs){
            SetMergeStyle(RibbonMdiMergeStyle.Always);
        }

        private void ParentRibbonFormOnActivated(object sender, EventArgs eventArgs){
            if (richEditControl1.Focused)
                CreateToolBars();
        }

        public override void DestroyToolBar(){
            if (!ToolBarsAreHidden) {
                this.Execute(control => {
                    if (control.MergedRibbon!=null) {
                        control.UnMergeRibbon();
                        if (_parentRibbonForm.MdiParent != null){
                            ((RibbonForm) _parentRibbonForm.MdiParent).Ribbon.UnMergeRibbon();
                            SetMergeStyle(RibbonMdiMergeStyle.Always);
                        }
                    }
                });
            }
        }

        public override void CreateToolBars(){
            if (!ToolBarsAreHidden) {
                this.Execute(control =>{
                    SetMergeStyle(RibbonMdiMergeStyle.Never);
                    control.MergeRibbon(ribbonControl1);
                    if (_parentRibbonForm.MdiParent != null)
                        ((RibbonForm)_parentRibbonForm.MdiParent).Ribbon.MergeRibbon(control);
                });
            }
        }

        private void SetMergeStyle(RibbonMdiMergeStyle mergeStyle){
            if (_parentRibbonForm.MdiParent != null){
                ((RibbonForm) _parentRibbonForm.MdiParent).Ribbon.MdiMergeStyle = mergeStyle;
                _parentRibbonForm.Ribbon.MdiMergeStyle = mergeStyle;
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
