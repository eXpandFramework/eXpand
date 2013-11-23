using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;

namespace Xpand.Persistent.Base.General {
    public interface IMasterDetailRuleInfo {
        IModelListView ChildListView { get; set; }
        IModelMember CollectionMember { get; set; }
        ITypeInfo TypeInfo { get; set; }
        CriteriaOperator Criteria { get; }
    }

    public class MasterDetailRuleInfo : IMasterDetailRuleInfo {
        readonly CriteriaOperator _criteria;

        public MasterDetailRuleInfo(IModelListView childListView, IModelMember collectionMember, ITypeInfo typeInfo, CriteriaOperator criteria) {
            ChildListView = childListView;
            CollectionMember = collectionMember;
            TypeInfo = typeInfo;
            _criteria = criteria;
        }

        public IModelListView ChildListView { get; set; }
        public IModelMember CollectionMember { get; set; }
        public ITypeInfo TypeInfo { get; set; }

        public CriteriaOperator Criteria {
            get {
                return _criteria;
            }
        }
    }
}
