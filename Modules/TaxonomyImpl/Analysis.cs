#region Copyright (c) 2000-2009 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                 }
{                                                                   }
{       Copyright (c) 2000-2009 Developer Express Inc.              }
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
#endregion Copyright (c) 2000-2009 Developer Express Inc.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using DevExpress.ExpressApp.Editors;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using eXpand.ExpressApp.Taxonomy.BaseObjects;

namespace eXpand.Persistent.TaxonomyImpl{
    public class DimensionPropertiesList : Collection<string>{
        protected override void ClearItems(){
            base.ClearItems();
            OnListChanged();
        }

        protected override void InsertItem(int index, string item){
            base.InsertItem(index, item);
            OnListChanged();
        }

        protected override void RemoveItem(int index){
            base.RemoveItem(index);
            OnListChanged();
        }

        protected override void SetItem(int index, string item){
            base.SetItem(index, item);
            OnListChanged();
        }

        protected virtual void OnListChanged(){
            if (ListChanged != null){
                ListChanged(this, EventArgs.Empty);
            }
        }

        public void ResetList(){
            OnListChanged();
        }

        public event EventHandler ListChanged;
    }

    public class Analysis : TaxonomyBaseObject, IAnalysisInfo{
        private const string PropertiesSeparator = ";";
        private readonly DimensionPropertiesList dimensionProperties = new DimensionPropertiesList();
        private string criteria;
        [Size(SizeAttribute.Unlimited), Persistent("DimensionPropertiesString"), ObjectValidatorIgnoreIssue(typeof (ObjectValidatorLargeNonDelayedMember))] private string dimensionPropertiesString;
        private string name;
        private string objectTypeName;

        public Analysis(Session session) : base(session) {}

        public string Name{
            get { return name; }
            set { SetPropertyValue("Name", ref name, value); }
        }

        [Browsable(false)]
        public string ObjectTypeName{
            get { return objectTypeName; }
            set{
                SetPropertyValue("ObjectTypeName", ref objectTypeName, value);
                if (!IsLoading){
                    LoadDimensionProperties(DimensionPropertyExtractor.Instance.GetDimensionProperties(DataType));
                    OnAnalysisInfoChanged(AnalysisInfoChangeType.ObjectTypeChanged);
                }
                else if (dimensionPropertiesString == null){
                    LoadDimensionProperties(DimensionPropertyExtractor.Instance.GetDimensionProperties(DataType));
                }
            }
        }
        #region IAnalysisInfo Members
        [CriteriaObjectTypeMember("DataType")]
        [Size(SizeAttribute.Unlimited), ObjectValidatorIgnoreIssue(typeof (ObjectValidatorLargeNonDelayedMember))]
        [VisibleInListView(true)]
        [Custom("RowCount", "0")]
        public string Criteria{
            get { return criteria; }
            set{
                SetPropertyValue("Criteria", ref criteria, value);
                OnAnalysisInfoChanged(AnalysisInfoChangeType.CriteriaChanged);
            }
        }

        [VisibleInListView(false)]
        public IAnalysisInfo Self{
            get { return this; }
        }

        [Delayed, Persistent("ChartSettingsContent")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public byte[] ChartSettingsContent{
            get { return GetDelayedPropertyValue<byte[]>("ChartSettingsContent"); }
            set { SetDelayedPropertyValue("ChartSettingsContent", value); }
        }

        [Delayed, Persistent("PivotGridSettingsContent")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public byte[] PivotGridSettingsContent{
            get { return GetDelayedPropertyValue<byte[]>("PivotGridSettingsContent"); }
            set { SetDelayedPropertyValue("PivotGridSettingsContent", value); }
        }

        Type IAnalysisInfo.ObjectType{
            get { return DataType; }
        }

        [TypeConverter(typeof (LocalizedClassInfoTypeConverter))]
        [ImmediatePostData, NonPersistent]
        public Type DataType{
            get{
                if (objectTypeName != null){
                    return ReflectionHelper.GetType(objectTypeName);
                }
                return null;
            }
            set{
                string stringValue = value == null ? null : value.FullName;
                string savedObjectTypeName = ObjectTypeName;
                try{
                    if (stringValue != objectTypeName){
                        ObjectTypeName = stringValue;
                    }
                }
                catch (Exception){
                    ObjectTypeName = savedObjectTypeName;
                }
                criteria = null;
            }
        }

        [Browsable(false)]
        [NonPersistent]
        public IList<string> DimensionProperties{
            get { return dimensionProperties; }
        }

        public event EventHandler<AnalysisInfoChangedEventEventArgs> InfoChanged;
        #endregion
        private void OnAnalysisInfoChanged(AnalysisInfoChangeType changeType){
            if (InfoChanged != null){
                InfoChanged(this, new AnalysisInfoChangedEventEventArgs(changeType));
            }
        }

        private void dimensionProperties_ListChanged(object sender, EventArgs e){
            OnAnalysisInfoChanged(AnalysisInfoChangeType.DimensionPropertiesChanged);
        }

        private void LoadDimensionProperties(string[] properties){
            dimensionProperties.ListChanged -= dimensionProperties_ListChanged;
            dimensionProperties.Clear();
            foreach (string propertyName in properties){
                dimensionProperties.Add(propertyName);
            }
            dimensionProperties.ListChanged += dimensionProperties_ListChanged;
            dimensionProperties.ResetList();
        }

        protected override void OnLoaded(){
            base.OnLoaded();
            dimensionProperties.ListChanged += dimensionProperties_ListChanged;
            if (!string.IsNullOrEmpty(dimensionPropertiesString)){
                LoadDimensionProperties(dimensionPropertiesString.Split(new[]{PropertiesSeparator}, StringSplitOptions.RemoveEmptyEntries));
            }
        }

        protected override void Dispose(bool disposing){
            try{
                if (disposing){
                    dimensionProperties.ListChanged -= dimensionProperties_ListChanged;
                }
            }
            finally{
                base.Dispose(disposing);
            }
        }

        protected override void OnSaving(){
            base.OnSaving();
            var temp = new string[dimensionProperties.Count];
            dimensionProperties.CopyTo(temp, 0);
            dimensionPropertiesString = string.Join(PropertiesSeparator, temp);
        }
    }
}