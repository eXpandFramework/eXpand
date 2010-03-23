using System.Web;

namespace eXpand.ExpressApp.WorldCreator.Web {
    public sealed partial class WorldCreatorWebModule : WorldCreatorModuleBase
    {
        public WorldCreatorWebModule()
        {
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