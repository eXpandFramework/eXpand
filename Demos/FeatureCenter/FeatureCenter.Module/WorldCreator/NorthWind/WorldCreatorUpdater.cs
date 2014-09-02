﻿using System.Configuration;
using System.IO;
using DevExpress.Xpo;
using Xpand.ExpressApp.IO.Core;
using Xpand.Persistent.BaseImpl.PersistentMetaData;

using Xpand.Xpo;

namespace FeatureCenter.Module.WorldCreator.NorthWind {
    public class WorldCreatorUpdater : Xpand.ExpressApp.WorldCreator.WorldCreatorUpdater {
        private const string NorthWind = "NorthWind";
        public WorldCreatorUpdater(Session session)
            : base(session) {
        }
        public override void Update() {
            if (Session.FindObject<PersistentAssemblyInfo>(info => info.Name == NorthWind) != null) return;

            var manifestResourceStream = GetType().Assembly.GetManifestResourceStream(GetType(), NorthWind + ".xml");
            if (manifestResourceStream != null) {
                string connectionString = ConfigurationManager.ConnectionStrings["NorthWind"].ConnectionString;
                var readToEnd = new StreamReader(manifestResourceStream).ReadToEnd().Replace(@"XpoProvider=MSSqlServer;data source=(local);integrated security=SSPI;initial catalog=Northwind", connectionString);
                new ImportEngine().ImportObjects(readToEnd, new UnitOfWork(Session.DataLayer));
            }

        }
    }
}