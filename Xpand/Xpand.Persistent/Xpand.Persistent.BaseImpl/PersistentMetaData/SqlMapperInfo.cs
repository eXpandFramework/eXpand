using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using Xpand.Persistent.Base.PersistentMetaData;

namespace Xpand.Persistent.BaseImpl.PersistentMetaData {
    [InterfaceRegistrator(typeof(IDataStoreLogonObject))]
    [NonPersistent]
    public class SqlMapperInfo:DataStoreLogonObject, ISqlMapperInfo {
        public SqlMapperInfo(Session session) : base(session) {
        }

        public SqlMapperInfo(Session session, SqlMapperInfo sqlMapperInfo)
            : base(session, sqlMapperInfo)
        {
        }
        private MapperInfo _mapperInfo;
        [Index(5)]
        [Aggregated][ExpandObjectMembers(ExpandObjectMembers.InDetailView)]
        public MapperInfo MapperInfo
        {
            get { return _mapperInfo; }
            set { SetPropertyValue("MapperInfo", ref _mapperInfo, value); }
        }

        IMapperInfo ISqlMapperInfo.MapperInfo {
            get { return MapperInfo; }
            set { MapperInfo=value as MapperInfo; }
        }
    }
    [NonPersistent]
    public class MapperInfo :BaseObject, IMapperInfo {
        public MapperInfo(Session session) : base(session) {
        }


        private string _navigationPath;

        public string NavigationPath {
            get { return _navigationPath; }
            set { SetPropertyValue("NavigationPath", ref _navigationPath, value); }
        }
        private bool _xpoDataBase;

        public bool XpoDataBase {
            get { return _xpoDataBase; }
            set { SetPropertyValue("XpoDataBase", ref _xpoDataBase, value); }
        }
    }
}