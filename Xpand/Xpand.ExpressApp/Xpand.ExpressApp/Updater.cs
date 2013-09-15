using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using Fasterflect;

namespace Xpand.ExpressApp {
    public class Updater : ModuleUpdater {
        public Updater(IObjectSpace objectSpace, Version currentDBVersion)
            : base(objectSpace, currentDBVersion) {
        }

        public override void UpdateDatabaseBeforeUpdateSchema() {
            base.UpdateDatabaseBeforeUpdateSchema();
            if (CurrentDBVersion < new Version(10, 1, 6) && CurrentDBVersion > new Version(0, 0, 0, 0)) {

                try {
                    var objectTypes = new Dictionary<object, string>();
                    using (var reader = ExecuteReader("select [Oid], [TypeName] from [XPObjectType] where [TypeName] like 'expand.%'", true)) {
                        while (reader != null && reader.Read()) {
                            objectTypes.Add(reader[0], reader[1] as string);
                        }
                    }

                    foreach (var item in objectTypes) {
                        var type = ReflectionHelper.FindType(item.Value.Substring(1, item.Value.Length - 1));
                        ExecuteNonQueryCommand(String.Format("UPDATE [{0}] SET [{1}] = '{2}',  [{3}] = '{4}' WHERE [{5}] = {6}",
                                                             typeof(XPObjectType).Name,
                                                             XPObjectType.Fields.TypeName.PropertyName,
                                                             type.FullName,
                                                             XPObjectType.Fields.AssemblyName.PropertyName,
                                                             type.Assembly.ManifestModule.Name,
                                                             XPObjectType.Fields.Oid.PropertyName,
                                                             item.Key),
                                               false);
                    }

                    Type baseType = typeof(SimpleDataLayer).BaseType;
                    if (baseType != null) {
                        var datalayer = ((XPObjectSpace)ObjectSpace).Session.DataLayer;
                        datalayer.CallMethod("ClearStaticData");
                        var session = ((XPObjectSpace)ObjectSpace).Session;
                        session.DataLayer.CallMethod("ClearStaticData");
                        session.DropIdentityMap();
                    }
                } catch (Exception) {

                }
            }
        }
    }
}
