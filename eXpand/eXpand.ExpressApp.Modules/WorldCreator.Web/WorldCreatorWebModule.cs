using System.Web;

namespace eXpand.ExpressApp.WorldCreator.Web {
    public sealed partial class WorldCreatorWebModule : WorldCreatorModuleBase
    {
        public WorldCreatorWebModule()
        {
            InitializeComponent();
        }

        protected override string GetPath() {
            HttpRequest request = HttpContext.Current.Request;
            return request.MapPath(request.ApplicationPath);
        }
    }
}