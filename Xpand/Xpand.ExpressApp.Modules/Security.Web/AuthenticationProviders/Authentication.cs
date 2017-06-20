using System;
using System.Net;
using System.Security.Principal;
using System.Web;
using System.Web.Security;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web;
using DevExpress.Persistent.Base;
using Xpand.ExpressApp.Security.AuthenticationProviders;
using Xpand.ExpressApp.Security.Web.Controllers;

namespace Xpand.ExpressApp.Security.Web.AuthenticationProviders {

    public class Authentication {
        IModelAutoAthentication _autoAthentication;
        IModelAnonymousAuthentication _anonymousAuthentication;

        void ApplicationOnLoggingOn(object sender, LogonEventArgs logonEventArgs) {
            var webApplication = ((WebApplication)sender);
            webApplication.LoggingOn -= ApplicationOnLoggingOn;
            var logonParameters = logonEventArgs.LogonParameters as XpandLogonParameters;
            if (logonParameters != null && RememberMeViewItemExists(webApplication, logonParameters) && webApplication.CanAutomaticallyLogonWithStoredLogonParameters) {
                webApplication.CanAutomaticallyLogonWithStoredLogonParameters = logonParameters.RememberMe;
            }
        }

        bool RememberMeViewItemExists(WebApplication webApplication, object logonParameters) {
            var detailViewId = webApplication.FindDetailViewId(logonParameters.GetType());
            return ((IModelDetailView)webApplication.Model.Views[detailViewId]).Items["RememberMe"] != null;
        }

        void ApplicationOnSetupComplete(object sender, EventArgs eventArgs) {
            var webApplication = ((WebApplication) sender);
            webApplication.SetupComplete -= ApplicationOnSetupComplete;
            _autoAthentication = ((IModelOptionsAuthentication)webApplication.Model.Options).Athentication.AutoAthentication;
            _anonymousAuthentication = ((IModelOptionsAuthentication) webApplication.Model.Options).Athentication.AnonymousAuthentication;
            if (_autoAthentication.UseOnlySecuredStorage) {
                webApplication.LastLogonParametersWriting += ApplicationOnLastLogonParametersWriting;
            }
            if (AutomaticallyLogonEnabled(webApplication)) {
                webApplication.CanAutomaticallyLogonWithStoredLogonParameters = true;
                webApplication.LoggedOn+=WebApplicationOnLoggedOn;
            }
            if (_anonymousAuthentication.Enabled){
                webApplication.CanAutomaticallyLogonWithStoredLogonParameters = true;
                var cookie = HttpContext.Current.Request.Cookies.Get(AnonymousLogonController.CookieName);
                Tracing.Tracer.LogText($"cookie={cookie}");
                if (cookie == null) {
                    var authenticationBase = ((SecurityStrategyBase) SecuritySystem.Instance).Authentication;
                    authenticationBase.SetLogonParameters(new AuthenticationStandardLogonParameters(_anonymousAuthentication.AnonymousUser,null));
                    var httpCookie = HttpCookie(_anonymousAuthentication.AnonymousUser, false);
                                        
                    HttpContext.Current.Response.Cookies.Add(httpCookie);
                    AuthenticateThisRequest();
                }
                else{
                    webApplication.LoggedOn += (o, args) => { AnonymousLogonController.InvalidateAnonymousCookie(); };
                }
            }
        }

        private void AuthenticateThisRequest() {
            if (HttpContext.Current.User.Identity.IsAuthenticated) return;

            var name = FormsAuthentication.FormsCookieName;
            var cookie = HttpContext.Current.Response.Cookies[name];
            if (cookie != null) {
                var ticket = FormsAuthentication.Decrypt(cookie.Value);
                if (ticket != null && !ticket.Expired) {
                    string[] roles = (ticket.UserData ?? "").Split(',');
                    HttpContext.Current.User = new GenericPrincipal(new FormsIdentity(ticket), roles);
                }
            }
        }
        protected virtual bool CanReadSecuredLogonParameters() {
            return HttpContext.Current != null
                   && HttpContext.Current.User != null
                   && HttpContext.Current.User.Identity is FormsIdentity
                   && HttpContext.Current.User.Identity.IsAuthenticated
                   && ((FormsIdentity)HttpContext.Current.User.Identity).Ticket != null;
        }



        private void WebApplicationOnLoggedOn(object sender, LogonEventArgs logonEventArgs){
            var logonParameters = SecuritySystem.LogonParameters as XpandLogonParameters;
            if (logonParameters != null && logonParameters.RememberMe){
                var cookie = HttpCookie(SecuritySystem.CurrentUserName, ((WebApplication) sender).CanAutomaticallyLogonWithStoredLogonParameters);
                HttpContext.Current.Response.Cookies.Add(cookie);
            }
        }

        bool AutomaticallyLogonEnabled(WebApplication webApplication) {
            var b = _autoAthentication.Enabled || webApplication.CanAutomaticallyLogonWithStoredLogonParameters;
            if (b&&!(webApplication.Security.LogonParameters is XpandLogonParameters))
                throw new NotImplementedException("use XpandLogonParameters object in your authentication ");
            return b;
        }


        string LogonParametersAsString() {
            string logonParametersAsString = "";
            if (SecuritySystem.LogonParameters != null) {
                var storage = new SettingsStorageOnString();
                ObjectSerializer.WriteObjectPropertyValues(null, storage, SecuritySystem.LogonParameters);
                logonParametersAsString = storage.GetContentAsString();
            }
            return logonParametersAsString;
        }

        HttpCookie HttpCookie(string userName, bool createPersistentCookie) {
            var logonParametersAsString = LogonParametersAsString();
            var cookie = FormsAuthentication.GetAuthCookie(userName, createPersistentCookie,FormsAuthentication.FormsCookiePath);
            FormsAuthenticationTicket formsTicket = FormsAuthentication.Decrypt(cookie.Value);
            if (formsTicket != null) {
                var ticket = EncryptTicket(formsTicket, logonParametersAsString);
                
                if (ticket != null && ticket.Length < CookieContainer.DefaultCookieLengthLimit) {
                    cookie.Value = ticket;
                } else {
                    if (ticket != null)
                        Tracing.Tracer.LogWarning("Cannot cache a login information into a FormsAuthentication cookie: " +
                                                  "the result length is '" + ticket.Length +
                                                  "' bytes and it exceeds the maximum cookie length '" +
                                                  CookieContainer.DefaultCookieLengthLimit +
                                                  "' (see the 'System.Net.CookieContainer.DefaultCookieLengthLimit' property)");
                }
                return cookie;
            }
            throw new NotImplementedException();
        }

        string EncryptTicket(FormsAuthenticationTicket formsTicket, string logonParametersAsString) {
            DateTime ticketExpiration = formsTicket.Expiration;
            if (HttpContext.Current != null && HttpContext.Current.Session != null) {
                TimeSpan ticketTimeout = formsTicket.Expiration - formsTicket.IssueDate;
                if (HttpContext.Current.Session.Timeout > ticketTimeout.Minutes) {
                    Tracing.Tracer.LogWarning(
                        "The FormsAuthentication timeout is less than the ASP.NET Session timeout: '{0}' and '{1}'. The ASP.NET Session timeout is used for FormsAuthentication.",
                        ticketTimeout.Minutes, HttpContext.Current.Session.Timeout);
                    ticketExpiration = formsTicket.IssueDate.AddMinutes(GetExpiration());
                }
            }
            var xafTicket = new FormsAuthenticationTicket(
                formsTicket.Version, formsTicket.Name, formsTicket.IssueDate, ticketExpiration,
                formsTicket.IsPersistent, logonParametersAsString, formsTicket.CookiePath);
            return FormsAuthentication.Encrypt(xafTicket);
        }

        double GetExpiration() {
            var ticketExpiration = _autoAthentication.TicketExpiration;
            return ticketExpiration == 0
                       ? HttpContext.Current.Session.Timeout + 1
                       : new TimeSpan(ticketExpiration, 0, 0, 0).TotalMinutes;
        }
        public void Attach(ModuleBase securityModule) {
            var xafApplication = securityModule.Application;
            xafApplication.SetupComplete += ApplicationOnSetupComplete;
            xafApplication.LoggingOn += ApplicationOnLoggingOn;    
        }

        void ApplicationOnLastLogonParametersWriting(object sender, LastLogonParametersWritingEventArgs lastLogonParametersWritingEventArgs) {
            ((WebApplication) sender).LastLogonParametersWriting-=ApplicationOnLastLogonParametersWriting;
            lastLogonParametersWritingEventArgs.Handled = lastLogonParametersWritingEventArgs.SettingsStorage is SettingsStorageOnCookies;
        }
    }

}
