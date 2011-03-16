using DevExpress.Xpo;
using DevExpress.Xpo.Metadata.Helpers;
using DevExpress.Xpo.Metadata;
using System.Collections.Generic;
using DevExpress.Data.Filtering;

namespace Xpand.Persistent.Base.General {
    public class ManyToManyCollectionHelper<T> {
        private readonly XPClassInfo intermediateClassInfo;
        private readonly IXPSimpleObject owner;
        private readonly XPBaseCollection hiddenCollection;
        private XPCollection<T> collection;
        public ManyToManyCollectionHelper(IXPSimpleObject owner, XPBaseCollection hiddenCollection, string
hiddenCollectionName) {
            intermediateClassInfo = owner.ClassInfo.GetMember(hiddenCollectionName).IntermediateClass;
            this.owner = owner;
            this.hiddenCollection = hiddenCollection;
        }
        private readonly Dictionary<object, IntermediateObject> intermediateObjectHash = new Dictionary<object, IntermediateObject>();
        private void collection_CollectionChanged(object sender, XPCollectionChangedEventArgs e) {
            if (e.CollectionChangedType == XPCollectionChangedType.BeforeRemove) {
                IntermediateObject intermediateObject;
                if (intermediateObjectHash.TryGetValue(e.ChangedObject, out intermediateObject)) {
                    intermediateObject.Delete();
                    intermediateObjectHash.Remove(e.ChangedObject);
                } else {
                    hiddenCollection.BaseRemove(e.ChangedObject);
                }
            }
            if (e.CollectionChangedType == XPCollectionChangedType.AfterAdd) {
                IntermediateObject intermediateObject = null;
                if (!owner.Session.IsNewObject(e.ChangedObject)) {
                    var criteria = new GroupOperator();
                    foreach (XPMemberInfo memberInfo in intermediateClassInfo.PersistentProperties) {
                        if (memberInfo.MemberType.IsAssignableFrom(owner.GetType())) {
                            criteria.Operands.Add(new BinaryOperator(memberInfo.Name, owner));
                        }
                        if (memberInfo.MemberType.IsAssignableFrom(e.ChangedObject.GetType())) {
                            criteria.Operands.Add(new BinaryOperator(memberInfo.Name, e.ChangedObject));
                        }
                    }
                    intermediateObject = owner.Session.FindObject(intermediateClassInfo, criteria) as IntermediateObject;
                    if (intermediateObject != null && intermediateObject.IsDeleted) {
                        var newIntermediateObject = new IntermediateObject(owner.Session,
intermediateClassInfo) {
                           LeftIntermediateObjectField = intermediateObject.LeftIntermediateObjectField,
                           RightIntermediateObjectField = intermediateObject.RightIntermediateObjectField
                       };
                        intermediateObject = newIntermediateObject;
                    }
                }
                if (intermediateObject == null) {
                    intermediateObject = new IntermediateObject(owner.Session, intermediateClassInfo);
                    foreach (XPMemberInfo memberInfo in intermediateClassInfo.PersistentProperties) {
                        if (memberInfo.MemberType.IsAssignableFrom(owner.GetType())) {
                            memberInfo.SetValue(intermediateObject, owner);
                        }
                        if (memberInfo.MemberType.IsAssignableFrom(e.ChangedObject.GetType())) {
                            memberInfo.SetValue(intermediateObject, e.ChangedObject);
                        }
                    }
                }
                intermediateObjectHash.Add(e.ChangedObject, intermediateObject);
            }
        }
        public XPCollection<T> GetCollection() {
            if (collection == null) {
                collection = new XPCollection<T>(owner.Session) {LoadingEnabled = false};
                collection.AddRange(hiddenCollection);
                collection.CollectionChanged += collection_CollectionChanged;
            }
            return collection;
        }
    }
}