using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Xpo;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.IO.Core{
    [AttributeUsage(AttributeTargets.Property)]
    public class SkipTypeMappingAttribute:Attribute{
    }

    public class StorageMapper {

        private Dictionary<object,ITypeInfo> _deletedObjectKeys;
        private object[] _modifiedObjects;

        public void MapTo(IObjectSpace objectSpace, params object[] objects){
            foreach (var o in objects) {
                MapTo( o, objectSpace);
            }
        }

        readonly Dictionary<ITypeInfo,IMemberInfo[]> _memberInfos=new Dictionary<ITypeInfo, IMemberInfo[]>();
        private IMemberInfo[] GetMemberInfos(ITypeInfo typeInfo) {
            if (!_memberInfos.ContainsKey(typeInfo)){
                var serviceFields = new[] { "GCRecord", "OptimisticLockField" };
                var memberInfos = typeInfo.Members.Where(info => CanBeMapped(info, serviceFields)).ToArray();
                _memberInfos.Add(typeInfo, memberInfos);
            }
            return _memberInfos[typeInfo];
        }

        private bool CanBeMapped(IMemberInfo info, string[] serviceFields){
            return (info.IsPersistent || (info.IsAssociation && info.IsList)) && !info.IsKey &&
                   !serviceFields.Contains(info.Name)&&MemberFilter(info);
        }

        public Func<IMemberInfo, bool> MemberFilter { get; set; } = info => true;

        readonly HashSet<KeyValuePair<object, Type>> _mappedKeys=new HashSet<KeyValuePair<object, Type>>();
        readonly Dictionary<KeyValuePair<object, Type>, object> _newObjects=new Dictionary<KeyValuePair<object, Type>, object>();
        private IObjectSpace _toObjectSpace;

        private object MapTo(object fromObject, IObjectSpace objectSpace) {
            var keyValue = objectSpace.GetKeyValue(fromObject);
            var typeInfo = fromObject.GetType().GetTypeInfo();
            var toObject = GetObject(objectSpace, typeInfo, keyValue) ;
            if (!ObjectMapped(typeInfo, keyValue)) {
                foreach (var memberInfo in GetMemberInfos(typeInfo)) {
                    var memberTypeInfo = memberInfo.MemberTypeInfo;
                    var value = memberInfo.GetValue(fromObject);
                    if (memberTypeInfo.IsPersistent && memberInfo.MemberType != typeof(XPObjectType)){
                        MapReferenceMember(objectSpace, value, memberInfo,toObject);
                    }
                    else if (memberInfo.IsAssociation){
                        MapCollectionMember(objectSpace, memberInfo, toObject, value);
                    }
                    else {
                        memberInfo.SetValue(toObject, value);
                    }
                }
            }
            return toObject;
        }

        private void MapCollectionMember(IObjectSpace objectSpace, IMemberInfo memberInfo, object toObject, object value){
            var list = ((IList) memberInfo.GetValue(toObject));
            foreach (var obj in ((IEnumerable) value).Cast<object>()){
                list.Add(MapTo(obj, objectSpace));
            }
        }

        private void MapReferenceMember(IObjectSpace objectSpace, object value, IMemberInfo memberInfo, object toObject){
            if (value != null){
                var typeInfo = value.GetType().GetTypeInfo();
                if (memberInfo.FindAttribute<SkipTypeMappingAttribute>()==null)
                    MapTo(objectSpace, value);
                memberInfo.SetValue(toObject,GetObject(objectSpace, typeInfo, objectSpace.GetKeyValue(value)));
            }
        }

        private  object GetObject(IObjectSpace objectSpace, ITypeInfo typeInfo, object keyValue){
            var objectByKey = objectSpace.GetObjectByKey(typeInfo.Type, keyValue);
            if (objectByKey != null)
                return objectByKey;
            var keyValuePair = new KeyValuePair<object, Type>(keyValue, typeInfo.Type);
            if (!_newObjects.ContainsKey(keyValuePair))
                _newObjects[keyValuePair] = CreateObject(objectSpace, typeInfo, keyValue);
            return _newObjects[keyValuePair];
        }

        private object CreateObject(IObjectSpace objectSpace,ITypeInfo typeInfo, object keyValue) {
            var toObject = objectSpace.CreateObject(typeInfo.Type);
            SynchKeys(typeInfo, keyValue, toObject);
            return toObject;
        }

        private bool ObjectMapped(ITypeInfo typeInfo, object keyValue){
            return !_mappedKeys.Add(new KeyValuePair<object, Type>(keyValue, typeInfo.Type));
        }

        private void SynchKeys(ITypeInfo typeInfo, object keyValue, object toObject) {
            var keyMember = typeInfo.KeyMember;
            var persistentAliasAttribute = keyMember.FindAttribute<PersistentAliasAttribute>();
            if (persistentAliasAttribute != null)
                keyMember = typeInfo.FindMember(persistentAliasAttribute.AliasExpression);
            else {
                var persistentAttribute = keyMember.FindAttribute<PersistentAttribute>();
                if (persistentAttribute != null)
                    keyMember = typeInfo.FindMember(persistentAttribute.MapTo);
            }
            keyMember.SetValue(toObject, keyValue);
        }

        public void UpdateModifications(IObjectSpace fromObjectSpace, IObjectSpace toObjectSpace){
            _toObjectSpace = toObjectSpace;
            fromObjectSpace.Committing += ObjectSpaceOnCommitting;
            fromObjectSpace.Committed += ObjectSpaceOnCommitted;
            fromObjectSpace.ObjectDeleting += ObjectSpaceOnObjectDeleting;
            fromObjectSpace.ObjectDeleted += ObjectSpaceOnObjectDeleted;
        }

        private void ObjectSpaceOnObjectDeleted(object sender, ObjectsManipulatingEventArgs e){
            _toObjectSpace.Delete(_deletedObjectKeys.Select(o => _toObjectSpace.GetObjectByKey(o.GetType(),o)));
            _toObjectSpace.CommitChanges();
        }

        private void ObjectSpaceOnObjectDeleting(object sender, ObjectsManipulatingEventArgs e){
            var objectSpace = ((IObjectSpace) sender);
            _deletedObjectKeys = e.Objects.Cast<object>().ToDictionary(o => objectSpace.GetKeyValue(o),o => o.GetType().GetTypeInfo());
        }
       
        private void ObjectSpaceOnCommitted(object sender, EventArgs e){
            MapTo(_toObjectSpace, _modifiedObjects);
        }

        private void ObjectSpaceOnCommitting(object sender, CancelEventArgs e){
            var objectSpace = ((IObjectSpace) sender);
            _modifiedObjects = objectSpace.ModifiedObjects.Cast<object>().ToArray();
        }

        public void Unsubscribe(IObjectSpace objectSpace){
            objectSpace.Committing -= ObjectSpaceOnCommitting;
            objectSpace.Committed -= ObjectSpaceOnCommitted;
            objectSpace.ObjectDeleting -= ObjectSpaceOnObjectDeleting;
            objectSpace.ObjectDeleted -= ObjectSpaceOnObjectDeleted;
        }
    }
}
