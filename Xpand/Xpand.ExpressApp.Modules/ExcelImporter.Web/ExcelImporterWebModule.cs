using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.FileAttachments.Web;
using DevExpress.ExpressApp.Validation;
using DevExpress.ExpressApp.Validation.Web;
using DevExpress.ExpressApp.Web.SystemModule;
using DevExpress.Utils;
using Xpand.ExpressApp.Web.SystemModule;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.ExcelImporter.Web {
    [ToolboxBitmap(typeof(ExcelImporterWebModule))]
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabAspNetModules)]
    public sealed class ExcelImporterWebModule : XpandModuleBase {
        public ExcelImporterWebModule() {
            RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.SystemModule.SystemModule));
            RequiredModuleTypes.Add(typeof(ConditionalAppearanceModule));
            RequiredModuleTypes.Add(typeof(ValidationModule));
            RequiredModuleTypes.Add(typeof(ValidationAspNetModule));
            RequiredModuleTypes.Add(typeof(FileAttachmentsAspNetModule));
            RequiredModuleTypes.Add(typeof(SystemAspNetModule));
            RequiredModuleTypes.Add(typeof(ExcelImporterModule));
            RequiredModuleTypes.Add(typeof(XpandSystemAspNetModule));
        }

    }
}
