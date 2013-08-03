using System.ComponentModel;
using DevExpress.ExpressApp.Utils;
using DevExpress.Xpo;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.PersistentMetaData;
using Xpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;

namespace Xpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos {
    [HideFromNewMenu]
    [InterfaceRegistrator(typeof(IPersistentAssemblyDataStoreAttribute))]
    [CreateableItem]
    public class PersistentAssemblyDataStoreAttribute : PersistentAssemblyAttributeInfo, IPersistentAssemblyDataStoreAttribute {
        public PersistentAssemblyDataStoreAttribute(Session session)
            : base(session) {
        }

        public override AttributeInfoAttribute Create() {
            var constructorInfo = typeof(DataStoreAttribute).GetConstructor(new[] { typeof(string), typeof(string), typeof(bool) });
            string initializedArgumentValues = null;
            if (PersistentClassInfo.PersistentAssemblyInfo != null)
                initializedArgumentValues = PersistentClassInfo.PersistentAssemblyInfo.Name + "." + PersistentClassInfo.Name;
            Guard.ArgumentNotNull(ConnectionString, "ConnectionString");
            return new AttributeInfoAttribute(constructorInfo, _connectionString, initializedArgumentValues, IsLegacy);
        }

        IPersistentClassInfo IPersistentAssemblyDataStoreAttribute.PersistentClassInfo {
            get { return _persistentClassInfo; }
            set { PersistentClassInfo = value as PersistentClassInfo; }
        }

        private bool _isLegacy;
        public bool IsLegacy {
            get {
                return _isLegacy;
            }
            set {
                SetPropertyValue("IsLegacy", ref _isLegacy, value);
            }
        }

        private string _connectionString;
        [Size(SizeAttribute.Unlimited)]
        public string ConnectionString {
            get {
                return _connectionString;
            }
            set {
                SetPropertyValue("ConnectionString", ref _connectionString, value);
            }
        }
        private PersistentClassInfo _persistentClassInfo;
        public PersistentClassInfo PersistentClassInfo {
            get { return _persistentClassInfo; }
            set { SetPropertyValue("PersistentClassInfo", ref _persistentClassInfo, value); }
        }
    }
    [HideFromNewMenu]
    [InterfaceRegistrator(typeof(IPersistentAssemblyDataStoreAttributeInfo))]
    [CreateableItem]
    public class PersistentAssemblyDataStoreAttributeInfo : PersistentAssemblyAttributeInfo, IPersistentAssemblyDataStoreAttributeInfo {
        public PersistentAssemblyDataStoreAttributeInfo(Session session)
            : base(session) {
        }


        private DataStoreLogonObject _dataStoreLogon;
        private string _connectionString;

        [Browsable(false)]
        [Size(SizeAttribute.Unlimited)]
        public string ConnectionString {
            get { return _connectionString; }
            set { SetPropertyValue("ConnectionString", ref _connectionString, value); }
        }
        public override AttributeInfoAttribute Create() {
            var constructorInfo = typeof(DataStoreAttribute).GetConstructor(new[] { typeof(string), typeof(string) });
            string initializedArgumentValues = null;
            if (PersistentClassInfo.PersistentAssemblyInfo != null)
                initializedArgumentValues = PersistentClassInfo.PersistentAssemblyInfo.Name + "." + PersistentClassInfo.Name;
            Guard.ArgumentNotNull(ConnectionString, "ConnectionString");
            return new AttributeInfoAttribute(constructorInfo, _connectionString, initializedArgumentValues);
        }

        [NonPersistent]
        public DataStoreLogonObject DataStoreLogon {
            get { return _dataStoreLogon; }
            set {
                SetPropertyValue("DataStoreLogon", ref _dataStoreLogon, value);
                _connectionString = value.GetConnectionString();
            }
        }
        private PersistentClassInfo _persistentClassInfo;
        public PersistentClassInfo PersistentClassInfo {
            get { return _persistentClassInfo; }
            set { SetPropertyValue("PersistentClassInfo", ref _persistentClassInfo, value); }
        }
        IPersistentClassInfo IPersistentAssemblyDataStoreAttributeInfo.PersistentClassInfo {
            get { return PersistentClassInfo; }
            set { PersistentClassInfo = value as PersistentClassInfo; }
        }

        IDataStoreLogonObject IPersistentAssemblyDataStoreAttributeInfo.DataStoreLogon {
            get { return DataStoreLogon; }
            set { DataStoreLogon = value as DataStoreLogonObject; }
        }
    }
}