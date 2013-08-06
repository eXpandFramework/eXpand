using System;
using System.ComponentModel;
using System.Net;
using System.Web;
using System.Web.Security;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web;
using DevExpress.Persistent.Base;
using Xpand.ExpressApp.Security.AuthenticationProviders;
using Xpand.Persistent.Base.General;

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
                ((IWriteSecuredLogonParameters)webApplication).CustomWriteSecuredLogonParameters += OnCustomWriteSecuredLogonParameters;
            }
            if (_anonymousAuthentication.Enabled) {
                var anomymousLogonParameters = webApplication.Security.LogonParameters as AnonymousLogonParameters;
                if (anomymousLogonParameters == null) {
                    throw new NotImplementedException(webApplication.Security+" LogonParameter is not of type " +typeof(AnonymousLogonParameters));
                }
                var anonymousAuthenticationStandard = ((SecurityStrategyBase) webApplication.Security).Authentication as AnonymousAuthenticationStandard;
                if (anonymousAuthenticationStandard == null) {
                    throw new NotImplementedException("Seucirty authentication is not of type " + typeof(AnonymousAuthenticationStandard));
                }
            }
        }

        bool AutomaticallyLogonEnabled(WebApplication webApplication) {
            var b = _autoAthentication.Enabled || webApplication.CanAutomaticallyLogonWithStoredLogonParameters;
            if (b&&!(webApplication.Security.LogonParameters is XpandLogonParameters))
                throw new NotImplementedException("use XpandLogonParameters object in your authentication ");
            return b;
        }


        void OnCustomWriteSecuredLogonParameters(object sender, HandledEventArgs handledEventArgs) {
            var webApplication = ((WebApplication) sender);
            handledEventArgs.Handled = true;
            if (HttpContext.Current == null) {
                Tracing.Tracer.LogWarning("Cannot add a Forms cookie to the Respose.Cookies collection: the HttpContext.Current property is null");
                return;
            }
            var logonParametersAsString = LogonParametersAsString();
            var cookie = HttpCookie(logonParametersAsString, webApplication);
            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        string LogonParametersAsString() {
            string logonParametersAsString = "";
            if (SecuritySystem.LogonParameters != null) {
                var parameters = SecuritySystem.LogonParameters as ISupportStringSerialization;
                if (parameters != null) {
                    logonParametersAsString = parameters.GetValuesAsString();
                } else {
                    var storage = new SettingsStorageOnString();
                    ObjectSerializer.WriteObjectPropertyValues(null, storage, SecuritySystem.LogonParameters);
                    logonParametersAsString = storage.GetContentAsString();
                }
            }
            return logonParametersAsString;
        }

        HttpCookie HttpCookie(string logonParametersAsString, WebApplication webApplication) {
            HttpCookie cookie = FormsAuthentication.GetAuthCookie("", webApplication.CanAutomaticallyLogonWithStoredLogonParameters);
            FormsAuthenticationTicket formsTicket = FormsAuthentication.Decrypt(cookie.Value);
            if (formsTicket != null) {
                var encryptedXafTicket = EncryptedXafTicket(formsTicket, logonParametersAsString);
                if (encryptedXafTicket != null && encryptedXafTicket.Length < CookieContainer.DefaultCookieLengthLimit) {
                    cookie.Value = encryptedXafTicket;
                } else {
                    if (encryptedXafTicket != null)
                        Tracing.Tracer.LogWarning("Cannot cache a login information into a FormsAuthentication cookie: " +
                                                  "the result length is '" + encryptedXafTicket.Length +
                                                  "' bytes and it exceeds the maximum cookie length '" +
                                                  CookieContainer.DefaultCookieLengthLimit +
                                                  "' (see the 'System.Net.CookieContainer.DefaultCookieLengthLimit' property)");
                }
            }
            return cookie;
        }

        string EncryptedXafTicket(FormsAuthenticationTicket formsTicket, string logonParametersAsString) {
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
            xafApplication.LastLogonParametersReading+=XafApplicationOnLastLogonParametersReading;
        }

        void XafApplicationOnLastLogonParametersReading(object sender, LastLogonParametersReadingEventArgs lastLogonParametersReadingEventArgs) {
//            if (((WebApplication) sender).CanAutomaticallyLogonWithStoredLogonParameters &&_anonymousAuthentication.Enabled) {
//                ((AnonymousLogonParameters)lastLogonParametersReadingEventArgs.LogonObject).WriteOption("AnonymousLogin", false.ToString());
//            }
        }

        void ApplicationOnLastLogonParametersWriting(object sender, LastLogonParametersWritingEventArgs lastLogonParametersWritingEventArgs) {
            ((WebApplication) sender).LastLogonParametersWriting-=ApplicationOnLastLogonParametersWriting;
            lastLogonParametersWritingEventArgs.Handled = lastLogonParametersWritingEventArgs.SettingsStorage is SettingsStorageOnCookies;
        }
    }

}
