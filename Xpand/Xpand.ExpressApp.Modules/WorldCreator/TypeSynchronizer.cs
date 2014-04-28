using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using Xpand.Persistent.Base.General;
using Xpand.Xpo;
using Xpand.Xpo.DB;

namespace Xpand.ExpressApp.WorldCreator {
    public class TypeSynchronizer {
        void SynchronizeTypesCore(XPObjectType xpObjectType, Session session) {
            if (session.FindObject<XPObjectType>(objectType => objectType.TypeName == xpObjectType.TypeName) == null) {
                var objectType = new XPObjectType(session, xpObjectType.AssemblyName, xpObjectType.TypeName);
                session.Save(objectType);
            }
        }

        public void SynchronizeTypes(string connectionString) {
            using (var session = new Session { ConnectionString = connectionString }) {
                using (var types = new XPCollection<XPObjectType>(session)) {
                    IEnumerable<XPObjectType> xpObjectTypes = types.Where(objectType => FindType(objectType.TypeName) != null).ToList();
                    var sqlMultiDataStoreProxy = new MultiDataStoreProxy(connectionString);
                    foreach (var connstring in GetConnectionStrings(sqlMultiDataStoreProxy.DataStoreManager, xpObjectTypes, connectionString)) {
                        if (connstring != connectionString) {
                            SynchronizeTypes(connstring, xpObjectTypes);
                        }
                    }
                }
            }
        }

        private static Type FindType(string name){
            Type type = null;
            try{
                type = ReflectionHelper.FindType(name);
            }
            catch (ReflectionTypeLoadException e){
                Tracing.Tracer.LogError(e);
            }
            return type;
        }

        public IEnumerable<string> GetConnectionStrings(DataStoreManager dataStoreManager, IEnumerable<XPObjectType> xpObjectTypes,
                                                        string exculdeString) {
            return xpObjectTypes.Select(type => dataStoreManager.GetConnectionString(FindType(type.TypeName))).Distinct().Where(s => s != exculdeString);
        }

        void SynchronizeTypes(string connectionString, IEnumerable<XPObjectType> xpObjectTypes) {
            var sqlDataStoreProxy = new DataStoreProxy(connectionString);
            using (var simpleDataLayer = new SimpleDataLayer(sqlDataStoreProxy)) {
                using (var session = new Session(simpleDataLayer)) {
                    var xpoObjectHacker = new XpoObjectHacker();
                    bool sync = false;
                    int[] oid = { 0 };
                    sqlDataStoreProxy.DataStoreUpdateSchema += EnsureIsNotIdentity(xpoObjectHacker);
                    sqlDataStoreProxy.DataStoreModifyData += (sender, args) => {
                        var insertStatement = args.ModificationStatements.OfType<InsertStatement>().SingleOrDefault(statement => statement.TableName == typeof(XPObjectType).Name);
                        if (insertStatement != null && !sync) {
                            sync = true;
                            xpoObjectHacker.CreateObjectTypeIndetifier(insertStatement, simpleDataLayer, oid[0]);
                            var modificationResult = sqlDataStoreProxy.ModifyData(insertStatement);
                            sync = false;
                            args.ModificationResult = modificationResult;
                            args.ModificationResult.Identities = new[] { new ParameterValue { Value = oid[0] } };
                        }
                    };
                    foreach (var xpObjectType in xpObjectTypes) {
                        oid[0] = xpObjectType.Oid;
                        SynchronizeTypesCore(xpObjectType, session);
                    }
                }
            }
        }

        static EventHandler<DataStoreUpdateSchemaEventArgs> EnsureIsNotIdentity(XpoObjectHacker xpoObjectHacker) {
            return (o, eventArgs) => xpoObjectHacker.EnsureIsNotIdentity(eventArgs.Tables);
        }
    }
}
