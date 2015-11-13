using System;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Web;
using DevExpress.Persistent.Base;
using Fasterflect;
using Xpand.ExpressApp.Web.FriendlyUrl;
using Xpand.ExpressApp.Web.Model;

namespace Xpand.ExpressApp.Web {
    public sealed class XpandHttpRequestManager : DefaultHttpRequestManager, IHttpRequestManager {
        public const string Ua = "ua";
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
                var startIndex = queryString.IndexOf("#", StringComparison.Ordinal) + 1;
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
        
    }
}
