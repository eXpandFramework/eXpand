using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Web;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.FileAttachments.Web;
using DevExpress.Utils;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.WorldCreator.Web {
    [ToolboxBitmap(typeof(WorldCreatorWebModule))]
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabAspNetModules)]
    public sealed class WorldCreatorWebModule : WorldCreatorModuleBase {
        public WorldCreatorWebModule() {
            RequiredModuleTypes.Add(typeof(FileAttachmentsAspNetModule));
            RequiredModuleTypes.Add(typeof(Security.Web.XpandSecurityWebModule));
            RequiredModuleTypes.Add(typeof(WorldCreatorModule));
            RequiredModuleTypes.Add(typeof(ConditionalAppearanceModule));
        }

        public override string GetPath() {
            if (HttpContext.Current != null) {
                HttpRequest request = HttpContext.Current.Request;
                return request.MapPath(request.ApplicationPath);
            }
            if (Application.IsHosted()){
                return Path.GetDirectoryName(AppDomain.CurrentDomain.SetupInformation.ApplicationBase);
            }
            return null;
        }
    }
}