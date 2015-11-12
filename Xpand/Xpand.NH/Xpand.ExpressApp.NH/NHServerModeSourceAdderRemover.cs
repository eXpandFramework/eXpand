#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
{                                                                   }
{       Copyright (c) 2000-2015 Developer Express Inc.              }
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
#endregion Copyright (c) 2000-2015 Developer Express Inc.

using System;
using System.Linq;
using System.Collections;
using System.ComponentModel;
using System.Collections.Generic;
using DevExpress.Data;
using DevExpress.Data.Helpers;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp;

namespace Xpand.ExpressApp.NH
{
    public class NHServerModeSourceAdderRemover : IListServer, IListServerHints, IBindingList, ITypedList, IDXCloneable
    {
        public const String SimultaneousGroupingAndAddingRemovingIsNotAllowed = "It's not allowed to simultaneously group and add/remove items.";
        private Object serverModeSource;
        private NHObjectSpace objectSpace;
        private Type objectType;
        private IListServer listServer;
        private IListServerHints listServerHints;
        private IBindingList bindingList;
        private ITypedList typedList;
        private IDXCloneable dxCloneable;
        private List<Object> addedObjects;
        private Dictionary<Object, Byte> addedObjectsDictionary;
        private Dictionary<Object, Byte> removedObjectsDictionary;
        private Int32 currentGroupCount;
        private XafPropertyDescriptorCollection propertyDescriptorCollection;
        private Boolean IsModified
        {
            get { return (addedObjectsDictionary.Count > 0) || (removedObjectsDictionary.Count > 0); }
        }
        private Boolean IsAddingRemovingAllowed
        {
            get { return (currentGroupCount == 0); }
        }
        private void ValidateLists()
        {
            for (Int32 i = addedObjects.Count - 1; i >= 0; i--)
            {
                Object obj = addedObjects[i];
                if (bindingList.Contains(obj))
                {
                    addedObjects.RemoveAt(i);
                    addedObjectsDictionary.Remove(obj);
                }
            }
            List<Object> removedObjects = new List<Object>(removedObjectsDictionary.Keys.ToList());
            foreach (Object obj in removedObjects)
            {
                if (!bindingList.Contains(obj))
                {
                    removedObjectsDictionary.Remove(obj);
                }
            }
        }
        private Int32 GetRemovedObjectsCountBeforeIndex(Int32 index)
        {
            Int32 removedObjectsCount = 0;
            foreach (Object removedObject in removedObjectsDictionary.Keys)
            {
                Int32 removedObjectIndex = bindingList.IndexOf(removedObject);
                if ((removedObjectIndex >= 0) && (removedObjectIndex < index))
                {
                    removedObjectsCount++;
                }
            }
            return removedObjectsCount;
        }
        private void RemoveObject(Object obj, Int32 index)
        {
            if (!IsAddingRemovingAllowed)
            {
                throw new InvalidOperationException(SimultaneousGroupingAndAddingRemovingIsNotAllowed);
            }
            if (addedObjectsDictionary.ContainsKey(obj))
            {
                addedObjects.Remove(obj);
                addedObjectsDictionary.Remove(obj);
            }
            else
            {
                removedObjectsDictionary[obj] = 0;
            }
            RaiseListChangedEvent(new ListChangedEventArgs(ListChangedType.ItemDeleted, index));
        }
        private void RaiseListChangedEvent(ListChangedEventArgs eventArgs)
        {
            if (ListChanged != null)
            {
                ListChanged(this, eventArgs);
            }
        }
        private void bindingList_ListChanged(Object sender, ListChangedEventArgs e)
        {
            ValidateLists();
            RaiseListChangedEvent(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }
        public NHServerModeSourceAdderRemover(Object serverModeSource, NHObjectSpace objectSpace, Type objectType)
        {
            this.serverModeSource = serverModeSource;
            this.objectSpace = objectSpace;
            this.objectType = objectType;
            listServer = serverModeSource as IListServer;
            listServerHints = serverModeSource as IListServerHints;
            bindingList = serverModeSource as IBindingList;
            typedList = serverModeSource as ITypedList;
            dxCloneable = serverModeSource as IDXCloneable;
            addedObjects = new List<Object>();
            addedObjectsDictionary = new Dictionary<Object, Byte>();
            removedObjectsDictionary = new Dictionary<Object, Byte>();
            bindingList.ListChanged += new ListChangedEventHandler(bindingList_ListChanged);
            ITypeInfo typeInfo = objectSpace.TypesInfo.FindTypeInfo(objectType);
            propertyDescriptorCollection = new XafPropertyDescriptorCollection(typeInfo);
            foreach (IMemberInfo memberInfo in NHObjectSpace.GetDefaultDisplayableMembers(typeInfo))
            {
                propertyDescriptorCollection.CreatePropertyDescriptor(memberInfo, memberInfo.Name);
            }
        }
        public String DisplayableProperties
        {
            get { return propertyDescriptorCollection.DisplayableMembers; }
            set
            {
                if (propertyDescriptorCollection.DisplayableMembers != value)
                {
                    propertyDescriptorCollection.DisplayableMembers = value;
                    RaiseListChangedEvent(new ListChangedEventArgs(ListChangedType.PropertyDescriptorChanged, -1, -1));
                }
            }
        }
        public void Apply(CriteriaOperator filterCriteria, ICollection<ServerModeOrderDescriptor> sortInfo, Int32 groupCount, ICollection<ServerModeSummaryDescriptor> groupSummaryInfo, ICollection<ServerModeSummaryDescriptor> totalSummaryInfo)
        {
            if ((groupCount > 0) && IsModified)
            {
                throw new InvalidOperationException(SimultaneousGroupingAndAddingRemovingIsNotAllowed);
            }
            currentGroupCount = groupCount;
            listServer.Apply(filterCriteria, sortInfo, groupCount, groupSummaryInfo, totalSummaryInfo);
            ValidateLists();
        }
        public Int32 FindIncremental(CriteriaOperator expression, String value, Int32 startIndex, Boolean searchUp, Boolean ignoreStartRow, Boolean allowLoop)
        {
            return listServer.FindIncremental(expression, value, startIndex, searchUp, ignoreStartRow, allowLoop);
        }
        public IList GetAllFilteredAndSortedRows()
        {
            List<Object> list = new List<Object>(listServer.GetAllFilteredAndSortedRows().OfType<Object>());
            foreach (Object obj in addedObjectsDictionary.Keys)
            {
                list.Remove(obj);
            }
            foreach (Object obj in removedObjectsDictionary.Keys)
            {
                list.Remove(obj);
            }
            list.AddRange(addedObjects);
            return list;
        }
        public virtual Boolean PrefetchRows(ListSourceGroupInfo[] groupsToPrefetch, System.Threading.CancellationToken cancellationToken)
        {
            return listServer.PrefetchRows(groupsToPrefetch, cancellationToken);
        }
        public List<ListSourceGroupInfo> GetGroupInfo(ListSourceGroupInfo parentGroup)
        {
            return listServer.GetGroupInfo(parentGroup);
        }
        public Int32 GetRowIndexByKey(Object key)
        {
            Int32 result = -1;
            if (!IsModified)
            {
                result = listServer.GetRowIndexByKey(key);
            }
            else if (key != null)
            {
                Object obj = null;
                if (objectType.IsAssignableFrom(key.GetType()))
                {
                    obj = key;
                }
                else
                {
                    try
                    {
                        obj = objectSpace.GetObjectByKey(objectType, key);
                    }
                    catch
                    {
                    }
                }
                result = IndexOf(obj);
            }
            return result;
        }
        public Object GetRowKey(Int32 index)
        {
            Object result = null;
            Object obj = this[index];
            if ((obj != null) && !objectSpace.IsNewObject(obj))
            {
                result = objectSpace.GetKeyValue(obj);
            }
            return result;
        }
        public List<Object> GetTotalSummary()
        {
            return listServer.GetTotalSummary();
        }
        public Object[] GetUniqueColumnValues(CriteriaOperator expression, Int32 maxCount, Boolean includeFilteredOut)
        {
            return listServer.GetUniqueColumnValues(expression, maxCount, includeFilteredOut);
        }
        public Int32 LocateByValue(CriteriaOperator expression, Object value, Int32 startIndex, Boolean searchUp)
        {
            return listServer.LocateByValue(expression, value, startIndex, searchUp);
        }
        public void Refresh()
        {
            addedObjects.Clear();
            addedObjectsDictionary.Clear();
            removedObjectsDictionary.Clear();
            listServer.Refresh();
        }
        public event EventHandler<ServerModeExceptionThrownEventArgs> ExceptionThrown
        {
            add { listServer.ExceptionThrown += value; }
            remove { listServer.ExceptionThrown -= value; }
        }
        public event EventHandler<ServerModeInconsistencyDetectedEventArgs> InconsistencyDetected
        {
            add { listServer.InconsistencyDetected += value; }
            remove { listServer.InconsistencyDetected -= value; }
        }
        public void HintGridIsPaged(Int32 pageSize)
        {
            listServerHints.HintGridIsPaged(pageSize);
        }
        public void HintMaxVisibleRowsInGrid(Int32 rowsInGrid)
        {
            listServerHints.HintMaxVisibleRowsInGrid(rowsInGrid);
        }
        public void AddIndex(PropertyDescriptor property)
        {
            bindingList.AddIndex(property);
        }
        public Object AddNew()
        {
            return bindingList.AddNew();
        }
        public void ApplySort(PropertyDescriptor property, ListSortDirection direction)
        {
            bindingList.ApplySort(property, direction);
        }
        public Int32 Find(PropertyDescriptor property, Object key)
        {
            return bindingList.Find(property, key);
        }
        public void RemoveIndex(PropertyDescriptor property)
        {
            bindingList.RemoveIndex(property);
        }
        public void RemoveSort()
        {
            bindingList.RemoveSort();
        }
        public Boolean AllowEdit
        {
            get { return bindingList.AllowEdit; }
        }
        public Boolean AllowNew
        {
            get { return bindingList.AllowNew && IsAddingRemovingAllowed; }
        }
        public Boolean AllowRemove
        {
            get { return IsAddingRemovingAllowed; }
        }
        public Boolean IsSorted
        {
            get { return bindingList.IsSorted; }
        }
        public ListSortDirection SortDirection
        {
            get { return bindingList.SortDirection; }
        }
        public PropertyDescriptor SortProperty
        {
            get { return bindingList.SortProperty; }
        }
        public Boolean SupportsChangeNotification
        {
            get { return true; }
        }
        public Boolean SupportsSearching
        {
            get { return bindingList.SupportsSearching; }
        }
        public Boolean SupportsSorting
        {
            get { return bindingList.SupportsSorting; }
        }
        public event ListChangedEventHandler ListChanged;
        public Int32 Add(Object obj)
        {
            Int32 result = IndexOf(obj);
            if (result < 0)
            {
                if (!IsAddingRemovingAllowed)
                {
                    throw new InvalidOperationException(SimultaneousGroupingAndAddingRemovingIsNotAllowed);
                }
                if (removedObjectsDictionary.ContainsKey(obj))
                {
                    removedObjectsDictionary.Remove(obj);
                }
                else
                {
                    addedObjects.Add(obj);
                    addedObjectsDictionary[obj] = 0;
                }
                result = IndexOf(obj);
                RaiseListChangedEvent(new ListChangedEventArgs(ListChangedType.ItemAdded, result));
            }
            return result;
        }
        public void Clear()
        {
            bindingList.Clear();
        }
        public Boolean Contains(Object obj)
        {
            if (addedObjectsDictionary.ContainsKey(obj))
            {
                return true;
            }
            if (removedObjectsDictionary.ContainsKey(obj))
            {
                return false;
            }
            return bindingList.Contains(obj);
        }
        public Int32 IndexOf(Object obj)
        {
            Int32 result = -1;
            if (obj != null)
            {
                if (addedObjectsDictionary.ContainsKey(obj))
                {
                    result = bindingList.Count - removedObjectsDictionary.Count + addedObjects.IndexOf(obj);
                }
                else if (!removedObjectsDictionary.ContainsKey(obj))
                {
                    result = bindingList.IndexOf(obj);
                    if (result > 0)
                    {
                        result = result - GetRemovedObjectsCountBeforeIndex(result);
                    }
                }
            }
            return result;
        }
        public void Insert(Int32 index, Object obj)
        {
            Add(obj);
        }
        public void Remove(Object obj)
        {
            Int32 index = IndexOf(obj);
            if (index >= 0)
            {
                RemoveObject(obj, index);
            }
        }
        public void RemoveAt(Int32 index)
        {
            if ((index >= 0) && (index < Count))
            {
                RemoveObject(this[index], index);
            }
        }
        public Boolean IsFixedSize
        {
            get { return !IsAddingRemovingAllowed; }
        }
        public Boolean IsReadOnly
        {
            get { return !IsAddingRemovingAllowed; }
        }
        public Object this[Int32 index]
        {
            get
            {
                Object result = null;
                Int32 indexInAddedObjects = index - (bindingList.Count - removedObjectsDictionary.Count);
                if (indexInAddedObjects >= 0)
                {
                    result = addedObjects[indexInAddedObjects];
                }
                else if (removedObjectsDictionary.Count == 0)
                {
                    result = bindingList[index];
                }
                else
                {
                    Int32[] removedObjectsIndices = new Int32[removedObjectsDictionary.Count];
                    Int32 i = 0;
                    foreach (Object removedObject in removedObjectsDictionary.Keys)
                    {
                        removedObjectsIndices[i] = bindingList.IndexOf(removedObject);
                        i++;
                    }
                    Array.Sort<Int32>(removedObjectsIndices);
                    Int32 resultIndex = index;
                    foreach (Int32 removedObjectIndex in removedObjectsIndices)
                    {
                        if (removedObjectIndex > resultIndex)
                        {
                            break;
                        }
                        resultIndex++;
                    }
                    result = bindingList[resultIndex];
                }
                return result;
            }
            set
            {
                throw new NotSupportedException();
            }
        }
        public void CopyTo(Array array, Int32 index)
        {
            throw new NotSupportedException();
        }
        public Int32 Count
        {
            get { return bindingList.Count + addedObjectsDictionary.Count - removedObjectsDictionary.Count; }
        }
        public Boolean IsSynchronized
        {
            get { return bindingList.IsSynchronized; }
        }
        public Object SyncRoot
        {
            get { return bindingList.SyncRoot; }
        }
        public IEnumerator GetEnumerator()
        {
            throw new NotSupportedException();
        }
        public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
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
        public String GetListName(PropertyDescriptor[] listAccessors)
        {
            return "";
        }
        public Object DXClone()
        {
            if (IsModified)
            {
                throw new InvalidOperationException("The clone operation is not allowed for a modified collection.");
            }
            return new Xpand.ExpressApp.NH.NHServerModeSourceAdderRemover(dxCloneable.DXClone(), objectSpace, objectType);
        }
    }
}
