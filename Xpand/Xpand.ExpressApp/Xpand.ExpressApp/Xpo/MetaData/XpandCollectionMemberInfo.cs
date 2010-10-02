using System;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;

namespace Xpand.ExpressApp.Xpo.MetaData
{
    public class XpandCollectionMemberInfo : XPCustomMemberInfo
    {
        readonly string _propertyName;
        readonly string _criteria;

        public XpandCollectionMemberInfo(XPClassInfo owner, string propertyName, Type propertyType, string criteria)
            : base(owner, propertyName, propertyType, null, false, false)
        {
            _propertyName = propertyName;
            _criteria = criteria;
        }

        public override object GetValue(object theObject)
        {
            var xpBaseObject = ((XPBaseObject)theObject);

            if (base.GetStore(this).GetCustomPropertyValue(this) == null)
            {
                return Activator.CreateInstance(this.MemberType, new object[] { xpBaseObject.Session, new CriteriaWrapper(_criteria, xpBaseObject).CriteriaOperator });
            }

            return base.GetValue(theObject);
        }

        protected override bool CanPersist
        {
            get { return false; }
        }
    }
}
