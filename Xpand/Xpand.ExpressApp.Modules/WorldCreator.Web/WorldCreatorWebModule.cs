using System.ComponentModel;
using System.Drawing;
using System.Web;

namespace Xpand.ExpressApp.WorldCreator.Web {
    [ToolboxBitmap(typeof(WorldCreatorWebModule))]
    [ToolboxItem(true)]
    public sealed partial class WorldCreatorWebModule : WorldCreatorModuleBase {
        public WorldCreatorWebModule() {
            InitializeComponent();
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