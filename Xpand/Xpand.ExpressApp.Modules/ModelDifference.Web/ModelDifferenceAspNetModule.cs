using System.ComponentModel;
using System.Web;

namespace Xpand.ExpressApp.ModelDifference.Web
{
    [ToolboxItemFilter("Xaf.Platform.Web")]
    public sealed class ModelDifferenceAspNetModule : ModelDifferenceBaseModule
    {
        private bool? persistentApplicationModelUpdated;

        public ModelDifferenceAspNetModule()
        {
            RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Web.SystemModule.SystemAspNetModule));
            RequiredModuleTypes.Add(typeof(ModelDifferenceModule));
        }

        protected override bool? PersistentApplicationModelUpdated{
            get{
                bool result;
                bool.TryParse(HttpContext.Current.Application["persistentApplicationModelUpdated"] + "", out result);
                persistentApplicationModelUpdated = result;
                return persistentApplicationModelUpdated;
            }
            set { HttpContext.Current.Application["persistentApplicationModelUpdated"] = value; }
        }

        public override string GetPath() {
            HttpRequest request = HttpContext.Current.Request;
            return request.MapPath(request.ApplicationPath);
        }
    }
}