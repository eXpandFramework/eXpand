using System.Web;
using DevExpress.ExpressApp;

namespace eXpand.ExpressApp.WorldCreator.Web {
    public sealed partial class WorldCreatorWebModule : ModuleBase
    {
        public WorldCreatorWebModule()
        {
            InitializeComponent();
        }

//        protected override string GetPath() {
//            HttpRequest request = HttpContext.Current.Request;
//            return request.MapPath(request.ApplicationPath);
//        }
    }
}