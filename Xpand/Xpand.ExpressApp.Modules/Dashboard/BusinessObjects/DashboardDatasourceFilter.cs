using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using DevExpress.ExpressApp.DC;
using DevExpress.Xpo;
using Xpand.Persistent.Base;

namespace Xpand.ExpressApp.Dashboard.BusinessObjects{
    [DomainComponent]
    public class DashboardDatasourceFilterList{
        public DashboardDatasourceFilterList(){
            Filters=new BindingList<DashboardDatasourceFilter>();
        }

        public BindingList<DashboardDatasourceFilter> Filters{ get; }
    }
    [NonPersistent]
    [DomainComponent]
    [SuppressMessage("Design", "XAF0023:Do not implement IObjectSpaceLink in the XPO types")]
    public class DashboardDatasourceFilter:XpandBaseCustomObject{
        public DashboardDatasourceFilter(Session session) : base(session){
        }

        string _iD;
        public string ID{
            get => _iD;
            set => SetPropertyValue(nameof(ID), ref _iD, value);
        }

        string _filter;

        public string Filter{
            get => _filter;
            set => SetPropertyValue(nameof(Filter), ref _filter, value);
        }

        string _dataSourceName;

        public string DataSourceName{
            get => _dataSourceName;
            set => SetPropertyValue(nameof(DataSourceName), ref _dataSourceName, value);
        }
        

        int _topReturnedRecords;

        public int TopReturnedRecords{
            get => _topReturnedRecords;
            set => SetPropertyValue(nameof(TopReturnedRecords), ref _topReturnedRecords, value);
        }

        bool _showPersistentMembersOnly;

        public bool ShowPersistentMembersOnly{
            get => _showPersistentMembersOnly;
            set => SetPropertyValue(nameof(ShowPersistentMembersOnly), ref _showPersistentMembersOnly, value);
        }
    }
}