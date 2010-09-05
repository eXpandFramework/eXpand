using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata;
using DevExpress.Xpo.Metadata.Helpers;
using Xpand.Xpo.DB;
using Xpand.Xpo;

namespace Xpand.ExpressApp {
    public class SqlMultiDataStoreProxy:SqlDataStoreProxy {
        readonly DataStoreManager _dataStoreManager;

        protected SqlMultiDataStoreProxy() {
        }

        public SqlMultiDataStoreProxy(string connectionString)
            : this(connectionString, XafTypesInfo.XpoTypeInfoSource.XPDictionary)
        {
            
        }

        public SqlMultiDataStoreProxy(string connectionString, XPDictionary xpDictionary):base(connectionString) {
            _dataStoreManager = new DataStoreManager(connectionString);
            FillDictionaries(xpDictionary);    
        }


        public DataStoreManager DataStoreManager
        {
            get { return _dataStoreManager; }
        }

        void FillDictionaries(XPDictionary xpDictionary){
            foreach (XPClassInfo queryClassInfo in xpDictionary.Classes.OfType<XPClassInfo>().Where(info => !(info is IntermediateClassInfo))){
                ReflectionDictionary reflectionDictionary = _dataStoreManager.GetDictionary(queryClassInfo);
                reflectionDictionary.QueryClassInfo(queryClassInfo.ClassType);
            }
        }

        public override ModificationResult ModifyData(params ModificationStatement[] dmlStatements) {
            var modificationResultIdentities = new List<ParameterValue>();
            var dataStoreModifyDataEventArgs = new DataStoreModifyDataEventArgs(dmlStatements);
            OnDataStoreModifyData(dataStoreModifyDataEventArgs);
            foreach (ModificationStatement stm in dataStoreModifyDataEventArgs.ModificationStatements){
                if (stm.TableName == typeof(XPObjectType).Name) {
                    ModifyXPObjectTypeData(stm,modificationResultIdentities);
                }
                else {
                    ModifyData(stm, modificationResultIdentities);
                }
            }
            return new ModificationResult(modificationResultIdentities);
        }

        void ModifyData(ModificationStatement stm, List<ParameterValue> modificationResultIdentities) {
            string key = _dataStoreManager.GetKey(stm.TableName);
            ModificationResult modificationResult = _dataStoreManager.SimpleDataLayers[key].ModifyData(stm);
            if (modificationResult != null) {
                modificationResultIdentities.AddRange(modificationResult.Identities);
            }
        }

        void ModifyXPObjectTypeData(ModificationStatement modificationStatement, List<ParameterValue> modificationResultIdentities) {
            var insertStatement = (InsertStatement) modificationStatement;
            foreach (var parameterValues in _dataStoreManager.SimpleDataLayers.Select(pair
                => pair.Value).Select(dataLayer => TypeExists(dataLayer,insertStatement) ? null : dataLayer.ModifyData(insertStatement).Identities).Where(values => values!=null))
            {
                modificationResultIdentities.AddRange(parameterValues);
            }
        }

        bool TypeExists(SimpleDataLayer dataLayer, InsertStatement stm1) {
            if (dataLayer.Connection.ConnectionString==Connection.ConnectionString)
                return false;
            var session = new Session(dataLayer){IdentityMapBehavior = IdentityMapBehavior.Strong};
            var value = stm1.Parameters.ToList()[0].Value as string;
            var xpObjectType = session.FindObject<XPObjectType>(type => type.TypeName == value);
            return xpObjectType != null;
        }

        public override SelectedData SelectData(params SelectStatement[] selects) {
            var resultSet = new List<SelectStatementResult>();
            List<SelectedData> selectedDatas = selects.Select(stm => {
                OnDataStoreSelectData(new DataStoreSelectDataEventArgs(new[]{stm}));
                return _dataStoreManager.SimpleDataLayers[_dataStoreManager.GetKey(stm.TableName)].SelectData(stm);
            }).ToList();
            foreach (SelectedData selectedData in selectedDatas.Where(
                selectedData => selectedData != null)) {
                resultSet.AddRange(selectedData.ResultSet);
            }
            return new SelectedData(resultSet.ToArray());
        }
        public override UpdateSchemaResult UpdateSchema(bool dontCreateIfFirstTableNotExist, params DBTable[] tables){
            foreach (KeyValuePair<IDataStore, List<DBTable>> dataStore in _dataStoreManager.GetDataStores(tables)){
                IDataStore store = dataStore.Key;
                List<DBTable> dbTables = dataStore.Value;
                store.UpdateSchema(false, dbTables.ToArray());
            }
            return UpdateSchemaResult.SchemaExists;
        }

//        public static SimpleDataLayer GetDataLayer(string connectionString, XPDictionary xpDictionary, Type type) {
//            var dummyProxy = new XpoMultiDataStoreProxy(connectionString,xpDictionary);
//            connectionString = dummyProxy.DataStoreManager.GetConnectionString(type);
//            var xpoDataStoreProxy = new XpoDataStoreProxy(connectionString);
//            xpoDataStoreProxy.DataStoreModifyData += (o, eventArgs) => dummyProxy.ModifyData(eventArgs.ModificationStatements);
//            xpoDataStoreProxy.DataStoreSelectData += (sender1, dataEventArgs) => {
//                if (dummyProxy.DataStoreManager.SimpleDataLayers.Count > 1 && dummyProxy.IsQueryingXPObjectType(dataEventArgs)){
//                    dummyProxy.CreateExcludeXPObjectTypeArgs(dataEventArgs.SelectStatements, xpDictionary);
//                }
//                dummyProxy.SelectData(dataEventArgs.SelectStatements);
//            };
//            xpoDataStoreProxy.DataStoreUpdateSchema +=
//                (o1, schemaEventArgs) => dummyProxy.UpdateSchema(schemaEventArgs.DontCreateIfFirstTableNotExist, schemaEventArgs.Tables);
//            return new SimpleDataLayer(xpDictionary, dummyProxy);
//        }

//        void CreateExcludeXPObjectTypeArgs(IEnumerable<SelectStatement> selectStatements, XPDictionary xpDictionary) {
//            IEnumerable<string> typeNames =
//                xpDictionary.Classes.OfType<XPClassInfo>().Where(classInfo => classInfo.ClassType != null).Select(
//                    info => info.ClassType.FullName);
//            foreach (
//                SelectStatement selectStatement in
//                    selectStatements.Where(statement => statement.TableName == "XPObjectType")) {
//                List<string> values = typeNames.ToList();
//                var criteriaOperator = new GroupOperator(GroupOperatorType.Or);
//                foreach (string value in values) {
//                    criteriaOperator.Operands.Add(new QueryOperand("TypeName", selectStatement.Alias) == value);
//                }
//                selectStatement.Condition = criteriaOperator;
//            }
//        }

//        bool IsQueryingXPObjectType(DataStoreSelectDataEventArgs dataEventArgs) {
//            return
//                dataEventArgs.SelectStatements.Select(statement => statement.TableName).Where(s => s == "XPObjectType").
//                    FirstOrDefault() != null;
//        }
    }
}