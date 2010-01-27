using DevExpress.Persistent.Base;

namespace eXpand.ExpressApp.Attributes {
    public class PivotedPropertyAttribute : ExpandObjectMembersAttribute
    {
        readonly string _collectionName;
        readonly string _criteria;


        public PivotedPropertyAttribute(string collectionName, string criteria):base(ExpandObjectMembers.InDetailView) {
            _collectionName = collectionName;
            _criteria = criteria;
        }

        public string Criteria {
            get { return _criteria; }
        }

        public PivotedPropertyAttribute(string collectionName):base(ExpandObjectMembers.InDetailView) {
            _collectionName = collectionName;
        }


        public string CollectionName {
            get { return _collectionName; }
        }
    }
}