using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.ReportsV2;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.BaseImpl;
using MainDemo.Module.BusinessObjects;
using MainDemo.Module.Reports;

namespace MainDemo.Module {
    public sealed partial class MainDemoModule : ModuleBase {
        public MainDemoModule() {
            InitializeComponent();
            BaseObject.OidInitializationMode = OidInitializationMode.AfterConstruction;
        }

        public override void CustomizeTypesInfo(ITypesInfo typesInfo) {
            base.CustomizeTypesInfo(typesInfo);
            CalculatedPersistentAliasHelper.CustomizeTypesInfo(typesInfo);
        }

        public static string GetXpandDllPath(string path) {
            while (Directory.GetDirectories(path).All(s => !string.Equals((new DirectoryInfo(s).Name + ""), "xpand.dll", StringComparison.OrdinalIgnoreCase))) {
                path = Path.GetFullPath(path + @"..\");
            }
            path += @"Xpand.dll";
            return path;
        }

        public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
            ModuleUpdater updater = new DatabaseUpdate.Updater(objectSpace, versionFromDB);
            PredefinedReportsUpdater predefinedReportsUpdater = new PredefinedReportsUpdater(Application, objectSpace, versionFromDB);
            predefinedReportsUpdater.AddPredefinedReport<ContactsReport>("Contacts Report", typeof(Contact), true);
            return new[] { updater, predefinedReportsUpdater };
        }
        static MainDemoModule() {
            /*Note that you can specify the required format in a configuration file:
            <appSettings>
               <add key="FullAddressFormat" value="{Country.Name} {City} {Street}">
               <add key="FullAddressPersistentAlias" value="Country.Name+City+Street">
               ...
            </appSettings>

            ... and set the specified format here in code:
            Address.SetFullAddressFormat(ConfigurationManager.AppSettings["FullAddressFormat"], ConfigurationManager.AppSettings["FullAddressPersistentAlias"]);
            */

            Person.SetFullNameFormat("{LastName} {FirstName} {MiddleName}", "concat(FirstName, MiddleName, LastName)");
            Address.SetFullAddressFormat("City: {City}, Street: {Street}", "concat(City, Street)");
			ResetViewSettingsController.DefaultAllowRecreateView = false;
		}
    }
}
