using System;
using System.ComponentModel;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using eXpand.Persistent.Base.PersistentMetaData;
using eXpand.Xpo.Converters.ValueConverters;

namespace eXpand.Persistent.BaseImpl.PersistentMetaData {
    [DefaultProperty("AssociationName")]
    public class PersistentAssociationAttribute : PersistentAttributeInfo, IPersistentAssociationAttribute {
        private string _associationName;
        

        public PersistentAssociationAttribute(Session session) : base(session) {
        }

        public PersistentAssociationAttribute() {
        }
        #region IPersistentAssociationAttribute Members
        [VisibleInListView(true)]
        [RuleRequiredField(null,DefaultContexts.Save)]
        public string AssociationName {
            get { return _associationName; }
            set { SetPropertyValue("AssociationName", ref _associationName, value); }
        }
        [Size(SizeAttribute.Unlimited)]
        [ValueConverter(typeof(TypeValueConverter))]
        [RuleRequiredField(null, DefaultContexts.Save)]
        [TypeConverter(typeof(LocalizedClassInfoTypeConverter))]
        public Type ElementType { get; set; }


        public override Attribute Create() {
            return new AssociationAttribute(AssociationName, ElementType.Assembly.FullName, ElementType.FullName);
        }
        #endregion
    }
}