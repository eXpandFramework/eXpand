using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Reflection;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.Helpers;
using DevExpress.Xpo.Metadata;

namespace eXpand.Xpo.Collections
{
    /// <summary>
    /// Inherits XPCollection and add support for NotDispayAblePropertyAttribute Attribute
    /// </summary>
    public class DBCollection : XPCollection
    {
        private readonly bool isLoadEnabled = true;

        public SortProperty[] SortProperties
        {
            get
            {
                var sortProperties = new SortProperty[Sorting.Count];
                for (int i = 0; i < Sorting.Count; i++)
                    sortProperties[i] = Sorting[i];

                return sortProperties;
            }
        }

        public bool IsLoadEnabled
        {
            get { return isLoadEnabled; }
        }

        public DBCollection GetColletion(int topReturnedObjects, int pageIndex)
        {
            var xpPageSelector = new XPPageSelector(this) {PageSize = topReturnedObjects};
            if (pageIndex > -1)
                xpPageSelector.CurrentPage = pageIndex;

            return (DBCollection) ((IListSource) xpPageSelector).GetList()
                ;
        }

        public DBCollection GetColletion(int topReturnedObjects)
        {
            return GetColletion(topReturnedObjects, -1);
        }

        

//        private void addDataRows(DataTable dataTable)
//        {
////            foreach (DBObject dbObject in this)
////            {
////                Exception e;
////                DataRow dataRow = dataTable.NewRow();
////                foreach (DataColumn dataColumn in dataTable.Columns)
////                {
////                    try
////                    {
////                        object value = ReflectorHelper.GetPropertyInfoValue(dataColumn.ColumnName, dbObject);
////                        if (value is DBObject)
////                        {
////                            value = ((DBObject)value).ClassInfo.KeyProperty.GetValue(value);
////                        }
////                        dataRow[dataColumn.ColumnName] = value;
////                    }
////                    catch (Exception exception1)
////                    {
////                        e = exception1;
////                        Console.Write(e);
////                    }
////                }
////                try
////                {
////                    dataTable.Rows.Add(dataRow);
////                }
////                catch (Exception exception2)
////                {
////                    e = exception2;
////                    Console.WriteLine(e);
////                }
////            }
//
//            foreach (DBObject dbObject in this)
//            {
//                try
//                {
//                    dataTable.Rows.Add(dbObject.GetDataRow(dataTable));
//                }
//                catch (Exception e)
//                {
//                    Console.WriteLine(e);
//                }
//            }
//        }


//        public DataTable DataTable()
//        {
//            
//        }

        /// <summary>
        /// creates a <see cref="System.Data.DataTable"/> using as columns all fields in 
        /// <see cref="XPCollection.DisplayableProperties"/>
        /// </summary>
        public DataTable GetDataTable()
        {
            return GetDataTable(DisplayableProperties);
        }

        public XPView GetXpView()
        {
            return GetXpView(DisplayableProperties);
        }

        public XPView GetXpView(string fields)
        {
            var xpView = new XPView(Session, ObjectClassInfo, GetOperatorCollection(fields, new DataTable()), Criteria)
                             {
                                 TopReturnedRecords = TopReturnedObjects,
                                 Sorting = Sorting
                             };
            return xpView;
        }

        public DataTable GetDataTable(CriteriaOperator criteriaOperator, string fields)
        {
            var dataTable = new DataTable();
            CriteriaOperatorCollection operatorCollection = GetOperatorCollection(fields, dataTable);


            List<object[]> selectData = Session.SelectData(ObjectClassInfo, operatorCollection,
                                                           criteriaOperator,
                                                           false, TopReturnedObjects, Sorting);


            foreach (var objects in selectData)
            {
                int i = 0;
                DataRow newRow = dataTable.NewRow();
                foreach (DataColumn dataColumn in dataTable.Columns)
                {
                    newRow[dataColumn] = objects[i] ?? DBNull.Value;
                    i++;
                }
                dataTable.Rows.Add(newRow);
            }

            return dataTable;
        }

        public DataTable GetDataTable(string fields)
        {
            return GetDataTable(Criteria, fields);
        }

        private CriteriaOperatorCollection GetOperatorCollection(string fields, DataTable dataTable)
        {
            var operatorCollection = new CriteriaOperatorCollection();
            foreach (string property in fields.Split(';'))
                if (property != "" && property.IndexOf("!") == -1 && property != "This")
                {
                    XPMemberInfo xpMemberInfo = ReflectorHelper.GetXpMemberInfo(Session, ObjectType, property);
                    if (xpMemberInfo.IsPersistent)
                    {
                        operatorCollection.Add(CriteriaOperator.Parse(property, new object[0]));
                        var dataColumn = new DataColumn(property) {DataType = xpMemberInfo.MemberType};
                        if (typeof (XPBaseObject).IsAssignableFrom(dataColumn.DataType))
                            dataColumn.DataType = Session.GetClassInfo(dataColumn.DataType).KeyProperty.MemberType;
                        dataTable.Columns.Add(dataColumn);
                        if (property == ObjectClassInfo.KeyProperty.Name)
                            dataTable.PrimaryKey = new[] {dataColumn};
                    }
                }
            return operatorCollection;
        }


        public static void EndEdit(ICollection collection)
        {
            foreach (IEditableObject editableObject in collection)
                editableObject.EndEdit();
        }

//        public static void GetErrors(ICollection collection)
//        {
//            foreach (IError error in collection)
//                error.HasErrors();
//        }

        public void EndEdit()
        {
            foreach (IEditableObject editableObject in this)
                editableObject.EndEdit();
        }

        public void GetErrors()
        {
            foreach (XPBaseObject dbObject in this)
                dbObject.HasErrors();
        }


        public void SetDisplayAbleProperties()
        {
        }

        /// <summary>
#pragma warning disable 1574
        /// retrive a list of <see cref="DBObject.ClassInfo.KeyProperty"/> values (IDs)
#pragma warning restore 1574

        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static SortedList GetIds(XPCollection collection)
        {
            var sortedList = new SortedList();
            foreach (IXPSimpleObject ixpSimpleObject in collection)
                sortedList.Add(ixpSimpleObject.ClassInfo.KeyProperty.GetValue(ixpSimpleObject), ixpSimpleObject);

            collection.Dispose();
            return sortedList;
        }

        //TODO: u can create classes on the fly so u do not need class like WEbUserAssingAble anymore

        public object[] Assing(Type sameInterfaceObjectsType)
        {
            var objects = new object[Count];
            for (int i = 0; i < Count; i++)
            {
                objects[i] = Activator.CreateInstance(sameInterfaceObjectsType);
                var dbObject = (DBObject) this[i];
                foreach (PropertyInfo propertyInfo in dbObject.GetType().GetProperties())
                {
                    PropertyInfo property = objects[i].GetType().GetProperty(propertyInfo.Name);
                    if (property != null)
                        property.SetValue(objects[i], propertyInfo.GetValue(dbObject, null), null);
                }
            }

            return objects;
        }

        public void DeleteAll(bool disableConstrains)
        {
            DeleteAll(this, disableConstrains);
        }

        public void DeleteAll()
        {
            DeleteAll(false);
        }

        public static void DeleteAllSuper(IList list, bool constrainsDisable)
        {
            for (int i = list.Count - 1; i > -1; i--)
            {
                var dbObject = (DBObject) list[i];
                dbObject.ConstrainsDisabled = constrainsDisable;
                dbObject.Delete();
            }
        }

        public static void DeleteAllSuper(IList list)
        {
            DeleteAllSuper(list, false);
        }

        public static void DeleteAll(XPCollection xpCollection, bool disableConstrains)
        {
            var collection = new DBCollection(xpCollection);
            using (collection)
            {
                var xpPageSelector = new XPPageSelector(collection) {PageSize = 100};
                for (int i = 0; i < xpPageSelector.PageCount; i++)
                {
                    xpPageSelector.CurrentPage = i;
                    for (int j = xpPageSelector.Collection.Count - 1; j > -1; j--)
                    {
                        var dbObject = (DBObject) xpCollection[j];
                        dbObject.ConstrainsDisabled = disableConstrains;
                        dbObject.Delete();
                    }
                }
            }
        }

        public static void DeleteAll(XPCollection xpCollection)
        {
            DeleteAll(xpCollection, false);
        }

//        public string Serialize()
//        {
//            var objects = (object[]) ArrayList.Adapter(this).ToArray(ObjectType);
//            return StreamUtils.Serialize(objects, ObjectType);
//        }

        public static void RemoveAllById(DBCollection collection1, DBCollection collection2)
        {
            for (int i = collection1.Count - 1; i > -1; i--)
            {
                var dbObject = (DBObject) collection1[i];
                object keyValue = dbObject.ClassInfo.KeyProperty.GetValue(dbObject);
                if (collection2.Lookup(keyValue) != null)
                    collection1.Remove(dbObject);
            }
        }

        public void Add(IEnumerable collection)
        {
            foreach (DBObject dbObject in collection)
                Add(dbObject);
        }

        public DBCollection[] SpanTo(int spanNumbers)
        {
            var dbCollections = new DBCollection[spanNumbers];
            if (spanNumbers < 2)
                return new[] {this};
            var arrayList = new ArrayList[spanNumbers];
            for (int i = 0; i < spanNumbers; i++)
            {
                arrayList[i] = new ArrayList {0};
            }

            for (int i = 0; i < Count; i++)
            {
                for (int j = 1; j < spanNumbers + 1; j++)
                {
                    if (i < Count)
                    {
                        var ixpSimpleObject = (IXPSimpleObject) this[i];
                        arrayList[j - 1].Add(ixpSimpleObject.ClassInfo.KeyProperty.GetValue(ixpSimpleObject));
                        i++;
                    }
                }
                i--;
            }
            for (int i = 0; i < arrayList.Length; i++)
                dbCollections[i] = new DBCollection(Session, ObjectType, new InOperator("ID", arrayList[i]),
                                                    SortProperties);
            return dbCollections;
        }

        public DBCollection[] SpanToNotLoadEnable(int spanNumbers)
        {
            var dbCollections = new DBCollection[spanNumbers];
            if (spanNumbers == 1)
                return new[] {this};
            for (int i = 1; i < spanNumbers + 1; i++)
            {
                var collection = new DBCollection(Session, ObjectType, true) {Sorting = Sorting};
                dbCollections[i - 1] = collection;
            }
            int j = 0;
            for (int i = 0; i < Count; i++)
            {
                dbCollections[j].Add(this[i]);
                j++;
                if (j == spanNumbers)
                    j = 0;
            }

            return dbCollections;
        }


        public void AddCriteria(GroupOperatorType groupOperatorType, CriteriaOperator criteriaOperator)
        {
            Criteria = new GroupOperator(groupOperatorType, Criteria, criteriaOperator);
        }

        public void AddCriteria(CriteriaOperator criteriaOperator)
        {
            AddCriteria(GroupOperatorType.And, criteriaOperator);
        }

        public int GetCount()
        {
            return (int) Session.Evaluate(ObjectType, CriteriaOperator.Parse("Count()"), Criteria);
        }

        public static int GetCount(Session session, Type objectType, CriteriaOperator criteria)
        {
            return (int) session.Evaluate(objectType, CriteriaOperator.Parse("Count()"), criteria);
        }

        public static int GetCount(XPCollection collection)
        {
            return GetCount(collection.Session, collection.ObjectType, collection.Criteria);
        }

        public XPCursor GetCursor(int pagesize)
        {
            SortingCollection sortingCollection = Sorting;
            var sortProperties = new SortProperty[Sorting.Count];
            for (int i = 0; i < sortProperties.Length; i++)
                sortProperties[i] = sortingCollection[i];

            var xpCursor = new XPCursor(Session, ObjectType, Criteria, sortProperties) {PageSize = pagesize};
            return xpCursor;
        }

        public static DBCollection GetColletion(Session session, Type objectType, int pageSize, int pageIndex)
        {
            var dbCollection = new DBCollection(session, objectType);
            return dbCollection.GetColletion(pageSize, pageIndex);
        }

        public static int GetCount(XPBaseCollection collection, Type type)
        {
            return GetCount(collection.Session, type, collection.Criteria);
        }
        #region Constructors
        public DBCollection()
        {
            SetDisplayAbleProperties();
        }

        public DBCollection(ISessionProvider dbObject) : this(dbObject.Session, dbObject.GetType(), true)
        {
            Add(dbObject);
        }

        public DBCollection(Type objType) : base(objType)
        {
            SetDisplayAbleProperties();
        }

        public DBCollection(Type objType, CriteriaOperator theCriteria, params SortProperty[] sortProperties)
            : base(objType, theCriteria, sortProperties)
        {
            SetDisplayAbleProperties();
        }

        public DBCollection(Session session, Type objType, bool createEmpty) : base(session, objType, !createEmpty)
        {
            isLoadEnabled = !createEmpty;
        }

        public DBCollection(Session session, Type objType) : base(session, objType)
        {
            SetDisplayAbleProperties();
        }

        public DBCollection(Session session, XPClassInfo objType) : base(session, objType)
        {
            SetDisplayAbleProperties();
        }


        public DBCollection(Session session, Type objType, CriteriaOperator theCriteria,
                            params SortProperty[] sortProperties)
            : base(session, objType, theCriteria, sortProperties)
        {
            SetDisplayAbleProperties();
        }

        public DBCollection(Session session, XPClassInfo objType, CriteriaOperator theCriteria,
                            params SortProperty[] sortProperties) : base(session, objType, theCriteria, sortProperties)
        {
            SetDisplayAbleProperties();
        }

        public DBCollection(Session session, XPClassInfo objType, IEnumerable originalCollection)
            : base(session, objType, originalCollection)
        {
        }

        public DBCollection(XPCollection collection)
            : this(collection.Session, collection.ObjectType, collection.Criteria)
        {
            var sortProperties = new SortProperty[collection.Sorting.Count];
            for (int i = 0; i < collection.Sorting.Count; i++)
            {
                SortProperty sortProperty = collection.Sorting[i];
                sortProperties[i] = new SortProperty(sortProperty.PropertyName, sortProperty.Direction);
            }
            Sorting = new SortingCollection(this, sortProperties);
        }
        #endregion
    }
}