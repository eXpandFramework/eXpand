using System;
using DevExpress.Xpo;
using eXpand.Persistent.Base.PersistentMetaData;
using eXpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;
using eXpand.Xpo.Attributes;

namespace eXpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos {
    [InterfaceRegistrator(typeof(IPersistentAssemblyDataStoreAttributeInfo))]
    public class PersistentAssemblyDataStoreAttributeInfo : PersistentAssemblyAttributeInfo, IPersistentAssemblyDataStoreAttributeInfo {
        public PersistentAssemblyDataStoreAttributeInfo(Session session) : base(session) {
        }
        [Persistent("connectionString")]
        private string _connectionString;
        private DataStoreLogonObject _dataStoreLogon;
        
        public override AttributeInfo Create() {
            var constructorInfo = typeof(DataStoreAttribute).GetConstructor(new[]{typeof(string),typeof(Type)});
            return new AttributeInfo(constructorInfo, _connectionString);
        }

        [NonPersistent]
        public DataStoreLogonObject DataStoreLogon {
            get { return _dataStoreLogon; }
            set {
                SetPropertyValue("DataStoreLogon", ref _dataStoreLogon, value);
                _connectionString = value.GetConnectionString();
            }
        }
        IDataStoreLogonObject IPersistentAssemblyDataStoreAttributeInfo.DataStoreLogon {
            get { return DataStoreLogon; }
            set { DataStoreLogon=value as DataStoreLogonObject; }
        }
    }
}