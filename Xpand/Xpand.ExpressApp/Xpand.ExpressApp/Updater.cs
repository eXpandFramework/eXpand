using System;
using DevExpress.ExpressApp.Updating;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using System.Collections.Generic;
using DevExpress.Xpo.Helpers;

namespace Xpand.ExpressApp
{
    public class Updater : ModuleUpdater
    {
        public Updater(Session session, Version currentDBVersion) : base(session, currentDBVersion) { }
        public override void UpdateDatabaseBeforeUpdateSchema()
        {
            base.UpdateDatabaseBeforeUpdateSchema();
            if (this.CurrentDBVersion <= new Version(10, 0))
            {
                Dictionary<object, string> objectTypes = new Dictionary<object, string>();
                using (var reader = this.ExecuteReader("select [Oid], [TypeName] from [XPObjectType] where [TypeName] like 'expand.%'", false))
                {
                    while (reader.Read())
                    {
                        objectTypes.Add(reader[0], reader[1] as string);
                    }
                }

                foreach (var item in objectTypes)
                {
                    var type = ReflectionHelper.FindType(item.Value.Substring(1, item.Value.Length - 1));
                    this.ExecuteNonQueryCommand(String.Format("UPDATE [{0}] SET [{1}] = '{2}',  [{3}] = '{4}' WHERE [{5}] = {6}",
                        typeof(XPObjectType).Name,
                        XPObjectType.Fields.TypeName.PropertyName,
                        type.FullName,
                        XPObjectType.Fields.AssemblyName.PropertyName,
                        type.Assembly.ManifestModule.Name,
                        XPObjectType.Fields.Oid.PropertyName,
                        item.Key),
                        false);
                }

                var method = typeof(SimpleDataLayer).BaseType.GetMethod("ClearStaticData", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                var datalayer = XpandModuleBase.Application.ObjectSpaceProvider.CreateObjectSpace().Session.DataLayer;
                method.Invoke(datalayer, null);
            }
        }
    }
}
