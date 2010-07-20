using System.Collections.Generic;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Xpo.Metadata.Helpers;

namespace eXpand.Xpo {
    public class ChangedMemberCollector {
        readonly XPBaseObject _xpBaseObject;
        readonly MemberInfoCollection _memberInfoCollection;

        public ChangedMemberCollector(XPBaseObject xpBaseObject) {
            _memberInfoCollection=new MemberInfoCollection(xpBaseObject.ClassInfo);
            _xpBaseObject = xpBaseObject;
            _xpBaseObject.Session.ObjectSaved+=SessionOnObjectSaved;
            xpBaseObject.Changed+=XPBaseObjectOnChanged;
        }

        void SessionOnObjectSaved(object sender, ObjectManipulationEventArgs objectManipulationEventArgs) {
            if (ReferenceEquals(objectManipulationEventArgs.Object, _xpBaseObject)) {
                if (objectManipulationEventArgs.Session is NestedUnitOfWork) {
                    var parentitem = (ISupportChangedMembers) ((NestedUnitOfWork)objectManipulationEventArgs.Session).GetParentObject(_xpBaseObject);
                    IEnumerable<XPMemberInfo> memberInfos =_memberInfoCollection.Where(changedProperty =>
                                                                                       !parentitem.ChangedMemberCollector.MemberInfoCollection.Contains(changedProperty));
                    foreach (XPMemberInfo changedProperty in memberInfos) {
                        parentitem.ChangedMemberCollector.MemberInfoCollection.Add(changedProperty);
                    }
                }
                _memberInfoCollection.Clear();
            }
        }

        void XPBaseObjectOnChanged(object sender, ObjectChangeEventArgs objectChangeEventArgs) {

            if (!objectChangeEventArgs.Session.IsObjectsLoading){
                XPMemberInfo member = GetPersistentMember(objectChangeEventArgs.PropertyName);
                if (member != null && !_memberInfoCollection.Contains(member)){
                    _memberInfoCollection.Add(_xpBaseObject.ClassInfo.GetMember(member.Name));
                }
            }
        }
        private XPMemberInfo GetPersistentMember(string propertyName)
        {
            XPMemberInfo persistentMember = _xpBaseObject.ClassInfo.GetPersistentMember(propertyName);
            if (persistentMember == null){
                var memberInfo = _xpBaseObject.ClassInfo.FindMember(propertyName);
                if (memberInfo != null && memberInfo.IsAliased){
                    var pa = (PersistentAliasAttribute)memberInfo.GetAttributeInfo(typeof(PersistentAliasAttribute));
                    CriteriaOperator criteria = CriteriaOperator.Parse(pa.AliasExpression);
                    if (criteria is OperandProperty){
                        string[] path = ((OperandProperty)criteria).PropertyName.Split('.');
                        return path.Aggregate<string, XPMemberInfo>(null, (current, pn) => current == null ? _xpBaseObject.ClassInfo.GetMember(pn) : current.ReferenceType.GetMember(pn));
                    }
                }
            }

            return persistentMember;
        }

        public MemberInfoCollection MemberInfoCollection {
            get { return _memberInfoCollection; }
        }
    }
}