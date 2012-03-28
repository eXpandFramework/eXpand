using System;
using System.Configuration;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration;
using Xpand.Persistent.Base.ExceptionHandling;

namespace Xpand.ExpressApp.ExceptionHandling {
    public abstract partial class ExceptionHandlingModule : XpandModuleBase {
        public const string ExceptionHandling = "ExceptionHandling";

        protected ExceptionHandlingModule() {
            InitializeComponent();
        }

        public override void Setup(XafApplication application) {
            base.Setup(application);
            if (RuntimeMode)
                AddToAdditionalExportedTypes("Xpand.Persistent.BaseImpl.ExceptionHandling");
        }
        protected void Log(Exception exception) {
            if (IsEnabled()) {
                if (EmailTraceListenersAreValid()) {
                    string asString = Tracing.Tracer.GetLastEntriesAsString();
                    Logger.Write(asString);
                }
                var connectionStringSettingsCollection = ConfigurationManager.ConnectionStrings;
                var connectionStringSettings = connectionStringSettingsCollection.OfType<ConnectionStringSettings>().Where(settings => settings.Name == "ExceptionHandlingConnectionString").FirstOrDefault();
                if (connectionStringSettings != null) {
                    if (XafTypesInfo.Instance.FindTypeInfo(typeof(IExceptionObject)).Implementors.Count() == 0) return;
                    var session = new Session(
                        new SimpleDataLayer(XpoDefault.GetConnectionProvider(connectionStringSettings.ConnectionString, AutoCreateOption.DatabaseAndSchema)));
                    var exceptionObject = ExceptionObjectBuilder.Create(session, exception, Application);
                    session.Save(exceptionObject);
                }
            }
        }

        bool EmailTraceListenersAreValid() {
            var loggingSettings = (LoggingSettings)ConfigurationManager.GetSection("loggingConfiguration");
            return loggingSettings.TraceListeners.OfType<EmailTraceListenerData>().Where(data => data.FromAddress == "user@domain.com" || data.SmtpServer == "mail.mail.com").FirstOrDefault() == null;
        }


        bool IsEnabled() {
            return (ConfigurationManager.AppSettings[ExceptionHandling] + "").ToLower() == "true";
        }
    }
}