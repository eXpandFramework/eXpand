using System;
using System.ComponentModel;
using System.Drawing;
using System.Web;
using DevExpress.Utils;
using Xpand.ExpressApp.Web;

namespace Xpand.ExpressApp.ModelDifference.Web {
    [ToolboxBitmap(typeof(ModelDifferenceAspNetModule))]
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabAspNetModules)]
    public sealed class ModelDifferenceAspNetModule : ModelDifferenceBaseModule {
        private bool? persistentApplicationModelUpdated;

        public ModelDifferenceAspNetModule() {
            RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Web.SystemModule.SystemAspNetModule));
            RequiredModuleTypes.Add(typeof(ModelDifferenceModule));
        }

        protected override bool? ModelsLoaded {
            get {
                if (HttpContext.Current != null) {
                    bool result;
                    bool.TryParse(HttpContext.Current.Application["ModelsLoaded"] + "", out result);
                    persistentApplicationModelUpdated = result;
                }
                return persistentApplicationModelUpdated;
            }
            set { HttpContext.Current.Application["ModelsLoaded"] = value; }
        }

        public override string GetPath() {
            HttpRequest request = HttpContext.Current.Request;
            return request.MapPath(request.ApplicationPath);
        }

        #region Overrides of XpandModuleBase
        protected override Type[] ApplicationTypes() {
            return new[]{typeof(XpandWebApplication)};
        }
        #endregion
    }
}