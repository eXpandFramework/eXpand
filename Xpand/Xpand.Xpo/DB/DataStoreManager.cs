using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata;

namespace Xpand.Xpo.DB {
    public class DataStoreManager {
        public const string StrDefault = "Default";
        readonly Dictionary<string, ReflectionDictionary> _reflectionDictionaries = new Dictionary<string, ReflectionDictionary>();
        readonly Dictionary<string, DataStoreManagerSimpleDataLayer> _simpleDataLayers = new Dictionary<string, DataStoreManagerSimpleDataLayer>();
        readonly Dictionary<string, List<string>> _tables = new Dictionary<string, List<string>>();
        readonly string _connectionString;
        readonly IList<DataStoreAttribute> _dataStoreAttributes;


        public DataStoreManager(string connectionString) {
            _connectionString = connectionString;
            _dataStoreAttributes = GetDataStoreAttributes().ToList();
        }


        public string GetKey(Type type) {
            var nameSpace = (type.Namespace + "");
            var dataStoreAttribute = _dataStoreAttributes.Where(attribute => nameSpace.StartsWith(attribute.NameSpace)).SingleOrDefault();
            return dataStoreAttribute == null ? StrDefault : (dataStoreAttribute.DataStoreNameSuffix ?? dataStoreAttribute.ConnectionString);
        }

        string GetKey(XPClassInfo xpClassInfo) {
            return GetKey(xpClassInfo.ClassType);
        }

        void AddTableNames(XPClassInfo xpClassInfo, string key) {
            List<string> list = _tables[key];
            list.Add(xpClassInfo.TableName);
            foreach (var tableName in GetIntermediateTableNames(xpClassInfo).Where(tableName => !list.Contains(tableName))) {
                list.Add(tableName);
            }
        }

        IEnumerable<string> GetIntermediateTableNames(XPClassInfo classInfo) {
            return classInfo.CollectionProperties.OfType<XPMemberInfo>().Where(info => info.IntermediateClass != null).Select(memberInfo => memberInfo.IntermediateClass.TableName);
        }
        public ReflectionDictionary GetDictionary(Type type) {
            XPClassInfo xpClassInfo = GetXPClassInfo(type);
            return GetDictionary(xpClassInfo);
        }

        XPClassInfo GetXPClassInfo(Type type) {
            var xpClassInfos = _reflectionDictionaries.Select(pair => pair.Value).SelectMany(dictionary => dictionary.Classes.OfType<XPClassInfo>());
            return xpClassInfos.Where(info => info.ClassType == type).Single();
        }

        public ReflectionDictionary GetDictionary(XPClassInfo xpClassInfo) {
            string key = GetKey(xpClassInfo);
            var reflectionDictionary = GetDictionary(key);
            if (xpClassInfo.IsPersistent)
                AddTableNames(xpClassInfo, key);
            return reflectionDictionary;
        }

        ReflectionDictionary GetDictionary(string key) {
            if (!_reflectionDictionaries.ContainsKey(key)) {
                var reflectionDictionary = new ReflectionDictionary();
                _reflectionDictionaries.Add(key, reflectionDictionary);
                var simpleDataLayer = new DataStoreManagerSimpleDataLayer(reflectionDictionary, GetConnectionProvider(key),key==StrDefault);
                _simpleDataLayers.Add(key, simpleDataLayer);
                _tables.Add(key, new List<string>());
            }
            return _reflectionDictionaries[key];
        }

        public IDataStore GetConnectionProvider(Type type) {
            return GetConnectionProvider(GetKey(type));
        }

        public string GetConnectionString(Type type) {
            string key = GetKey(type);
            return GetConnectionString(key);
        }


        public string GetConnectionString(string key) {
            if (key == StrDefault)
                return _connectionString;
            if (key.StartsWith(DataStoreBase.XpoProviderTypeParameterName))
                return key;
            ConnectionStringSettings connectionStringSettings =
                ConfigurationManager.ConnectionStrings[string.Format("{0}ConnectionString", key)];
            if (connectionStringSettings != null) {
                return connectionStringSettings.ConnectionString;
            }
            IDataStore connectionProvider = XpoDefault.GetConnectionProvider(_connectionString, AutoCreateOption.DatabaseAndSchema);
            if (connectionProvider is ConnectionProviderSql) {
                IDbConnection dbConnection = ((ConnectionProviderSql)connectionProvider).Connection;
                return _connectionString == null ? AccessConnectionProvider.GetConnectionString(key)
                                              : _connectionString.Replace(dbConnection.Database, String.Format("{0}{1}.mdb", dbConnection.Database, key));

            }
            throw new NoNullAllowedException(string.Format("{0}ConnectionString not found ", key));
        }

        public IDataStore GetConnectionProvider(string key) {
            string connectionString = GetConnectionString(key);
            return XpoDefault.GetConnectionProvider(connectionString, AutoCreateOption.DatabaseAndSchema);
        }

        public IEnumerable<DataStoreAttribute> GetDataStoreAttributes() {
            var dataStoreAttributes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetCustomAttributes(typeof(Attribute), false).OfType<DataStoreAttribute>());
            return dataStoreAttributes.Where(attribute => (attribute.ConnectionString != null || ConfigurationManager.ConnectionStrings[string.Format("{0}ConnectionString", attribute.DataStoreNameSuffix)] != null)).ToList();
        }

        public Dictionary<string, DataStoreManagerSimpleDataLayer> SimpleDataLayers {
            get { return _simpleDataLayers; }
        }

        public Dictionary<IDataStore, List<DBTable>> GetDataStores(DBTable[] dbTables) {
            var dictionary = _simpleDataLayers.Select(pair =>
                                                      pair.Value.ConnectionProvider).ToDictionary(dataStore => dataStore, dataStore => new List<DBTable>());
            foreach (var dbTable in dbTables) {
                if (dbTable.Name == "XPObjectType")
                    foreach (var simpleDataLayer in _simpleDataLayers) {
                        dictionary[simpleDataLayer.Value.ConnectionProvider].Add(dbTable);
                    } else
                    dictionary[_simpleDataLayers[GetKey(dbTable.Name)].ConnectionProvider].Add(dbTable);

            }
            return dictionary;
        }

        public string GetKey(string tableName) {
            var keyValuePairs = _tables.Where(valuePair => valuePair.Value.Contains(tableName)).ToList();
            string key = StrDefault;
            if (keyValuePairs.Count() > 0)
                key = keyValuePairs[0].Key;
            return key;
        }

        public Type GetType(string typeName) {
            var types = _reflectionDictionaries.Select(pair => pair.Value).SelectMany(dictionary => dictionary.Classes.OfType<XPClassInfo>()).Select(classInfo => classInfo.ClassType);
            return types.Where(type => type.Name == typeName).SingleOrDefault();
        }

    }

    public class DataStoreManagerSimpleDataLayer:SimpleDataLayer {
        readonly bool _isMainLayer;

        public DataStoreManagerSimpleDataLayer(IDataStore provider) : base(provider) {
        }

        public DataStoreManagerSimpleDataLayer(XPDictionary dictionary, IDataStore provider, bool isMainLayer) : base(dictionary, provider) {
            _isMainLayer = isMainLayer;
        }

        public bool IsMainLayer {
            get { return _isMainLayer; }
        }
    }
}