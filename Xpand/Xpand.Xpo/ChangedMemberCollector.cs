using System.Collections.Generic;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;


namespace Xpand.Xpo {
    public class ChangedMemberCollector {
        public static void CollectOnSave(ISupportChangedMembers supportChangedMembers) {
            var nestedUnitOfWork = supportChangedMembers.Session as NestedUnitOfWork;
            if (nestedUnitOfWork != null) {
                var parentitem = (nestedUnitOfWork).GetParentObject(supportChangedMembers);
                IEnumerable<string> memberInfos = supportChangedMembers.ChangedProperties.Where(changedProperty => !parentitem.ChangedProperties.Contains(changedProperty));
                foreach (var changedProperty in memberInfos) {
                    parentitem.ChangedProperties.Add(changedProperty);
                }
            }
            supportChangedMembers.ChangedProperties = null;
        }

        public static void Collect(ISupportChangedMembers supportChangedMembers, string changedPropertyName) {
            if (supportChangedMembers.ChangedProperties == null)
                supportChangedMembers.ChangedProperties = new HashSet<string>();
            string memberName = GetPersistentName(supportChangedMembers, changedPropertyName);
            if (memberName != null && !supportChangedMembers.ChangedProperties.Contains(memberName)) {
                supportChangedMembers.ChangedProperties.Add(memberName);
            }
        }
        private static string GetPersistentName(ISupportChangedMembers supportChangedMembers, string propertyName) {
            var classInfo = supportChangedMembers.Session.GetClassInfo(supportChangedMembers);
            var persistentMember = classInfo.GetPersistentMember(propertyName);
            if (persistentMember == null) {
                var memberInfo = classInfo.FindMember(propertyName);
                if (memberInfo != null && memberInfo.IsAliased) {
                    var pa = (PersistentAliasAttribute)memberInfo.GetAttributeInfo(typeof(PersistentAliasAttribute));
                    CriteriaOperator criteria = CriteriaOperator.Parse(pa.AliasExpression);
                    var operandProperty = criteria as OperandProperty;
                    if (!(ReferenceEquals(operandProperty, null))) {
                        string[] path = (operandProperty).PropertyName.Split('.');
                        XPMemberInfo xpMemberInfo = path.Aggregate<string, XPMemberInfo>(null, (current, pn) => current == null ? classInfo.GetMember(pn) : current.ReferenceType.GetMember(pn));
                        return xpMemberInfo != null ? xpMemberInfo.Name : null;
                    }
                }
            }
            return persistentMember != null ? persistentMember.Name : null;
        }
    }
}