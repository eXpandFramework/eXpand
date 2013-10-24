using System.Collections.Specialized;
using System.Web;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Web;
using Xpand.ExpressApp.Web.FriendlyUrl;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.Security;

namespace Xpand.ExpressApp.Web {
    public sealed class XpandHttpRequestManager : DefaultHttpRequestManager, IHttpRequestManager {
        readonly FriendlyUrlHelper _friendlyUrlHelper;
        public XpandHttpRequestManager() {
            _friendlyUrlHelper=new FriendlyUrlHelper(this);
        }
        #region Implementation of IHttpRequestManager
        ViewShortcut IHttpRequestManager.GetViewShortcut(string shortcutString) {
            return _friendlyUrlHelper.GetViewShortCut(shortcutString);
        }

        #endregion

        public override ViewShortcut GetViewShortcut() {
            if (WebApplication.Instance.SupportsUserActivation()) {
                var ua = Request.QueryString["ua"];
                if (!string.IsNullOrEmpty(ua)) {
                    using (var objectSpace = WebApplication.Instance.CreateObjectSpace()) {
                        var name = ((IModelRegistrationActivation) ((IModelOptionsRegistration) WebApplication.Instance.Model.Options).Registration).ActivationIdMember.Name;
                        var findObject = objectSpace.FindObject(XpandModuleBase.UserType, CriteriaOperator.Parse(name + "='" + ua+"'"));
                        objectSpace.TypesInfo.FindTypeInfo(XpandModuleBase.UserType).FindMember("IsActive").SetValue(findObject,true);
                        objectSpace.CommitChanges();
                        HttpContext.Current.Response.Write("Activation successful!");
                        HttpContext.Current.Response.End();
                    }
                }
            }
            return base.GetViewShortcut();
        }
        public override void WriteShortcutTo(ViewShortcut currentShortcut, NameValueCollection queryString) {
            base.WriteShortcutTo(currentShortcut, queryString);
            _friendlyUrlHelper.WriteShortcutTo(currentShortcut, queryString);
        }

        
    }
}
