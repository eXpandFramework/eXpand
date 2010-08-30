using DevExpress.Persistent.Base;

namespace eXpand.ExpressApp.PivotChart.PivotedProperty {
    public class PivotedPropertyAttribute : ExpandObjectMembersAttribute
    {
        readonly string _collectionName;
        readonly string _analysisCriteria;
        readonly string _associatedMemberName;


        public PivotedPropertyAttribute(string collectionName, string analysisCriteria):base(ExpandObjectMembers.InDetailView) {
            _collectionName = collectionName;
            _analysisCriteria = analysisCriteria;
        }
        public PivotedPropertyAttribute(string collectionName, string analysisCriteria, string associatedMemberName):this(collectionName,analysisCriteria) {
            _collectionName = collectionName;
            _analysisCriteria = analysisCriteria;
            _associatedMemberName = associatedMemberName;
        }

        public PivotedPropertyAttribute(string collectionName):base(ExpandObjectMembers.InDetailView) {
            _collectionName = collectionName;
        }

        public string AssociatedMemberName {
            get { return _associatedMemberName; }
        }

        public string AnalysisCriteria {
            get { return _analysisCriteria; }
        }


        public string CollectionName {
            get { return _collectionName; }
        }
    }
}