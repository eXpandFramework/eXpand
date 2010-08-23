using System;
using DevExpress.Xpo;
using eXpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;
using eXpand.Persistent.Base.SqlDBMapper;
using eXpand.Persistent.BaseImpl.SqlDBMapper;
using eXpand.Xpo.Attributes;

namespace eXpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos {
    public class PersistentAssemblyDataStoreAttributeInfo : PersistentAssemblyAttributeInfo
    {
        public PersistentAssemblyDataStoreAttributeInfo(Session session) : base(session) {
        }
        [Persistent("connectionString")]
        private string _connectionString;
        private DataStoreLogonObject _dataStoreLogon;
        [NonPersistent]
        public DataStoreLogonObject DataStoreLogon {
            get { return _dataStoreLogon; }
            set {
                SetPropertyValue("DataStoreLogon", ref _dataStoreLogon, value);
                _connectionString = value.GetConnectionString();
            }
        }
        public override AttributeInfo Create() {
            var constructorInfo = typeof(DataStoreAttribute).GetConstructor(new[]{typeof(string),typeof(Type)});
            return new AttributeInfo(constructorInfo, _connectionString);
        }
    }
}