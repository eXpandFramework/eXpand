using System;
using System.Collections.Generic;
using System.Reflection;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using Xpand.Xpo.MetaData;

namespace Xpand.ExpressApp.Xpo.MetaData {
    public class XpandCollectionMemberInfo : XpandCustomMemberInfo {
        readonly string _criteria;
        static XpandCollectionMemberInfo() {
            var propertyInfo = typeof (PersistentBase).GetProperty("CustomPropertyStore", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        public XpandCollectionMemberInfo(XPClassInfo owner, string propertyName, Type propertyType, string criteria)
            : base(owner, propertyName, propertyType, null, true, false) {
            _criteria = criteria;
        }

        public string Criteria {
            get { return _criteria; }
        }

        public override object GetValue(object theObject) {
            var xpBaseObject = ((XPBaseObject)theObject);
            return base.GetStore(theObject).GetCustomPropertyValue(this) == null
                       ? Activator.CreateInstance(MemberType, GetArguments(xpBaseObject))
                       : base.GetValue(theObject);
        }

        object[] GetArguments(XPBaseObject xpBaseObject) {
            if (!string.IsNullOrEmpty(_criteria))
                return new object[] {
                                    xpBaseObject.Session,
                                    new CriteriaWrapper(_criteria, xpBaseObject).CriteriaOperator
                                };
            return new object[] { xpBaseObject.Session };
        }

        protected override bool CanPersist {
            get { return false; }
        }
    }
}
