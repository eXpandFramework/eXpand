using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Web;
using Xpand.ExpressApp.Web.Layout;
using System.Linq;

namespace Xpand.ExpressApp.Web
{
    public class XpandWebWindow : WebWindow
    {
        public XpandWebWindow(XafApplication application, TemplateContext context, ICollection<Controller> controllers, bool isMain, bool activateControllersImmediatelly)
            : base(application, context, controllers, isMain, activateControllersImmediatelly)
        {

            ClientScripts.Add("XpandHelper", XpandLayoutManager.GetXpandHelperScript());
        }

        protected override void OnTemplateChanged()
        {
            base.OnTemplateChanged();
            Page page = Template as Page;
            if (page != null)
            {
                HtmlForm form = FindControlByType<HtmlForm>(page);
                if (form != null)
                {
                    ScriptManager scriptManager = new ScriptManager();
                    scriptManager.ID = "scriptManager";
                    form.Controls.AddAt(0, scriptManager);
                }
            }
        }

        public static CType FindControlByType<CType>(Control parent) where CType : Control
        {
            CType result = parent as CType;
            if (result != null) return result;
            return parent.Controls.Cast<Control>().Select(c => FindControlByType<CType>(c)).Where(c=>c!=null).FirstOrDefault();
        }
    }
}
