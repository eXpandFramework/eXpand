using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
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
using Serialize.Linq.Extensions;
using Serialize.Linq.Nodes;
using System.Reflection;
using DevExpress.Data.Linq.Helpers;
using DevExpress.Data;

namespace Xpand.ExpressApp.NH
{
    public class NHObjectSpace : BaseObjectSpace, IObjectSpace, IExpressionExecutor
    {

        public const Int32 UnableToOpenDatabaseErrorNumber = 4060;

        private readonly IPersistenceManager persistenceManager;
        private readonly IEntityStore entityStore;
        private readonly Dictionary<object, ObjectSpaceInstanceInfo> instances;
        private readonly ISelectDataSecurity selectDataSecurity;

        internal NHObjectSpace(ITypesInfo typesInfo,
            IEntityStore entityStore,
            IPersistenceManager persistenceManager,
            Dictionary<object, ObjectSpaceInstanceInfo> instances,
            ISelectDataSecurity selectDataSecurity)
            : base(typesInfo, entityStore)
        {
            Guard.ArgumentNotNull(typesInfo, "typesInfo");
            Guard.ArgumentNotNull(persistenceManager, "persistenceManager");
            Guard.ArgumentNotNull(entityStore, "entityStore");
            this.persistenceManager = persistenceManager;
            this.entityStore = entityStore;
            this.instances = instances;
            this.selectDataSecurity = selectDataSecurity;
        }

        public NHObjectSpace(ITypesInfo typesInfo, IEntityStore entityStore, IPersistenceManager persistenceManager) :
            this(typesInfo, entityStore, persistenceManager, new Dictionary<object, ObjectSpaceInstanceInfo>(), null) { }

        public NHObjectSpace(ITypesInfo typesInfo, IEntityStore entityStore, IPersistenceManager persistenceManager, ISelectDataSecurity selectDataSecurity) :
            this(typesInfo, entityStore, persistenceManager, new Dictionary<object, ObjectSpaceInstanceInfo>(), selectDataSecurity) { }

        public void ApplyCriteria(object collection, DevExpress.Data.Filtering.CriteriaOperator criteria)
        {
            DoIfINHCollection(collection, nhc => nhc.Criteria = criteria);

        }

        public void ApplyFilter(object collection, DevExpress.Data.Filtering.CriteriaOperator filter)
        {
            throw new NotImplementedException();
        }

        public override bool CanFilterByNonPersistentMembers
        {
            get
            {
                return false;
            }
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
            return new ParseCriteriaScope(this);
        }

        public object CreateServerCollection(Type objectType, DevExpress.Data.Filtering.CriteriaOperator criteria)
        {
            return new NHServerCollection(this, objectType, criteria, null);
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


        private static string ConvertToString(CriteriaOperator criteria)
        {
            CriteriaToStringWrapperSubstituteProcessor processor = new CriteriaToStringWrapperSubstituteProcessor();
            return processor.ConvertToString(criteria);
        }
        public int GetObjectsCount(Type objectType, DevExpress.Data.Filtering.CriteriaOperator criteria)
        {
            Guard.ArgumentNotNull(objectType, "objectType");

            return persistenceManager.GetObjectsCount(objectType.AssemblyQualifiedName, GetCriteriaString(objectType, criteria));
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
            get
            {
                return instances.Values
                    .Where(ii => ii.State == InstanceState.Changed || ii.State == InstanceState.New)
                    .Select(ii => ii.Instance).
                    ToList();
            }
        }

        public DevExpress.Data.Filtering.CriteriaOperator ParseCriteria(string criteria)
        {
            using (var scope = CreateParseCriteriaScope())
            {
                return CriteriaOperator.TryParse(criteria);
            }
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
        private void DoIfINHCollection(object collection, Action<INHCollection> action)
        {
            INHCollection nhCollection = collection as INHCollection;
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


        private static bool IsExpandedMember(IMemberInfo info)
        {
            var attr = info.FindAttribute<ExpandObjectMembersAttribute>();
            return attr != null && (attr.ExpandingMode & ExpandObjectMembers.InListView) == ExpandObjectMembers.InListView;
        }
        private IEnumerable<string> GetExpandedObjectFields(ITypeInfo typeInfo)
        {
            return typeInfo.Members.Where(IsExpandedMember).SelectMany(m => GetViewPropertiesNames(m.MemberType, m.Name + "."));
        }
        private IList<string> GetViewPropertiesNames(Type objectType, string prefix)
        {
            var typeInfo = TypesInfo.FindTypeInfo(objectType);
            Guard.ArgumentNotNull(typeInfo, "typeInfo");

            return typeInfo.Members.Where(m => m.IsPersistent && !m.IsAssociation).Select(m => prefix + m.Name).Concat(GetExpandedObjectFields(typeInfo)).ToArray();
        }
        protected override IList CreateDataViewCore(Type objectType, IList<DataViewExpression> expressions, CriteriaOperator criteria, IList<SortProperty> sorting)
        {

            if (expressions == null || expressions.Count == 0)
                expressions = GetViewPropertiesNames(objectType, string.Empty)
                    .Select(pn => new DataViewExpression(pn, new OperandProperty(pn))).ToArray();

            return new NHDataView(this, objectType, expressions, criteria, sorting);
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


        private List<CriteriaOperator> GetSecurityCriteria(Type type)
        {
            if (selectDataSecurity != null)
            {

                IList<string> criteria = selectDataSecurity.GetObjectCriteria(type);
                if (criteria != null)
                {
                    using (var scope = CreateParseCriteriaScope())
                    {
                        return criteria.Select(c => CriteriaOperator.TryParse(c)).Where(c => !ReferenceEquals(c, null)).ToList();
                    }
                }

            }

            return new List<CriteriaOperator>();
        }

        private CriteriaOperator CreateSecuredCriteria(Type objectType, CriteriaOperator criteria)
        {
            var secureCriteria = GetSecurityCriteria(objectType);
            if (!ReferenceEquals(null, criteria))
                secureCriteria.Add(criteria);

            if (secureCriteria.Count > 0)
            {
                CriteriaObjectReplacer replacer = new CriteriaObjectReplacer(typesInfo);
                var joinedCriteria = CriteriaOperator.And(secureCriteria);
                return (CriteriaOperator)joinedCriteria.Accept(replacer);
            }

            return null;
        }
        internal IEnumerable GetObjects(Type objectType, IList<CriteriaOperator> memberNames, CriteriaOperator criteria, List<SortProperty> sorting, int topReturnedObjectsCount)
        {
            Guard.ArgumentNotNull(objectType, "objectType");

            IList<ISortPropertyInfo> sortInfos = null;

            if (sorting != null)
            {
                sortInfos = sorting.Select(sp => new SortPropertyInfo { PropertyName = sp.PropertyName, Descending = sp.Direction == SortingDirection.Descending })
                    .Cast<ISortPropertyInfo>().ToList();
            }


            string criteriaString = GetCriteriaString(objectType, criteria);

            var objects = persistenceManager.GetObjects(objectType.AssemblyQualifiedName, memberNames, criteriaString,
                sortInfos, topReturnedObjectsCount);

            if (memberNames == null || memberNames.Count == 0)
                AddToCache(objectType, objects);
            return objects;
        }

        private string GetCriteriaString(Type objectType, CriteriaOperator criteria)
        {
            var securedCriteria = CreateSecuredCriteria(objectType, criteria);
            string criteriaString = !ReferenceEquals(null, securedCriteria) ? ConvertToString(securedCriteria) : null;
            return criteriaString;
        }

        private void AddToCache(Type objectType, IList objects)
        {
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
        }

        private object FindInstanceByKey(Type objectType, object key)
        {
            return instances.Values
                .Where(i => i.Instance.GetType() == objectType && object.Equals(GetKeyValue(i.Instance), key))
                .Select(i => i.Instance)
                .FirstOrDefault();
        }
        private void AddObjects(IEnumerable instances, InstanceState state)
        {
            foreach (var instance in instances)
                AddObject(instance, state);
        }

        private object AddOrGetExistingInstance(object instance, InstanceState state)
        {
            var existingObject = FindInstanceByKey(instance.GetType(), GetKeyValue(instance));

            if (existingObject != null) //TODO: Refresh existing object?
                return existingObject;

            AddObject(instance, state);
            return instance;
        }

        private void MergeWithExistingInstances(IList list, InstanceState state)
        {
            for (int i = 0; i < list.Count; i++)
                list[i] = AddOrGetExistingInstance(list[i], state);
        }

        private void AddObject(object instance, InstanceState state)
        {
            Guard.ArgumentNotNull(instance, "instance");
            if (instances.ContainsKey(instance)) return;
            object keyValue = GetKeyValue(instance);
            Type instanceType = instance.GetType();

            if (instances.Values.Any(ii => instanceType.IsInstanceOfType(ii.Instance) && GetKeyValue(ii.Instance) == keyValue))
                throw new ArgumentException("Instance with of the same type and key already added.", "instance");

            instances.Add(instance, new ObjectSpaceInstanceInfo { Instance = instance, State = state });
            var typeInfo = TypesInfo.FindTypeInfo(instance.GetType());
            foreach (var list in
                typeInfo.Members.Where(m => m.IsAssociation && m.IsList)
                    .Select(m => m.GetValue(instance))
                    .OfType<IList>())
                MergeWithExistingInstances(list, state);


            foreach (var member in typeInfo.Members.Where(m => m.MemberTypeInfo.IsPersistent && !m.IsList && !m.IsReadOnly))
            {
                var obj = member.GetValue(instance);
                if (obj != null)
                {
                    var addedObj = AddOrGetExistingInstance(obj, state);
                    if (!ReferenceEquals(addedObj, obj))
                        member.SetValue(instance, addedObj);
                }
            }
        }
        protected override object CreateObjectCore(Type type)
        {
            var instance = Activator.CreateInstance(type);
            AddObject(instance, InstanceState.New);
            return instance;
        }


        public override object GetObject(object obj)
        {
            XafDataViewRecord viewRecord = obj as XafDataViewRecord;
            if (viewRecord != null)
                return GetObject(viewRecord);
            else
                return GetObjectByObject(obj);

        }

        private object GetObject(XafDataViewRecord dataViewRecord)
        {
            ITypeInfo objectTypeInfo = TypesInfo.FindTypeInfo(GetOriginalType(dataViewRecord.ObjectType));
            if (objectTypeInfo.KeyMembers.Count > 1)
            {
                List<Object> keyMemberValues = new List<Object>();
                foreach (IMemberInfo keyMemberInfo in objectTypeInfo.KeyMembers)
                {
                    keyMemberValues.Add(dataViewRecord[keyMemberInfo.Name]);
                }
                return GetObjectByKey(dataViewRecord.ObjectType, keyMemberValues);
            }
            else
            {
                Object keyMemberValue = dataViewRecord[GetKeyPropertyName(dataViewRecord.ObjectType)];
                return GetObjectByKey(dataViewRecord.ObjectType, keyMemberValue);
            }
        }
        private object GetObjectByObject(object objectFromDifferentObjectSpace)
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

            result = GetObjectByKey(objectFromDifferentObjectSpace.GetType(), key);

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
            RefreshObject(oldObject, newObject, new List<object>());
        }

        private void RefreshObject(object oldObject, object newObject, List<object> refreshedInstances)
        {
            Guard.ArgumentNotNull(oldObject, "oldObject");
            Guard.ArgumentNotNull(newObject, "newObject");

            if (refreshedInstances.Any(ri => ReferenceEquals(ri, oldObject))) return;
            refreshedInstances.Add(oldObject);

            if (oldObject.GetType() != newObject.GetType())
                throw new ArgumentException("Objects must have the same type.", "newObject");

            var ti = typesInfo.FindTypeInfo(oldObject.GetType());
            if (ti == null)
                throw new ArgumentException("Unknown object type", "oldObject");


            foreach (var member in ti.Members.Where(m => m.IsPersistent))
            {
                object newValue = member.GetValue(newObject);
                object oldValue = member.GetValue(oldObject);
                if (newValue != null && oldValue != null &&
                    instances.ContainsKey(oldValue) && object.Equals(GetKeyValue(oldValue), GetKeyValue(newValue)))
                {
                    RefreshObject(oldValue, newValue, refreshedInstances);
                }
                else
                    member.SetValue(oldObject, newValue);
            }
        }

        protected override void SetModified(object obj, ObjectChangedEventArgs args)
        {
            base.SetModified(obj, args);

            if (obj != null && IsPersistent(obj))
                GetInstanceInfoSafe(obj).State = InstanceState.Changed;
        }

        private bool IsPersistent(Type type)
        {
            var typeInfo = TypesInfo.FindTypeInfo(type);
            return typeInfo != null && typeInfo.IsPersistent;
        }

        private bool IsPersistent(object obj)
        {
            return obj != null && IsPersistent(obj.GetType());
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
            else if (keyPropertyType == typeof(decimal))
            {
                decimal val;
                decimal.TryParse(objectKeyString, out val);
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

        public override object GetObjectByKey(Type type, object key)
        {

            if (key == null)
                return null;

            //TODO: Check Object Permissions
            object result = FindInstanceByKey(type, key);

            if (result != null)
                return result;

            result = persistenceManager.GetObjectByKey(type, key);
            if (result != null)
                AddObject(result, InstanceState.Unchanged);

            return result;
        }

        protected internal CriteriaOperator ProcessCriteria(Type objectType, CriteriaOperator criteria)
        {
            CriteriaOperator result = null;
            if (!ReferenceEquals(criteria, null))
            {
                result = (CriteriaOperator)((ICloneable)criteria).Clone();
                ObjectMemberValueCriteriaProcessor objectMemberValueCriteriaProcessor = new ObjectMemberValueCriteriaProcessor(typesInfo, objectType);
                objectMemberValueCriteriaProcessor.Process(result);
            }
            return result;
        }
        private IQueryable GetObjectQueryT<T>(IList<String> memberNames, CriteriaOperator criteria, IList<SortProperty> sorting)
        {
            CriteriaOperator workCriteria = ProcessCriteria(typeof(T), criteria);
            CriteriaToNHExpressionConverter converter = new CriteriaToNHExpressionConverter();
            IQueryable objectQuery = new RemoteObjectQuery<T>(new RemoteQueryProvider(this));

            objectQuery = AddSecurityWhere(objectQuery, typeof(T), converter, workCriteria);

            if (sorting != null)
            {
                List<ServerModeOrderDescriptor> orderDescriptors = new List<ServerModeOrderDescriptor>();
                foreach (SortProperty sortProperty in sorting)
                {
                    ServerModeOrderDescriptor orderDescriptor = new ServerModeOrderDescriptor(CriteriaOperator.Parse(sortProperty.PropertyName), (sortProperty.Direction == SortingDirection.Descending));
                    orderDescriptors.Add(orderDescriptor);
                }
                objectQuery = objectQuery.MakeOrderBy(converter, orderDescriptors.ToArray());
            }
            return objectQuery;
        }

        private IQueryable AddSecurityWhere(IQueryable objectQuery, Type elementType, CriteriaToNHExpressionConverter converter, CriteriaOperator workCriteria)
        {
            RemoteQueryProvider provider = (RemoteQueryProvider)objectQuery.Provider;
            try
            {
                provider.BeginAddSecurityWhere();
                return objectQuery.AppendWhere(converter, CreateSecuredCriteria(elementType, workCriteria));
            }
            finally
            {
                provider.EndAddSecurityWhere();
            }
        }
        internal IQueryable GetObjectQuery(Type type, IList<String> memberNames, CriteriaOperator criteria, IList<SortProperty> sorting)
        {
            MethodInfo methodInfo = GetType().GetMethod("GetObjectQueryT", BindingFlags.Instance | BindingFlags.NonPublic).MakeGenericMethod(type);
            return (IQueryable)methodInfo.Invoke(this, new Object[] { memberNames, criteria, sorting });
        }


        public object Execute(System.Linq.Expressions.Expression expression)
        {
            NHNodeFactory factory = new NHNodeFactory();
            var result = persistenceManager.ExecuteExpression(factory.Create(expression));
            IList list = result as IList;
            if (list != null && list
                .Cast<object>()
                .Where(o => o != null)
                .Select(o => o.GetType())
                .Distinct()
                .Count() == 1)
            {

                var persistentObjects = list.OfType<object>().Where(o => IsPersistent(o)).ToArray();
                if (persistentObjects.Length > 0)
                {
                    AddToCache(persistentObjects[0].GetType(), persistentObjects);
                    return persistentObjects;
                }

            }

            return result;
        }

        internal static string GetKeyValueAsString(ITypesInfo typesInfo, object obj)
        {
            Object keyValue = BaseObjectSpace.GetKeyValue(typesInfo, obj);
            if (keyValue != null)
            {
                Type objectType = null;
                if (obj is XafDataViewRecord)
                {
                    objectType = ((XafDataViewRecord)obj).ObjectType;
                }
                else if (obj != null)
                {
                    objectType = obj.GetType();
                }
                if (typesInfo.FindTypeInfo(objectType).KeyMembers.Count > 1)
                {
                    throw new NotSupportedException("Composite primary keys are not supported yet.");
                }
                else
                {
                    return keyValue.ToString();
                }
            }

            throw new NotImplementedException("Null key values are not supported.");
        }


        public void AddSecurityWhere(IQueryable queryable, Type elementType)
        {
            AddSecurityWhere(queryable, elementType, new CriteriaToNHExpressionConverter(), null);
        }

    }
}