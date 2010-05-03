using System;
using System.Collections.Generic;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Xpo.Metadata.Helpers;

namespace eXpand.Xpo.Collections {
    public class ManyToManyCollectionHelper<T>
    {
        private XPClassInfo intermediateClassInfo;
        private IXPSimpleObject owner;
        private XPBaseCollection hiddenCollection;
        private XPCollection<T> collection;
        public ManyToManyCollectionHelper(IXPSimpleObject owner, XPBaseCollection hiddenCollection, string hiddenCollectionName)
        {
            this.intermediateClassInfo = owner.ClassInfo.GetMember(hiddenCollectionName).IntermediateClass;
            this.owner = owner;
            this.hiddenCollection = hiddenCollection;
        }
        private Dictionary<object, IntermediateObject> intermediateObjectHash = new Dictionary<object, IntermediateObject>();
        private void collection_CollectionChanged(object sender, XPCollectionChangedEventArgs e)
        {
            if (e.CollectionChangedType == XPCollectionChangedType.BeforeRemove)
            {
                IntermediateObject intermediateObject = null;
                if (intermediateObjectHash.TryGetValue(e.ChangedObject, out intermediateObject))
                {
                    intermediateObject.Delete();
                    intermediateObjectHash.Remove(e.ChangedObject);
                }
                else
                {
                    hiddenCollection.BaseRemove(e.ChangedObject);
                }
            }
            if (e.CollectionChangedType == XPCollectionChangedType.AfterAdd)
            {
                IntermediateObject intermediateObject = null;
                if (!owner.Session.IsNewObject(e.ChangedObject))
                {
                    GroupOperator criteria = new DevExpress.Data.Filtering.GroupOperator();
                    foreach (XPMemberInfo memberInfo in intermediateClassInfo.PersistentProperties)
                    {
                        if (memberInfo.MemberType.IsAssignableFrom(owner.GetType()))
                        {
                            criteria.Operands.Add(new BinaryOperator(memberInfo.Name, owner));
                        }
                        if (memberInfo.MemberType.IsAssignableFrom(e.ChangedObject.GetType()))
                        {
                            criteria.Operands.Add(new BinaryOperator(memberInfo.Name, e.ChangedObject));
                        }
                    }
                    intermediateObject = owner.Session.FindObject(intermediateClassInfo, criteria) as IntermediateObject;
                    if (intermediateObject != null && intermediateObject.IsDeleted)
                    {
                        IntermediateObject newIntermediateObject = new IntermediateObject(owner.Session, intermediateClassInfo);
                        newIntermediateObject.LeftIntermediateObjectField = intermediateObject.LeftIntermediateObjectField;
                        newIntermediateObject.RightIntermediateObjectField = intermediateObject.RightIntermediateObjectField;
                        intermediateObject = newIntermediateObject;
                    }
                }
                if (intermediateObject == null)
                {
                    intermediateObject = new IntermediateObject(owner.Session, intermediateClassInfo);
                    foreach (XPMemberInfo memberInfo in intermediateClassInfo.PersistentProperties)
                    {
                        if (memberInfo.MemberType.IsAssignableFrom(owner.GetType()))
                        {
                            memberInfo.SetValue(intermediateObject, owner);
                        }
                        if (memberInfo.MemberType.IsAssignableFrom(e.ChangedObject.GetType()))
                        {
                            memberInfo.SetValue(intermediateObject, e.ChangedObject);
                        }
                    }
                }
                intermediateObjectHash.Add(e.ChangedObject, intermediateObject);
            }
        }
        public XPCollection<T> GetCollection()
        {
            if (collection == null)
            {
                Type type = hiddenCollection.GetType();
                collection = new XPCollection<T>(owner.Session);
                collection.LoadingEnabled = false;
                collection.AddRange(hiddenCollection);
                collection.CollectionChanged += new XPCollectionChangedEventHandler(collection_CollectionChanged);
            }
            return collection;
        }
    }
}