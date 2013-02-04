using System;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;

namespace Xpand.Xpo.MetaData {
    public class XpandCalcMemberInfo : XpandCustomMemberInfo {


        public XpandCalcMemberInfo(XPClassInfo owner, string propertyName, Type propertyType, string aliasExpression)
            : base(owner, propertyName, propertyType, null, true, false) {
            AddAttribute(new PersistentAliasAttribute(aliasExpression));
        }

        public override object GetValue(object theObject) {
            var xpBaseObject = ((XPBaseObject)theObject);
            return !xpBaseObject.Session.IsObjectsLoading && !xpBaseObject.Session.IsObjectsSaving
                       ? xpBaseObject.EvaluateAlias(Name)
                       : base.GetValue(theObject);
        }

        protected override bool CanPersist {
            get { return false; }
        }

        public void SetAliasExpression(string aliasExpression) {
            RemoveAttribute(typeof(PersistentAliasAttribute));
            AddAttribute(new PersistentAliasAttribute(aliasExpression));
        }
    }
}
