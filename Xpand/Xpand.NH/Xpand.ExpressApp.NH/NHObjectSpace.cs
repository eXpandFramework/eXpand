using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Utils;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using Xpand.ExpressApp.NH.Core;

namespace Xpand.ExpressApp.NH
{
    public class NHObjectSpace : BaseObjectSpace, IObjectSpace
    {

        public const Int32 UnableToOpenDatabaseErrorNumber = 4060;

        private readonly IPersistenceManager persistenceManager;
        private readonly IEntityStore entityStore;
        private readonly Dictionary<object, ObjectSpaceInstanceInfo> instances;

        internal NHObjectSpace(ITypesInfo typesInfo, IEntityStore entityStore, IPersistenceManager persistenceManager, Dictionary<object, ObjectSpaceInstanceInfo> instances)
            : base(typesInfo, entityStore)
        {
            Guard.ArgumentNotNull(typesInfo, "typesInfo");
            Guard.ArgumentNotNull(persistenceManager, "persistenceManager");
            Guard.ArgumentNotNull(entityStore, "entityStore");
            this.persistenceManager = persistenceManager;
            this.entityStore = entityStore;
            this.instances = instances;
        }

        public NHObjectSpace(ITypesInfo typesInfo, IEntityStore entityStore, IPersistenceManager persistenceManager) :
            this(typesInfo, entityStore, persistenceManager, new Dictionary<object, ObjectSpaceInstanceInfo>()) { }

        public void ApplyCriteria(object collection, DevExpress.Data.Filtering.CriteriaOperator criteria)
        {
            DoIfNHCollection(collection, nhc => nhc.Criteria = criteria);
        }

        public void ApplyFilter(object collection, DevExpress.Data.Filtering.CriteriaOperator filter)
        {
            throw new NotImplementedException();
        }

        public bool CanApplyCriteria(Type collectionType)
        {
            return typeof(NHCollection).IsAssignableFrom(collectionType);
        }

        public bool CanApplyFilter(object collection)
        {
            return collection is NHCollection;
        }

        public bool CanInstantiate(Type type)
        {
            return entityStore.RegisteredEntities.Contains(type);
        }

        public bool Contains(object obj)
        {
            if (obj == null)
                return false;

            ITypeInfo ti = typesInfo.FindTypeInfo(GetOriginalType(obj.GetType()));
            if (ti.IsPersistent)
                return instances.ContainsKey(obj);
            else
                return true;

        }

        public IObjectSpace CreateNestedObjectSpace()
        {
            return new NHNestedObjectSpace(typesInfo, entityStore, persistenceManager, instances, this);
        }

        public IDisposable CreateParseCriteriaScope()
        {
            throw new NotImplementedException();
        }

        public object CreateServerCollection(Type objectType, DevExpress.Data.Filtering.CriteriaOperator criteria)
        {
            throw new NotImplementedException();
        }

        public string Database
        {
            get { throw new NotImplementedException(); }
        }

        public void EnableObjectDeletionOnRemove(object collection, bool enable)
        {
            DoIfNHCollection(collection, nhc => nhc.DeleteObjectOnRemove = enable);
        }

        public IList<DevExpress.Xpo.SortProperty> GetCollectionSorting(object collection)
        {
            throw new NotImplementedException();
        }

        public CriteriaOperator GetCriteria(object collection)
        {
            CriteriaOperator result = null;
            DoIfNHCollection(collection, nhc => result = nhc.Criteria);
            return result;
        }

        public string GetDisplayableProperties(object collection)
        {
            throw new NotImplementedException();
        }

        public DevExpress.Data.Filtering.CriteriaOperator GetFilter(object collection)
        {
            throw new NotImplementedException();
        }

        public string GetKeyPropertyName(Type type)
        {
            return GetKeyMemberProperty(type, mi => mi.Name);
        }

        public override object FindObject(Type objectType, CriteriaOperator criteria, bool inTransaction)
        {
            return GetObjects(objectType, null, criteria, null, 1).Cast<object>().FirstOrDefault();
        }

        private TResult GetKeyMemberProperty<TResult>(Type type, Func<IMemberInfo, TResult> func)
        {
            return typesInfo.PersistentTypes
                .FirstOrDefault(pt => pt.Type == type)
                .WhenNotNull(ti => func(ti.KeyMember));
        }

        public Type GetKeyPropertyType(Type type)
        {
            return GetKeyMemberProperty(type, mi => mi.MemberType);
        }

        public int GetObjectsCount(Type objectType, DevExpress.Data.Filtering.CriteriaOperator criteria)
        {
            Guard.ArgumentNotNull(objectType, "objectType");

            return persistenceManager.GetObjectsCount(objectType.AssemblyQualifiedName, criteria.ToString());


        }


        private IList GetObjectsByState(params InstanceState[] states)
        {
            return instances.Values.Where(v => states.Contains(v.State)).Select(v => v.Instance).ToArray();
        }

        public System.Collections.ICollection GetObjectsToDelete(bool includeParent)
        {

            return GetObjectsByState(InstanceState.Deleted);
        }

        public System.Collections.ICollection GetObjectsToSave(bool includeParent)
        {
            return GetObjectsByState(InstanceState.Changed, InstanceState.New);
        }

        public int GetTopReturnedObjectsCount(object collection)
        {
            throw new NotImplementedException();
        }

        public bool IsCollectionLoaded(object collection)
        {
            return true;
        }

        public bool IsConnected
        {
            get { return true; }
        }

        public bool IsDeletedObject(object obj)
        {
            return IsObjectStateEquals(obj, InstanceState.Deleted);
        }

        public bool IsDeletionDeferredType(Type type)
        {
            return false;
        }

        public bool IsDisposedObject(object obj)
        {
            IDisposableExt disposable = obj as IDisposableExt;
            return disposable != null && disposable.IsDisposed;
        }

        private bool IsObjectStateEquals(object obj, InstanceState state)
        {
            Guard.ArgumentNotNull(obj, "obj");
            ObjectSpaceInstanceInfo instanceInfo;
            if (instances.TryGetValue(obj, out instanceInfo))
                return instanceInfo.State == state;
            else
                return false;
        }

        private ObjectSpaceInstanceInfo GetInstanceInfoSafe(object obj)
        {
            Guard.ArgumentNotNull(obj, "obj");
            ObjectSpaceInstanceInfo instanceInfo;
            if (instances.TryGetValue(obj, out instanceInfo))
                return instanceInfo;
            else
                throw new ArgumentException("Object does not belongs to the ObjectSpace");
        }

        private InstanceState GetObjectState(object obj)
        {
            return GetInstanceInfoSafe(obj).State;
        }
        public bool IsNewObject(object obj)
        {
            return IsObjectStateEquals(obj, InstanceState.New);
        }

        public bool IsObjectDeletionOnRemoveEnabled(object collection)
        {
            throw new NotImplementedException();
        }

        public bool IsObjectToDelete(object obj)
        {
            return IsObjectStateEquals(obj, InstanceState.Deleted);
        }

        public bool IsObjectToSave(object obj)
        {
            return IsObjectStateEquals(obj, InstanceState.Changed) || IsObjectStateEquals(obj, InstanceState.New);
        }

        public System.Collections.IList ModifiedObjects
        {
            get { throw new NotImplementedException(); }
        }

        public DevExpress.Data.Filtering.CriteriaOperator ParseCriteria(string criteria)
        {
            throw new NotImplementedException();
        }

        public void ReloadCollection(object collection)
        {
            throw new NotImplementedException();
        }

        public virtual object ReloadObject(object obj)
        {
            Guard.ArgumentNotNull(obj, "obj");

            obj = GetObject(obj);
            object newObject = persistenceManager.GetObjectByKey(obj.GetType(), GetKeyValue(obj));
            RefreshObject(obj, newObject);
            return obj;
        }

        public void RemoveFromModifiedObjects(object obj)
        {
            throw new NotImplementedException();
        }

        private void DoIfNHCollection(object collection, Action<NHCollection> action)
        {
            NHCollection nhCollection = collection as NHCollection;
            if (nhCollection != null)
            {
                action(nhCollection);
            }
        }
        public void SetCollectionSorting(object collection, IList<DevExpress.Xpo.SortProperty> sorting)
        {
            DoIfNHCollection(collection, nhc => nhc.Sorting = sorting);
        }

        public void SetDisplayableProperties(object collection, string displayableProperties)
        {
            throw new NotImplementedException();
        }

        public void SetTopReturnedObjectsCount(object collection, int topReturnedObjects)
        {
            DoIfNHCollection(collection, nhc => nhc.TopReturnedObjectsCount = topReturnedObjects);
        }

        public static IList<IMemberInfo> GetDefaultDisplayableMembers(ITypeInfo typeInfo)
        {
            List<IMemberInfo> result = new List<IMemberInfo>();
            foreach (IMemberInfo memberInfo in typeInfo.Members)
            {
                if (memberInfo.IsVisible || (memberInfo == memberInfo.Owner.KeyMember))
                {
                    result.Add(memberInfo);
                }
            }
            return result;
        }

        internal object CreateDetachedObject(Type objectType)
        {
            return Activator.CreateInstance(objectType);
        }


        public static SqlException GetSqlException(Exception exception)
        {
            SqlException result = null;
            while (exception != null)
            {
                if (exception is SqlException)
                {
                    result = (SqlException)exception;
                    break;
                }
                else
                {
                    exception = exception.InnerException;
                }
            }
            return result;
        }


        public object GetObjectByDataViewRecord(Type objectType, object dataViewRecord)
        {
            throw new NotImplementedException();
        }

        protected override IList<T> CreateCollection<T>(CriteriaOperator criteria, IList<SortProperty> sorting, bool inTransaction)
        {
            return new NHCollection<T>(this, criteria, sorting, inTransaction);
        }
        protected override IList CreateCollection(Type objectType, CriteriaOperator criteria, IList<SortProperty> sorting, Boolean inTransaction)
        {
            return new NHCollection(this, objectType, criteria, sorting, inTransaction);
        }

        internal IEnumerable GetObjects(Type objectType, IList<string> memberNames, CriteriaOperator criteria, List<SortProperty> sorting, int topReturnedObjectsCount)
        {
            Guard.ArgumentNotNull(objectType, "objectType");

            IList<ISortPropertyInfo> sortInfos = null;

            if (sorting != null)
            {
                sortInfos = sorting.Select(sp => new SortPropertyInfo { PropertyName = sp.PropertyName, Descending = sp.Direction == SortingDirection.Descending })
                    .Cast<ISortPropertyInfo>().ToList();
            }

            var objects = persistenceManager.GetObjects(objectType.AssemblyQualifiedName, ReferenceEquals(null, criteria) ? null : criteria.ToString(),
                sortInfos, topReturnedObjectsCount);

            Debug.Print("Adding instances...");
            var keyInstanceCache = instances.Values.Where(ii => objectType.IsInstanceOfType(ii.Instance)).ToDictionary(ii => GetKeyValue(ii.Instance));
            for (int i = 0; i < objects.Count; i++)
            {
                var obj = objects[i];
                ObjectSpaceInstanceInfo existingInstanceInfo;
                keyInstanceCache.TryGetValue(GetKeyValue(obj), out existingInstanceInfo);
                if (existingInstanceInfo != null)
                    objects[i] = existingInstanceInfo.Instance;
                else
                    AddObject(obj, InstanceState.Unchanged);
            }
            Debug.Print("Adding instances completed");
            return objects;
        }

        private object FindInstanceByKey(object key)
        {
            return instances.Values
                .Where(i => object.Equals(GetKeyValue(i.Instance), key))
                .Select(i => i.Instance)
                .FirstOrDefault();
        }
        private void AddObjects(IEnumerable instances, InstanceState state)
        {
            foreach (var instance in instances)
                AddObject(instance, state);
        }
        private void AddObject(object instance, InstanceState state)
        {
            Guard.ArgumentNotNull(instance, "instance");
            if (instances.ContainsKey(instance)) return;

            instances.Add(instance, new ObjectSpaceInstanceInfo { Instance = instance, State = state });
            var typeInfo = TypesInfo.FindTypeInfo(instance.GetType());
            AddObjects(
                typeInfo.Members.Where(m => m.IsAssociation && m.IsList)
                    .Select(m => m.GetValue(instance))
                    .OfType<IEnumerable>()
                    .Cast<IEnumerable<object>>()
                    .SelectMany(e => e), state);
        }
        protected override object CreateObjectCore(Type type)
        {
            var instance = Activator.CreateInstance(type);
            AddObject(instance, InstanceState.New);
            return instance;
        }

        public override object GetObject(object objectFromDifferentObjectSpace)
        {
            if (objectFromDifferentObjectSpace == null)
                return null;

            if (Contains(objectFromDifferentObjectSpace))
                return objectFromDifferentObjectSpace;

            object key = GetKeyValue(objectFromDifferentObjectSpace);
            if (key == null)
                throw new ArgumentException("No key defined", "objectFromDifferentObjectSpace");

            object result = instances.Values
                .Where(i => objectFromDifferentObjectSpace.GetType() == i.Instance.GetType() && object.Equals(GetKeyValue(i.Instance), key))
                .Select(i => i.Instance).FirstOrDefault();

            if (result != null)
                return result;

            result = persistenceManager.GetObjectByKey(objectFromDifferentObjectSpace.GetType(), key);

            if (result != null)
                AddObject(result, InstanceState.Unchanged);

            return result ?? objectFromDifferentObjectSpace;
        }

        protected override void DoCommit()
        {
            var modifiedObjects = GetObjectsByState(InstanceState.New, InstanceState.Changed);
            var deletedObjects = GetObjectsByState(InstanceState.Deleted);

            IList refreshedObjects = persistenceManager.UpdateObjects(modifiedObjects, deletedObjects);
            foreach (var obj in deletedObjects)
                instances.Remove(obj);

            for (int i = 0; i < modifiedObjects.Count; i++)
            {
                RefreshObject(modifiedObjects[i], refreshedObjects[i]);
                instances[modifiedObjects[i]].State = InstanceState.Unchanged;
            }
        }


        private void DeleteObject(object obj)
        {
            var info = GetInstanceInfoSafe(obj);
            info.State = InstanceState.Deleted;
        }

        protected override void DeleteCore(IList objects)
        {
            foreach (var obj in objects)
                DeleteObject(obj);
        }

        private void RefreshObject(object oldObject, object newObject)
        {
            Guard.ArgumentNotNull(oldObject, "oldObject");
            Guard.ArgumentNotNull(newObject, "newObject");

            if (oldObject.GetType() != newObject.GetType())
                throw new ArgumentException("Objects must have the same type.", "newObject");

            var ti = typesInfo.FindTypeInfo(oldObject.GetType());
            if (ti == null)
                throw new ArgumentException("Unknown object type", "oldObject");


            foreach (var member in ti.Members.Where(m => m.IsPersistent))
            {
                object newValue = member.GetValue(newObject);
                object oldValue = member.GetValue(oldObject);
                if (newValue != null && oldValue != null && instances.ContainsKey(oldValue) && object.Equals(GetKeyValue(oldValue), GetKeyValue(newValue)))
                {
                    RefreshObject(oldValue, newValue);
                }
                else
                    member.SetValue(oldObject, newValue);
            }
        }

        protected override void SetModified(object obj, ObjectChangedEventArgs args)
        {
            base.SetModified(obj, args);

            if (obj != null)
                GetInstanceInfoSafe(obj).State = InstanceState.Changed;
        }

        public override String GetKeyValueAsString(Object obj)
        {
            Object keyValue = GetKeyValue(obj);
            if (keyValue != null)
            {
                return keyValue.ToString();
            }

            return null;
        }

        public override Object GetObjectKey(Type type, String objectKeyString)
        {
            Object result = null;
            Type keyPropertyType = GetKeyPropertyType(type);
            if (keyPropertyType == typeof(Int16))
            {
                Int16 val = 0;
                Int16.TryParse(objectKeyString, out val);
                result = val;
            }
            else if (keyPropertyType == typeof(Int32))
            {
                Int32 val = 0;
                Int32.TryParse(objectKeyString, out val);
                result = val;
            }
            else if (keyPropertyType == typeof(Int64))
            {
                Int64 val = 0;
                Int64.TryParse(objectKeyString, out val);
                result = val;
            }
            else if (keyPropertyType == typeof(Guid))
            {
                result = new Guid(objectKeyString);
            }
            else if (keyPropertyType == typeof(String))
            {
                result = objectKeyString;
            }
            else
            {
                result = ObjectKeyHelper.Instance.DeserializeObjectKey(objectKeyString, typeof(List<Object>));
            }
            return result;
        }


    }
}