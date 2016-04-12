using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.Controllers;

namespace Xpand.ExpressApp.Security.Controllers {
    public interface IModelOptionsChooseDatabaseAtLogon {
        [Category(XpandSecurityModule.XpandSecurity)]
        bool ChooseDatabaseAtLogon { get; set; }
    }

    public interface IDBServerParameter {
        string DBServer { get; set; }
    }

    class PopulateDBServerController : PopulateController<IDBServerParameter> {

        protected override string GetPredefinedValues(IModelMember wrapper) {
            var connectionStringSettingses = ChooseDatabaseAtLogonController.GetConnectionStringSettings();
            return string.Join(";", connectionStringSettingses.Select(ChooseDatabaseAtLogonController.GetDbServerName));
        }

        protected override Expression<Func<IDBServerParameter, object>> GetPropertyName() {
            return parameter => parameter.DBServer;
        }
    }

    public class ChooseDatabaseAtLogonController : ObjectViewController<DetailView, IDBServerParameter>, IModelExtender {
        private const string LogonDBServer = "LogonDBServer";

        protected override void OnActivated() {
            base.OnActivated();
            var dbServerParameter = ((IDBServerParameter)View.CurrentObject);
            if (string.IsNullOrEmpty(dbServerParameter.DBServer))
                dbServerParameter.DBServer = GetConnectionStringSettings().Select(GetDbServerName).First();
        }

        public static string GetDbServerName(ConnectionStringSettings settings) {
            return settings.Name.Replace(LogonDBServer, "");
        }

        protected override void OnFrameAssigned() {
            base.OnFrameAssigned();
            Frame.RegisterController(new PopulateDBServerController());
            if (!Application.IsLoggedIn()) {
                Application.LoggingOn += ApplicationOnLoggingOn;
                Frame.Disposing += FrameOnDisposing;
            }
        }

        private void FrameOnDisposing(object sender, EventArgs eventArgs) {
            Frame.Disposing -= FrameOnDisposing;
            Application.LoggingOn -= ApplicationOnLoggingOn;
        }

        public static IEnumerable<ConnectionStringSettings> GetConnectionStringSettings() {
            return ConfigurationManager.ConnectionStrings.Cast<ConnectionStringSettings>().Where(settings => settings.Name.StartsWith(LogonDBServer));
        }

        private void ApplicationOnLoggingOn(object sender, LogonEventArgs logonEventArgs) {
            var parameter = logonEventArgs.LogonParameters as IDBServerParameter;
            if (parameter != null) {
                var connectionString = GetConnectionStringSettings().First(settings
                    => GetDbServerName(settings) == parameter.DBServer).ConnectionString;
                Application.ConnectionString = connectionString;
                foreach (var provider in Application.ObjectSpaceProviders.OfType<XpandObjectSpaceProvider>().Select(provider => provider.DataStoreProvider).OfType<MultiDataStoreProvider>()) {
                    provider.ConnectionString = connectionString;
                }
                Application.TypesInfo.ModifySequenceObjectWhenMySqlDatalayer();
            }
        }

        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders){
            extenders.Add<IModelOptions,IModelOptionsChooseDatabaseAtLogon>();
        }
    }
}
