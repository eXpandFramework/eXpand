using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.HtmlPropertyEditor.Web;
using DevExpress.Utils;
using Xpand.Persistent.Base.General;
using Xpand.XAF.Modules.ModelMapper;
using Xpand.XAF.Modules.ModelMapper.Configuration;
using Xpand.XAF.Modules.ModelMapper.Services;

namespace Xpand.ExpressApp.HtmlPropertyEditor.Web {
    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(HtmlPropertyEditorAspNetModule), "Resources.Toolbox_Module_HtmlPropertyEditor_Web.bmp")]
    [ToolboxTabName(XpandAssemblyInfo.TabAspNetModules)]
    public sealed class XpandHtmlPropertyEditorAspNetModule : XpandModuleBase {
        public XpandHtmlPropertyEditorAspNetModule() {
            RequiredModuleTypes.Add(typeof(HtmlPropertyEditorAspNetModule));
            RequiredModuleTypes.Add(typeof(ModelMapperModule));
        }

        public override void Setup(ApplicationModulesManager moduleManager) {
            base.Setup(moduleManager);
            moduleManager.ExtendMap(PredefinedMap.ASPxHtmlEditor);
        }
    }
}