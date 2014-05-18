#region Copyright (c) 2000-2014 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
{                                                                   }
{       Copyright (c) 2000-2014 Developer Express Inc.              }
{       ALL RIGHTS RESERVED                                         }
{                                                                   }
{   The entire contents of this file is protected by U.S. and       }
{   International Copyright Laws. Unauthorized reproduction,        }
{   reverse-engineering, and distribution of all or any portion of  }
{   the code contained in this file is strictly prohibited and may  }
{   result in severe civil and criminal penalties and will be       }
{   prosecuted to the maximum extent possible under the law.        }
{                                                                   }
{   RESTRICTIONS                                                    }
{                                                                   }
{   THIS SOURCE CODE AND ALL RESULTING INTERMEDIATE FILES           }
{   ARE CONFIDENTIAL AND PROPRIETARY TRADE                          }
{   SECRETS OF DEVELOPER EXPRESS INC. THE REGISTERED DEVELOPER IS   }
{   LICENSED TO DISTRIBUTE THE PRODUCT AND ALL ACCOMPANYING .NET    }
{   CONTROLS AS PART OF AN EXECUTABLE PROGRAM ONLY.                 }
{                                                                   }
{   THE SOURCE CODE CONTAINED WITHIN THIS FILE AND ALL RELATED      }
{   FILES OR ANY PORTION OF ITS CONTENTS SHALL AT NO TIME BE        }
{   COPIED, TRANSFERRED, SOLD, DISTRIBUTED, OR OTHERWISE MADE       }
{   AVAILABLE TO OTHER INDIVIDUALS WITHOUT EXPRESS WRITTEN CONSENT  }
{   AND PERMISSION FROM DEVELOPER EXPRESS INC.                      }
{                                                                   }
{   CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON       }
{   ADDITIONAL RESTRICTIONS.                                        }
{                                                                   }
{*******************************************************************}
*/
#endregion Copyright (c) 2000-2014 Developer Express Inc.

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Exceptions;
using DevExpress.Data.Filtering;
using DevExpress.Data.Linq.Helpers;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp;

namespace Xpand.ExpressApp.NH
{
    public class NHCollection : ICancelAddNew, IBindingList, IList, ICollection, IEnumerable, ITypedList, IDisposable
    {
        private NHObjectSpace objectSpace;
        private Type objectType;
        private CriteriaOperator criteria;
        private List<SortProperty> sorting;
        private Boolean inTransaction;
        private Int32 topReturnedObjectsCount;
        private Boolean deleteObjectOnRemove;
        protected internal List<Object> objects;
        private XafPropertyDescriptorCollection propertyDescriptorCollection;
        private Boolean allowNew;
        private Boolean allowEdit;
        private Boolean allowRemove;
        private Object newObject;
        private Int32 newObjectIndex;
        private Boolean isDisposed;
        private void ClearObjects()
        {
            if (objects != null)
            {
                objects.Clear();
            }
            objects = null;
        }
        private void RemoveObject(Object obj, Int32 index)
        {
            objects.RemoveAt(index);
            RaiseListChangedEvent(new ListChangedEventArgs(ListChangedType.ItemDeleted, index));
            if (deleteObjectOnRemove)
            {
                objectSpace.Delete(obj);
            }
        }
        private void ObjectSpace_ObjectReloaded(Object sender, ObjectManipulatingEventArgs e)
        {
            if ((objects != null) && (e.Object != null) && objectType.IsAssignableFrom(e.Object.GetType()))
            {
                Int32 index = objects.IndexOf(e.Object);
                if (index >= 0)
                {
                    RaiseListChangedEvent(new ListChangedEventArgs(ListChangedType.ItemChanged, index, -1));
                }
            }
        }
        protected void InitObjects()
        {
            if (objects == null)
            {
                objects = new List<Object>();
                if (topReturnedObjectsCount == 0)
                {
                    topReturnedObjectsCount = Int32.MaxValue;
                }
                IList<String> memberNames = propertyDescriptorCollection.DisplayableMembers.Split(new String[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                IEnumerable queryable = objectSpace.GetObjects(objectType, memberNames, criteria, sorting, topReturnedObjectsCount);
                try
                {
                    foreach (Object obj in queryable)
                    {
                        if (!objectSpace.IsObjectToDelete(obj))
                        {
                            objects.Add(obj);
                        }
                    }
                }
                catch (Exception exception)
                {
                    SqlException sqlException = NHObjectSpace.GetSqlException(exception);
                    if ((sqlException != null) && (sqlException.Number == NHObjectSpace.UnableToOpenDatabaseErrorNumber))
                    {
                        throw new UnableToOpenDatabaseException("", exception);
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }
        protected internal void RaiseListChangedEvent(ListChangedEventArgs eventArgs)
        {
            if (ListChanged != null)
            {
                ListChanged(this, eventArgs);
            }
        }
        public NHCollection(NHObjectSpace objectSpace, Type objectType, CriteriaOperator criteria, IList<SortProperty> sorting, Boolean inTransaction)
        {
            this.objectSpace = objectSpace;
            this.objectType = objectType;
            this.criteria = criteria;
            this.sorting = new List<SortProperty>();
            if (sorting != null)
            {
                this.sorting.AddRange(sorting);
            }
            this.inTransaction = inTransaction;
            propertyDescriptorCollection = new XafPropertyDescriptorCollection(objectSpace.TypesInfo.FindTypeInfo(objectType));
            foreach (IMemberInfo memberInfo in NHObjectSpace.GetDefaultDisplayableMembers(propertyDescriptorCollection.TypeInfo))
            {
                propertyDescriptorCollection.CreatePropertyDescriptor(memberInfo, memberInfo.Name);
            }
            newObjectIndex = -1;
            allowNew = true;
            allowEdit = true;
            allowRemove = true;
            objectSpace.ObjectReloaded += new EventHandler<ObjectManipulatingEventArgs>(ObjectSpace_ObjectReloaded);
        }
        public void Dispose()
        {
            isDisposed = true;
            ListChanged = null;
            if (objects != null)
            {
                objects.Clear();
                objects = null;
            }
            if (objectSpace != null)
            {
                objectSpace.ObjectReloaded -= new EventHandler<ObjectManipulatingEventArgs>(ObjectSpace_ObjectReloaded);
                objectSpace = null;
            }
            propertyDescriptorCollection = null;
        }
        public void Reload()
        {
            ClearObjects();
            RaiseListChangedEvent(new ListChangedEventArgs(ListChangedType.Reset, 0));
        }
        public Type ObjectType
        {
            get { return objectType; }
        }
        public CriteriaOperator Criteria
        {
            get { return criteria; }
            set
            {
                if (!ReferenceEquals(criteria, value))
                {
                    criteria = value;
                    ClearObjects();
                    RaiseListChangedEvent(new ListChangedEventArgs(ListChangedType.Reset, 0));
                }
            }
        }
        public IList<SortProperty> Sorting
        {
            get { return sorting.AsReadOnly(); }
            set
            {
                sorting.Clear();
                if (value != null)
                {
                    sorting.AddRange(value);
                }
                ClearObjects();
                RaiseListChangedEvent(new ListChangedEventArgs(ListChangedType.Reset, 0));
            }
        }
        public Boolean InTransaction
        {
            get { return inTransaction; }
        }
        public Int32 TopReturnedObjectsCount
        {
            get { return topReturnedObjectsCount; }
            set { topReturnedObjectsCount = value; }
        }
        public String DisplayableProperties
        {
            get { return propertyDescriptorCollection.DisplayableMembers; }
            set { propertyDescriptorCollection.DisplayableMembers = value; }
        }
        public Boolean DeleteObjectOnRemove
        {
            get { return deleteObjectOnRemove; }
            set { deleteObjectOnRemove = value; }
        }
        public Boolean IsLoaded
        {
            get { return (objects != null); }
        }
        void ICancelAddNew.CancelNew(Int32 itemIndex)
        {
            if ((newObject != null) && (newObjectIndex == itemIndex))
            {
                objects.Remove(newObject);
                newObject = null;
                RaiseListChangedEvent(new ListChangedEventArgs(ListChangedType.ItemDeleted, newObjectIndex));
                newObjectIndex = -1;
            }
        }
        void ICancelAddNew.EndNew(Int32 itemIndex)
        {
            if ((newObject != null) && (newObjectIndex == itemIndex))
            {
                objectSpace.SetModified(newObject);
                newObject = null;
                newObjectIndex = -1;
            }
        }
        void IBindingList.AddIndex(PropertyDescriptor property)
        {
        }
        Object IBindingList.AddNew()
        {
            if (!allowNew)
            {
                throw new Exception("AddNew is not allowed.");
            }
            InitObjects();
            newObject = objectSpace.CreateDetachedObject(objectType);
            objects.Add(newObject);
            RaiseListChangedEvent(new ListChangedEventArgs(ListChangedType.ItemAdded, objects.Count - 1));
            newObjectIndex = objects.Count - 1;
            return newObject;
        }
        void IBindingList.ApplySort(PropertyDescriptor memberDescriptor, ListSortDirection direction)
        {
            sorting.Clear();
            SortProperty sortProperty = new SortProperty(memberDescriptor.Name, (direction == ListSortDirection.Ascending) ? SortingDirection.Ascending : SortingDirection.Descending);
            sorting.Add(sortProperty);
            ClearObjects();
            RaiseListChangedEvent(new ListChangedEventArgs(ListChangedType.Reset, 0));
        }
        void IBindingList.RemoveSort()
        {
            if (sorting.Count > 0)
            {
                sorting.Clear();
                ClearObjects();
                RaiseListChangedEvent(new ListChangedEventArgs(ListChangedType.Reset, 0));
            }
        }
        void IBindingList.RemoveIndex(PropertyDescriptor property)
        {
        }
        Int32 IBindingList.Find(PropertyDescriptor property, Object key)
        {
            InitObjects();
            for (Int32 i = 0; i < objects.Count; i++)
            {
                Object propertyValue = property.GetValue(objects[i]);
                if (propertyValue != null)
                {
                    if (propertyValue.Equals(key))
                    {
                        return i;
                    }
                }
                else
                {
                    if (key == null)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }
        public Boolean AllowNew
        {
            get { return allowNew; }
            set { allowNew = value; }
        }
        public Boolean AllowEdit
        {
            get { return allowEdit; }
            set { allowEdit = value; }
        }
        public Boolean AllowRemove
        {
            get { return allowRemove; }
            set { allowRemove = value; }
        }
        Boolean IBindingList.IsSorted
        {
            get { return (sorting.Count > 0); }
        }
        Boolean IBindingList.SupportsSorting
        {
            get { return true; }
        }
        PropertyDescriptor IBindingList.SortProperty
        {
            get
            {
                if (sorting.Count > 0)
                {
                    return ((ITypedList)this).GetItemProperties(null).Find(sorting[0].PropertyName, false);
                }
                else
                {
                    return null;
                }
            }
        }
        ListSortDirection IBindingList.SortDirection
        {
            get
            {
                if ((sorting.Count > 0) && (sorting[0].Direction == SortingDirection.Descending))
                {
                    return ListSortDirection.Descending;
                }
                else
                {
                    return ListSortDirection.Ascending;
                }
            }
        }
        Boolean IBindingList.SupportsSearching
        {
            get { return true; }
        }
        Boolean IBindingList.SupportsChangeNotification
        {
            get { return true; }
        }
        public event ListChangedEventHandler ListChanged;
        public Int32 Add(Object obj)
        {
            InitObjects();
            objects.Add(obj);
            RaiseListChangedEvent(new ListChangedEventArgs(ListChangedType.ItemAdded, objects.Count - 1));
            return objects.Count - 1;
        }
        public void Insert(Int32 index, Object obj)
        {
            InitObjects();
            objects.Insert(index, obj);
            RaiseListChangedEvent(new ListChangedEventArgs(ListChangedType.ItemAdded, index));
        }
        public void Remove(Object obj)
        {
            if (!allowRemove)
            {
                throw new Exception("Remove is not allowed.");
            }
            InitObjects();
            Int32 index = objects.IndexOf(obj);
            if (index >= 0)
            {
                RemoveObject(obj, index);
            }
        }
        public void RemoveAt(Int32 index)
        {
            if (!allowRemove)
            {
                throw new Exception("Remove is not allowed.");
            }
            InitObjects();
            if ((index >= 0) && (index < objects.Count))
            {
                Object obj = objects[index];
                RemoveObject(obj, index);
            }
        }
        public void Clear()
        {
            if (!allowRemove)
            {
                throw new Exception("Remove is not allowed.");
            }
            ClearObjects();
            RaiseListChangedEvent(new ListChangedEventArgs(ListChangedType.Reset, 0));
        }
        public Boolean Contains(Object obj)
        {
            InitObjects();
            return objects.Contains(obj);
        }
        public Int32 IndexOf(Object obj)
        {
            InitObjects();
            return objects.IndexOf(obj);
        }
        public Boolean IsReadOnly
        {
            get { return false; }
        }
        public Boolean IsFixedSize
        {
            get { return false; }
        }
        public Object this[Int32 index]
        {
            get
            {
                InitObjects();
                if ((index >= 0) && (index < objects.Count))
                {
                    return objects[index];
                }
                else
                {
                    return null;
                }
            }
            set
            {
                throw new Exception("List is read only");
            }
        }
        public void CopyTo(Array array, Int32 index)
        {
            InitObjects();
            ((IList)objects).CopyTo(array, index);
        }
        public Int32 Count
        {
            get
            {
                if (isDisposed)
                {
                    return 0;
                }
                else
                {
                    InitObjects();
                    return objects.Count;
                }
            }
        }
        public Boolean IsSynchronized
        {
            get { return false; }
        }
        public Object SyncRoot
        {
            get { return this; }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            InitObjects();
            return objects.GetEnumerator();
        }
        PropertyDescriptorCollection ITypedList.GetItemProperties(PropertyDescriptor[] listAccessors)
        {
            if ((listAccessors != null) && (listAccessors.Length > 0))
            {
                throw new Exception("listAccessors != null");
            }
            else
            {
                return propertyDescriptorCollection;
            }
        }
        String ITypedList.GetListName(PropertyDescriptor[] listAccessors)
        {
            return "";
        }
    }
}
