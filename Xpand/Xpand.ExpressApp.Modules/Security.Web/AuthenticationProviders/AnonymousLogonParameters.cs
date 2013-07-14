using System;
using System.ComponentModel;
using System.Web;
using System.Web.Security;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web;
using DevExpress.Xpo;
using Xpand.ExpressApp.Security.AuthenticationProviders;

namespace Xpand.ExpressApp.Security.Web.AuthenticationProviders {
    [NonPersistent]
    public class AnonymousLogonParameters : XpandLogonParameters, ICustomObjectSerialize {
        static volatile string _anonymousUserName;

        [Browsable(false)]
        public bool AnonymousLogin { get; set; }
        
//        public new bool RememberMe { get; set; }

        [Browsable(false)]
        public string AnonymousUserName {
            get {return _anonymousUserName ??(_anonymousUserName =((IModelOptionsAuthentication) WebApplication.Instance.Model.Options).Athentication.AnonymousAuthentication.AnonymousUser);}
        }
        protected virtual bool CanReadSecuredLogonParameters() {
            return HttpContext.Current != null
                   && HttpContext.Current.User != null
                && HttpContext.Current.User.Identity is FormsIdentity
                && HttpContext.Current.User.Identity.IsAuthenticated
                && ((FormsIdentity)HttpContext.Current.User.Identity).Ticket != null;
        }

        void ICustomObjectSerialize.ReadPropertyValues(SettingsStorage storage) {
            ReadPropertyValuesCore(storage);
            AnonymousLogin = ReadBoolOption("AnonymousLogin", true);
            if (AnonymousLogin&&!CanReadSecuredLogonParameters()) {
                UserName = AnonymousUserName;
                Password = null;
            } else {
                AnonymousLogin = true;
            }
        }

        bool ReadBoolOption(string optionName, bool defaultValue) {
            var httpCookie = HttpContext.Current.Request.Cookies[WebApplication.Instance.ApplicationName];
            return httpCookie == null ? defaultValue : bool.Parse(httpCookie.Values[optionName]);
        }

        void ICustomObjectSerialize.WritePropertyValues(SettingsStorage storage) {
            WritePropertyValuesCore(storage);
            WriteOption("AnonymousLogin", AnonymousLogin.ToString());
        }

        void WriteOption(string optionName, string value) {
            var cookie = new HttpCookie(WebApplication.Instance.ApplicationName);
            cookie.Values.Add(optionName, value);
            cookie.Expires = DateTime.Now.AddSeconds(5);
            HttpContext.Current.Response.Cookies.Add(cookie);
        }
    }
}