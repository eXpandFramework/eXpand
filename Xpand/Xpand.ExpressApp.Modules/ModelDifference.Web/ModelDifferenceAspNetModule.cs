using System.ComponentModel;
using System.Web;

namespace Xpand.ExpressApp.ModelDifference.Web {
    [ToolboxItemFilter("Xaf.Platform.Web")]
    public sealed class ModelDifferenceAspNetModule : ModelDifferenceBaseModule {
        private bool? persistentApplicationModelUpdated;

        public ModelDifferenceAspNetModule() {
            RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Web.SystemModule.SystemAspNetModule));
            RequiredModuleTypes.Add(typeof(ModelDifferenceModule));
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