using DevExpress.Persistent.Base;

namespace eXpand.ExpressApp.PivotChart.PivotedProperty {
    public class PivotedPropertyAttribute : ExpandObjectMembersAttribute
    {
        readonly string _collectionName;
        readonly string _criteria;
        readonly string _associatedMemberName;


        public PivotedPropertyAttribute(string collectionName, string criteria):base(ExpandObjectMembers.InDetailView) {
            _collectionName = collectionName;
            _criteria = criteria;
        }
        public PivotedPropertyAttribute(string collectionName, string criteria, string associatedMemberName):this(collectionName,criteria) {
            _collectionName = collectionName;
            _criteria = criteria;
            _associatedMemberName = associatedMemberName;
        }

        public PivotedPropertyAttribute(string collectionName):base(ExpandObjectMembers.InDetailView) {
            _collectionName = collectionName;
        }

        public string AssociatedMemberName {
            get { return _associatedMemberName; }
        }

        public string Criteria {
            get { return _criteria; }
        }


        public string CollectionName {
            get { return _collectionName; }
        }
    }
}