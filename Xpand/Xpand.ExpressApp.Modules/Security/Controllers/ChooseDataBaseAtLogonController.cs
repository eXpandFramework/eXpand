using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Validation;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.Security.Controllers {
    public interface IModelOptionsChooseDatabaseAtLogon {
        [Category(XpandSecurityModule.XpandSecurity)]
        bool ChooseDatabaseAtLogon { get; set; }
    }

    public interface IDBServerParameter {
        string DBServer { get; set; }
    }

    public class ChooseDatabaseAtLogonController : ObjectViewController<DetailView, IDBServerParameter>, IModelExtender {
        public const string DBServer = "DBServer";
        private static readonly string[] _dbServers;
        private const string LogonDBServer = "LogonDBServer";

        static ChooseDatabaseAtLogonController(){
            _dbServers = GetConnectionStringSettings().Select(GetDbServerName).ToArray();
        }

        protected override void OnActivated() {
            base.OnActivated();
            if (!_dbServers.Any())
                throw new Exception("No connectionstring with the "+LogonDBServer+" prefix found");
        }

        public static string GetDbServerName(ConnectionStringSettings settings) {
            return settings.Name.Replace(LogonDBServer, "");
        }

        protected override void OnFrameAssigned() {
            base.OnFrameAssigned();
            Active["Model"] = ((IModelOptionsChooseDatabaseAtLogon) Application.Model.Options).ChooseDatabaseAtLogon;
            if (Active["Model"]&&!Application.IsLoggedIn()) {
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
                Validator.RuleSet.Validate(ObjectSpace, logonEventArgs.LogonParameters, DBServer);
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
