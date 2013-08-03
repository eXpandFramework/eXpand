using System;
using System.ComponentModel;
using DevExpress.Xpo;
using Xpand.ExpressApp.PivotChart.PivotedProperty;
using Xpand.Persistent.Base.PersistentMetaData;
using Xpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;

namespace Xpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos {
    [DefaultProperty("CollectionName")]
    [System.ComponentModel.DisplayName("Pivoted Property")]
    [CreateableItem(typeof(IPersistentMemberInfo))]
    [CreateableItem(typeof(IExtendedMemberInfo))]
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

        public override AttributeInfoAttribute Create() {
            return
                new AttributeInfoAttribute(
                    typeof (PivotedPropertyAttribute).GetConstructor(new[] {typeof (string), typeof (string),typeof(string)}),
                    CollectionName, Criteria,AssociatedMemberName);
        }
    }
}