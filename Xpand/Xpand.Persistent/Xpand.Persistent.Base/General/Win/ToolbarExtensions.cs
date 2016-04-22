using System;
using System.Windows.Forms;
using DevExpress.ExpressApp.Win;
using DevExpress.ExpressApp.Win.Controls;
using DevExpress.ExpressApp.Win.Templates;
using DevExpress.ExpressApp.Win.Templates.ActionContainers;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;

namespace Xpand.Persistent.Base.General.Win{
    public static class ToolbarExtensions{
        public static void Execute(this Control control, Action<BarManager> action){
            var form = control.FindForm();
            if (form != null && form.MdiParent != null)
                form = form.MdiParent;
            if (form != null) {
                var barManager = ((IBarManagerHolder) form).BarManager;
                if (barManager != null) {
                    barManager.BeginUpdate();
                    action(barManager);
                    barManager.EndUpdate();
                }
            }
        }

        public static void Execute(this Control control,Action<RibbonControl> action){
            if (((WinApplication)ApplicationHelper.Instance.Application).UseOldTemplates) {
                var xtraFormTemplateBase = (XtraFormTemplateBase)control.FindForm();
                if (xtraFormTemplateBase != null)
                    xtraFormTemplateBase.RibbonTransformer.Transformed += (o, args) =>
                        action(((ClassicToRibbonTransformer)o).Ribbon);
            }
            else {
                var ribbonForm = (RibbonForm)control.FindForm();
                if (ribbonForm != null) action(ribbonForm.Ribbon);
            }
        }
    }
}