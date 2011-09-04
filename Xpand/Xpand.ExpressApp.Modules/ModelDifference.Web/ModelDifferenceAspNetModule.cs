using System.ComponentModel;
using System.Drawing;
using System.Web;

namespace Xpand.ExpressApp.ModelDifference.Web {
    [ToolboxBitmap(typeof(ModelDifferenceAspNetModule))]
    [ToolboxItem(true)]
    public sealed class ModelDifferenceAspNetModule : ModelDifferenceBaseModule {
        private bool? persistentApplicationModelUpdated;

        public ModelDifferenceAspNetModule() {
            RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Web.SystemModule.SystemAspNetModule));
            RequiredModuleTypes.Add(typeof(ModelDifferenceModule));
            RequiredModuleTypes.Add(typeof(ExpressApp.Web.SystemModule.XpandSystemAspNetModule));
        }

        public override bool? ModelsLoaded {
            get {
                bool result;
                bool.TryParse(HttpContext.Current.Application["ModelsLoaded"] + "", out result);
                persistentApplicationModelUpdated = result;
                return persistentApplicationModelUpdated;
            }
            set { HttpContext.Current.Application["ModelsLoaded"] = value; }
        }

        public override string GetPath() {
            HttpRequest request = HttpContext.Current.Request;
            return request.MapPath(request.ApplicationPath);
        }
    }
}