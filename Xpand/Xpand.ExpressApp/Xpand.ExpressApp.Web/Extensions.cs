using System.Linq;
using System.Web.UI;
using DevExpress.ExpressApp.Web;
using Xpand.ExpressApp.Web.FriendlyUrl;
using Xpand.Persistent.Base.Security;

namespace Xpand.ExpressApp.Web {

    public static class Extensions {
        public static DefaultHttpRequestManager NewHttpRequestManager(this WebApplication application) {
            return application.SupportsFriendlyUrl() || application.SupportsUserActivation()
                       ? new XpandHttpRequestManager()
                       : new DefaultHttpRequestManager();
        }

        public static bool SupportsUserActivation(this WebApplication application) {
            var modelOptionsRegistration = application.Model.Options as IModelOptionsRegistration;
            return modelOptionsRegistration != null && modelOptionsRegistration.Registration.Enabled &&
                   ((IModelRegistrationActivation)modelOptionsRegistration.Registration).ActivationIdMember != null;
        }

        public static bool SupportsFriendlyUrl(this WebApplication application)  {
            if (application.Model == null)
                return false;
            var modelOptionsFriendlyUrl = application.Model.Options as IModelOptionsFriendlyUrl;
            return modelOptionsFriendlyUrl != null && modelOptionsFriendlyUrl.EnableFriendlyUrl;
        }

        public static CType FindControlByType<CType>(this Control parent) where CType : Control {
            var result = parent as CType;
            if (result != null) return result;
            return parent.Controls.Cast<Control>().Select(FindControlByType<CType>).FirstOrDefault(c => c != null);
        }

    }
}