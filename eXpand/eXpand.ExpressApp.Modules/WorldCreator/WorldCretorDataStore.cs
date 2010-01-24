using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata;
using eXpand.ExpressApp.WorldCreator.Core;

namespace eXpand.ExpressApp.WorldCreator {
    public class WorldCretorDataStore : IDataStore
    {
        readonly string[] tempDatabaseTables = new[] { "ModuleInfo", "XPObjectType" };
        SimpleDataLayer legacyDataLayer;
        IDataStore legacyDataStore;
        SimpleDataLayer tempDataLayer;
        IDataStore tempDataStore;
        #region IDataStore Members
        public AutoCreateOption AutoCreateOption
        {
            get { return AutoCreateOption.DatabaseAndSchema; }
        }

        public ModificationResult ModifyData(params ModificationStatement[] dmlStatements)
        {
            var modificationResultIdentities = new List<ParameterValue>();
            foreach (ModificationStatement stm in dmlStatements)
            {
                ModificationResult modificationResult = !IsTempDatabaseTable(stm.TableName) ? legacyDataLayer.ModifyData(stm) : tempDataLayer.ModifyData(stm);
                if (modificationResult != null)
                {
                    modificationResultIdentities.AddRange(modificationResult.Identities);
                }
            }

            return new ModificationResult(modificationResultIdentities);
        }

        public SelectedData SelectData(params SelectStatement[] selects)
        {
            var resultSet = new List<SelectStatementResult>();
            foreach (SelectStatement stm in selects)
            {
                SelectedData selectedData = !IsTempDatabaseTable(stm.TableName) ? legacyDataLayer.SelectData(stm) : tempDataLayer.SelectData(stm);
                if (selectedData != null)
                {
                    resultSet.AddRange(selectedData.ResultSet);
                }
            }

            return new SelectedData(resultSet.ToArray());
        }

        public UpdateSchemaResult UpdateSchema(bool dontCreateIfFirstTableNotExist, params DBTable[] tables)
        {
            var db1Tables = new List<DBTable>();
            var db2Tables = new List<DBTable>();

            foreach (DBTable table in tables)
            {
                if (!IsTempDatabaseTable(table.Name))
                {
                    db1Tables.Add(table);
                }
                else
                {
                    db2Tables.Add(table);
                }
            }
            legacyDataStore.UpdateSchema(false, db1Tables.ToArray());
            tempDataStore.UpdateSchema(false, db2Tables.ToArray());
            return UpdateSchemaResult.SchemaExists;
        }
        #endregion
        bool IsTempDatabaseTable(string tableName)
        {
            if (!string.IsNullOrEmpty(tableName))
            {
                foreach (string currentTableName in tempDatabaseTables)
                {
                    if (tableName.EndsWith(currentTableName))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void Initialize(XPDictionary dictionary, string legacyConnectionString, string tempConnectionString)
        {
            var legacyDictionary = new ReflectionDictionary();
            var tempDictionary = new ReflectionDictionary();
            foreach (XPClassInfo ci in dictionary.Classes)
            {
                if (!IsTempDatabaseTable(ci.TableName))
                {
                    legacyDictionary.QueryClassInfo(ci.ClassType);
                }
                else
                {
                    tempDictionary.QueryClassInfo(ci.ClassType);
                }
            }
            legacyDataStore = XpoDefault.GetConnectionProvider(legacyConnectionString,
                                                               AutoCreateOption.DatabaseAndSchema);
            legacyDataLayer = new SimpleDataLayer(legacyDictionary, legacyDataStore);

            tempDataStore = XpoDefault.GetConnectionProvider(tempConnectionString, AutoCreateOption.DatabaseAndSchema);
            tempDataLayer = new SimpleDataLayer(tempDictionary, tempDataStore);
        }
    }

//    public class WorldCretorDataStore : IDataStore {
//        readonly IDataStore _currentDataStore;
//        readonly IDataLayer _currentDataLayer;
//        readonly IDataLayer _wcDataLayer;
//        readonly IDataStore _wcDataStore;
//        readonly List<string> _tables;
//
//        public WorldCretorDataStore(string connectionString,XPDictionary xpDictionary) {
//            var types = typeof (TypesInfo).GetProperties().Where(info => info.PropertyType != typeof (TypesInfo)).Select(
//                propertyInfo => propertyInfo.GetValue(TypesInfo.Instance,null)).OfType<Type>();
//            _tables =xpDictionary.Classes.OfType<XPClassInfo>().Where(classInfo => types.Contains(classInfo.ClassType)).Select(xpClassInfo => xpClassInfo.TableName).ToList();
//            _tables = new[] {"ModuleInfo", "XPObjectType"}.ToList();
//            var currentDictionary = new ReflectionDictionary();
//            var wcDictionary = new ReflectionDictionary();
//            foreach (XPClassInfo queryClassInfo in xpDictionary.Classes.Cast<XPClassInfo>()) {
//                if (!IsWCDatabaseTable(queryClassInfo.TableName))
//                    currentDictionary.QueryClassInfo(queryClassInfo.ClassType);
//                else {
//                    wcDictionary.QueryClassInfo(queryClassInfo.ClassType);
//                }
//            }
//
//            _currentDataStore = XpoDefault.GetConnectionProvider(connectionString,AutoCreateOption.DatabaseAndSchema);            
//            _currentDataLayer = new SimpleDataLayer(currentDictionary, _currentDataStore);
//            _wcDataStore = XpoDefault.GetConnectionProvider(getWCConnectionSting(connectionString), AutoCreateOption.DatabaseAndSchema);
//            _wcDataLayer = new SimpleDataLayer(wcDictionary, _wcDataStore);
//        }
//
//        string getWCConnectionSting(string connectionString)
//        {
//            var connectionStringSettings = ConfigurationManager.ConnectionStrings["WorldCreatorConnectionString"];
//            if (connectionStringSettings != null) {
//                return connectionStringSettings.ConnectionString;
//            }
//            if (_currentDataStore is MSSqlConnectionProvider) {
//                var dbConnection = ((MSSqlConnectionProvider) _currentDataStore).Connection;
//                return connectionString.Replace(dbConnection.Database, dbConnection.Database+"WorldCreator");
//            }
//            throw new NoNullAllowedException("WorldCreatorConnectionString not found in config file");
//        }
//
//
//
//
//        bool IsWCDatabaseTable(string name) {
//            return _tables.Contains(name);
//        }
//
//        public UpdateSchemaResult UpdateSchema(bool dontCreateIfFirstTableNotExist, params DBTable[] tables) {
//            var db1Tables = new List<DBTable>();
//            var db2Tables = new List<DBTable>();
//            foreach (DBTable table in tables)
//            {
//                if (!IsWCDatabaseTable(table.Name))
//                {
//                    db1Tables.Add(table);
//                }
//                else
//                {
//                    db2Tables.Add(table);
//                }
//            }
//            _currentDataStore.UpdateSchema(false, db1Tables.ToArray());
//            _wcDataStore.UpdateSchema(false, db2Tables.ToArray());
//            return UpdateSchemaResult.SchemaExists;
//        }
//
//        public SelectedData SelectData(params SelectStatement[] selects) {
//            var resultSet = new List<SelectStatementResult>();
//            foreach (SelectedData selectedData in selects.Select(stm => !IsWCDatabaseTable(stm.TableName) ? _currentDataLayer.SelectData(stm)
//                            : _wcDataLayer.SelectData(stm)).Where(selectedData => selectedData != null))
//            {
//                resultSet.AddRange(selectedData.ResultSet);
//            }
//            return new SelectedData(resultSet.ToArray());
//        }
//
//        public ModificationResult ModifyData(params ModificationStatement[] dmlStatements) {
//            var modificationResultIdentities = new List<ParameterValue>();
//            foreach (ModificationStatement stm in dmlStatements)
//            {
//                ModificationResult modificationResult = !IsWCDatabaseTable(stm.TableName)
//                                                            ? _currentDataLayer.ModifyData(stm)
//                                                            : _wcDataLayer.ModifyData(stm);
//                if (modificationResult != null)
//                {
//                    modificationResultIdentities.AddRange(modificationResult.Identities);
//                }
//            }
//            return new ModificationResult(modificationResultIdentities);
//
//        }
//
//        public AutoCreateOption AutoCreateOption {
//            get { return AutoCreateOption.DatabaseAndSchema; }
//        }
//    }
}