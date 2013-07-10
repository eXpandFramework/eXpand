using System;
using System.ComponentModel;
using System.Drawing;
using System.Net;
using System.Web;
using System.Web.Security;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web;
using DevExpress.Persistent.Base;
using DevExpress.Utils;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.Security.Web {
    public enum TicketExpiration {
        SessionTimeout,
        After1Day,
        After15Days,
        After1Month
    }

    public interface IOptionsAutomaticallyLogon:IModelNode {
        bool Enabled { get; set; }
        TicketExpiration TicketExpiration { get; set; }
    }
    public interface IModelOptionsAutomaticallyLogon {
        IOptionsAutomaticallyLogon AutomaticallyLogon { get; }
    }
    [ToolboxBitmap(typeof(SecurityModule), "Resources.BO_Security.ico")]
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabAspNetModules)]
    public sealed class XpandSecurityWebModule : XpandModuleBase{
        bool _automaticallyLogonEnabled;

        public XpandSecurityWebModule() {
            RequiredModuleTypes.Add(typeof(XpandSecurityModule));
        }
        public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelOptions, IModelOptionsAutomaticallyLogon>();
        }

        void OnCustomWriteSecuredLogonParameters(object sender, HandledEventArgs handledEventArgs) {
            handledEventArgs.Handled = true;
            if (HttpContext.Current == null) {
                Tracing.Tracer.LogWarning("Cannot add a Forms cookie to the Respose.Cookies collection: the HttpContext.Current property is null");
                return;
            }
            var logonParametersAsString = LogonParametersAsString();
            var cookie = HttpCookie(logonParametersAsString);
            HttpContext.Current.Response.Cookies.Add(cookie);

        }

        string LogonParametersAsString() {
            string logonParametersAsString = "";
            if (SecuritySystem.LogonParameters != null) {
                var parameters = SecuritySystem.LogonParameters as ISupportStringSerialization;
                if (parameters != null) {
                    logonParametersAsString = parameters.GetValuesAsString();
                }
                else {
                    var storage = new SettingsStorageOnString();
                    ObjectSerializer.WriteObjectPropertyValues(null, storage, SecuritySystem.LogonParameters);
                    logonParametersAsString = storage.GetContentAsString();
                }
            }
            return logonParametersAsString;
        }

        HttpCookie HttpCookie(string logonParametersAsString) {
            HttpCookie cookie = FormsAuthentication.GetAuthCookie("", _automaticallyLogonEnabled);
            FormsAuthenticationTicket formsTicket = FormsAuthentication.Decrypt(cookie.Value);
            if (formsTicket != null) {
                var encryptedXafTicket = EncryptedXafTicket(formsTicket, logonParametersAsString);
                if (encryptedXafTicket != null && encryptedXafTicket.Length < CookieContainer.DefaultCookieLengthLimit) {
                    cookie.Value = encryptedXafTicket;
                }
                else {
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

        int GetExpiration() {
            var ticketExpiration = ((IModelOptionsAutomaticallyLogon) Application.Model.Options).AutomaticallyLogon.TicketExpiration;
            switch (ticketExpiration) {
                case TicketExpiration.SessionTimeout:
                    return HttpContext.Current.Session.Timeout + 1;
                case TicketExpiration.After1Day:
                    return new TimeSpan(1, 0, 0, 0).Minutes;
                case TicketExpiration.After15Days:
                    return new TimeSpan(15, 0, 0, 0).Minutes;
                case TicketExpiration.After1Month:
                    return new TimeSpan(30, 0, 0, 0).Minutes;
            }
            throw new NotImplementedException();
        }

        protected override Type[] ApplicationTypes() {
            return new[] { typeof (IWriteSecuredLogonParameters) };
        }

        public override void Setup(ApplicationModulesManager moduleManager) {
            base.Setup(moduleManager);
            if (RuntimeMode)
                Application.SetupComplete+=ApplicationOnSetupComplete;
        }

        void ApplicationOnSetupComplete(object sender, EventArgs eventArgs) {
            _automaticallyLogonEnabled = ((IModelOptionsAutomaticallyLogon)Application.Model.Options).AutomaticallyLogon.Enabled || ((WebApplication)Application).CanAutomaticallyLogonWithStoredLogonParameters;
            if (_automaticallyLogonEnabled) {
                ((WebApplication) Application).CanAutomaticallyLogonWithStoredLogonParameters = true;
                ((IWriteSecuredLogonParameters)Application).CustomWriteSecuredLogonParameters += OnCustomWriteSecuredLogonParameters;
                
            }
        }
    }
}