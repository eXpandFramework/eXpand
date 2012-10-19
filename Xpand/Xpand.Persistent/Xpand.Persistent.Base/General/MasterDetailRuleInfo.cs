using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;

namespace Xpand.Persistent.Base.General {
    public interface IMasterDetailRuleInfo {
        IModelListView ChildListView { get; set; }
        IModelMember CollectionMember { get; set; }
        ITypeInfo TypeInfo { get; set; }
        CriteriaOperator Criteria { get; }
        bool SynchronizeActions { get; }
    }

    public class MasterDetailRuleInfo : IMasterDetailRuleInfo {
        readonly CriteriaOperator _criteria;
        readonly bool _synchronizeActions;


        public MasterDetailRuleInfo(IModelListView childListView, IModelMember collectionMember, ITypeInfo typeInfo, CriteriaOperator criteria, bool synchronizeActions) {
            ChildListView = childListView;
            CollectionMember = collectionMember;
            TypeInfo = typeInfo;
            _criteria = criteria;
            _synchronizeActions = synchronizeActions;
        }

        public IModelListView ChildListView { get; set; }
        public IModelMember CollectionMember { get; set; }
        public ITypeInfo TypeInfo { get; set; }

        public CriteriaOperator Criteria {
            get {
                return _criteria;
            }
        }

        public bool SynchronizeActions {
            get {
                return _synchronizeActions;
            }
        }
    }
}
