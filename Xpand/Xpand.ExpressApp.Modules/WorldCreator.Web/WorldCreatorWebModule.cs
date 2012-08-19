using System;
using System.ComponentModel;
using System.Drawing;
using System.Web;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.FileAttachments.Web;

namespace Xpand.ExpressApp.WorldCreator.Web {
    [ToolboxBitmap(typeof(WorldCreatorWebModule))]
    [ToolboxItem(true)]
    public sealed class WorldCreatorWebModule : WorldCreatorModuleBase {
        public WorldCreatorWebModule() {
            RequiredModuleTypes.Add(typeof(FileAttachmentsAspNetModule));
            RequiredModuleTypes.Add(typeof(WorldCreatorModule));
            RequiredModuleTypes.Add(typeof(ConditionalAppearanceModule));
        }


        public override string GetPath() {
            if (HttpContext.Current != null) {
                HttpRequest request = HttpContext.Current.Request;
                return request.MapPath(request.ApplicationPath);
            }
            return null;
        }
    }
}