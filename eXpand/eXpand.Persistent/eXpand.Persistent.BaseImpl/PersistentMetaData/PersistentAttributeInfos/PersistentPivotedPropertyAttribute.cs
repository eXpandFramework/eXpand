using System.ComponentModel;
using DevExpress.Xpo;
using eXpand.ExpressApp.Attributes;
using eXpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;

namespace eXpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos {
    [DefaultProperty("CollectionName")]
    public class PersistentPivotedPropertyAttribute : PersistentAttributeInfo {
        string _associatedMemberName;
        string _collectionName;
        string _criteria;

        public PersistentPivotedPropertyAttribute(Session session) : base(session) {
        }

        public string Criteria {
            get { return _criteria; }
            set { SetPropertyValue("Criteria", ref _criteria, value); }
        }

        public string CollectionName {
            get { return _collectionName; }
            set { SetPropertyValue("CollectionName", ref _collectionName, value); }
        }

        public string AssociatedMemberName {
            get { return _associatedMemberName; }
            set { SetPropertyValue("AssociatedMemberName", ref _associatedMemberName, value); }
        }

        public override AttributeInfo Create() {
            return
                new AttributeInfo(
                    typeof (PivotedPropertyAttribute).GetConstructor(new[] {typeof (string), typeof (string),typeof(string)}),
                    CollectionName, Criteria,AssociatedMemberName);
        }
    }
}