using System;
using System.Windows.Forms;
using DevExpress.ExpressApp.Win;
using DevExpress.ExpressApp.Win.Templates;
using DevExpress.ExpressApp.Win.Templates.ActionContainers;
using DevExpress.XtraBars.Ribbon;

namespace Xpand.Persistent.Base.General.Win{
    public static class RibbonExtensions{
        public static void Execute(this Control control,Action<RibbonControl> action){
            if (((WinApplication)ApplicationHelper.Instance.Application).UseOldTemplates) {
                var xtraFormTemplateBase = (XtraFormTemplateBase)control.FindForm();
                if (xtraFormTemplateBase != null)
                    xtraFormTemplateBase.RibbonTransformer.Transformed += (o, args) =>
                        action(((ClassicToRibbonTransformer)o).Ribbon);
            }
            else {
                var detailRibbonFormV2 = (RibbonForm)control.FindForm();
                if (detailRibbonFormV2 != null)
                    action(detailRibbonFormV2.Ribbon);
            }
        }
    }
}