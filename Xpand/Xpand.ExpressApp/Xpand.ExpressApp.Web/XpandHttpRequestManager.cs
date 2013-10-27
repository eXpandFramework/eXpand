using System.Collections.Specialized;
using System.Linq;
using System.Web;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Web;
using DevExpress.Persistent.Base;
using Xpand.ExpressApp.Web.FriendlyUrl;
using Xpand.ExpressApp.Web.Model;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.Security;
using Fasterflect;

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
        public override string GetQueryString(ViewShortcut viewShortcut) {
            var queryString = base.GetQueryString(viewShortcut);
            if (WebApplication.Instance.SupportsQueryStringParameter()) {
                var startIndex = queryString.IndexOf("#", System.StringComparison.Ordinal) + 1;
                var substring = queryString.Substring(startIndex);
                var nameValueCollection = HttpUtility.ParseQueryString(substring);
                var queryStringParameters = ((IModelOptionsQueryStringParameter) WebApplication.Instance.Model.Options).QueryStringParameters;
                foreach (var queryStringParameter in queryStringParameters) {
                    var readOnlyParameter = (ReadOnlyParameter) queryStringParameter.ReadOnlyParameter.Type.CreateInstance();
                    nameValueCollection.Add(queryStringParameter.Key,readOnlyParameter.CurrentValue+"");
                }
                return queryString.Substring(0,startIndex)+ ToQueryString(nameValueCollection);
            }
            return queryString;
        }

        private string ToQueryString(NameValueCollection nvc) {
            var array = (nvc.AllKeys.SelectMany(nvc.GetValues,(key, value) =>string.Format("{0}={1}", HttpUtility.UrlEncode(key),HttpUtility.UrlEncode(value)))).ToArray();
            return string.Join("&", array);
        }

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
