using System.Linq;
using System.Web.UI;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web;
using DevExpress.Web;
using Xpand.ExpressApp.Model;
using Xpand.ExpressApp.Web.FriendlyUrl;
using Xpand.ExpressApp.Web.Model;
using Xpand.Persistent.Base.Security;

namespace Xpand.ExpressApp.Web.Services {

    public static class Extensions {
        public static IModelMemberViewItem Model(this GridViewDataColumn column,IModelListView modelListView){
            return !string.IsNullOrEmpty(column.UnboundExpression)? modelListView.Columns.OfType<IModelColumnUnbound>()
                    .FirstOrDefault(modelColumn => modelColumn.UnboundExpression == column.UnboundExpression)
                : (modelListView.Columns.FirstOrDefault(modelColumn => modelColumn.FieldName == column.FieldName)?? modelListView.Columns.FirstOrDefault(modelColumn => modelColumn.PropertyName == column.FieldName));
        }

        public static DefaultHttpRequestManager NewHttpRequestManager(this WebApplication application) {
            return (application.SupportsFriendlyUrl() || application.SupportsUserActivation() ||
                    application.SupportsQueryStringParameter())
                       ? new XpandHttpRequestManager()
                       : new DefaultHttpRequestManager();
        }

        public static bool SupportsUserActivation(this WebApplication application) {
            if (application.Model == null)
                return false;
            return application.Model.Options is IModelOptionsRegistration modelOptionsRegistration && modelOptionsRegistration.Registration.Enabled &&
                   ((IModelRegistrationActivation)modelOptionsRegistration.Registration).ActivationIdMember != null;
        }
        public static bool SupportsQueryStringParameter(this WebApplication application) {
            if (application.Model == null)
                return false;
            return application.Model.Options is IModelOptionsQueryStringParameter modelOptionsQueryStringParameter && modelOptionsQueryStringParameter.QueryStringParameters.Any();
        }

        public static bool SupportsFriendlyUrl(this WebApplication application) {
            if (application.Model == null)
                return false;
            return application.Model.Options is IModelOptionsFriendlyUrl {EnableFriendlyUrl: true};
        }

        public static TCType FindControlByType<TCType>(this Control parent) where TCType : Control {
            if (parent is TCType result) return result;
            return parent.Controls.Cast<Control>().Select(FindControlByType<TCType>).FirstOrDefault(c => c != null);
        }

    }
}