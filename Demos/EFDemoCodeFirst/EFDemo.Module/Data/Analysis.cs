using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.DC;

namespace EFDemo.Module.Data {
	[NavigationItem("Reports")]
	public class Analysis : IAnalysisInfo, IAnalysisInfoTestable, ISupportInitialize {
		private const String propertiesSeparator = ";";
		private InitializeIndicator initializeIndicator;
		private DimensionPropertiesList dimensionProperties;
		private String criteria;
		private String objectTypeName;

		private Boolean IsInitializing {
			get { return initializeIndicator.IsInitializing; }
		}
		private void OnAnalysisInfoChanged(AnalysisInfoChangeType changeType) {
			if(InfoChanged != null) {
				InfoChanged(this, new AnalysisInfoChangedEventEventArgs(changeType));
			}
		}
		private void dimensionProperties_ListChanged(Object sender, EventArgs e) {
			String[] dimensionPropertiesArray = new String[dimensionProperties.Count];
			dimensionProperties.CopyTo(dimensionPropertiesArray, 0);
			DimensionPropertiesString = String.Join(propertiesSeparator, dimensionPropertiesArray);

			OnAnalysisInfoChanged(AnalysisInfoChangeType.DimensionPropertiesChanged);
		}
		private void LoadDimensionProperties(String[] properties) {
			dimensionProperties.ListChanged -= new EventHandler(dimensionProperties_ListChanged);
			dimensionProperties.Clear();
			foreach(String propertyName in properties) {
				dimensionProperties.Add(propertyName);
			}
			dimensionProperties.ListChanged += new EventHandler(dimensionProperties_ListChanged);
			dimensionProperties.ResetList();
		}
		private void UpdateDimensionProperties() {
			if(!String.IsNullOrEmpty(DimensionPropertiesString)) {
				LoadDimensionProperties(DimensionPropertiesString.Split(new String[] { propertiesSeparator }, StringSplitOptions.RemoveEmptyEntries));
			}
		}

		public Analysis() {
			dimensionProperties = new DimensionPropertiesList();
			initializeIndicator = new InitializeIndicator();
		}
		[Browsable(false)]
		public Int32 ID { get; protected set; }
		public String Name { get; set; }
		[CriteriaOptions("DataType")]
		[EditorAlias(EditorAliases.PopupCriteriaPropertyEditor)]
		[FieldSize(FieldSizeAttribute.Unlimited)]
		[ObjectValidatorIgnoreIssue(typeof(ObjectValidatorLargeNonDelayedMember))]
		[VisibleInListView(true)]
		[ModelDefault("RowCount", "0")]
		public String Criteria {
			get { return criteria; }
			set {
				if(criteria != value) {
					criteria = value;
					OnAnalysisInfoChanged(AnalysisInfoChangeType.CriteriaChanged);
				}
			}
		}
		[Browsable(false)]
		public String ObjectTypeName {
			get { return objectTypeName; }
			set {
				if(objectTypeName != value) {
					objectTypeName = value;
					LoadDimensionProperties(DimensionPropertyExtractor.Instance.GetDimensionProperties(DataType));
					OnAnalysisInfoChanged(AnalysisInfoChangeType.ObjectTypeChanged);
				}
			}
		}
		[Browsable(false)]
		[FieldSize(FieldSizeAttribute.Unlimited)]
		[ObjectValidatorIgnoreIssue(typeof(ObjectValidatorLargeNonDelayedMember))]
		public String DimensionPropertiesString { get; set; }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public Byte[] PivotGridSettingsContent { get; set; }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public Byte[] ChartSettingsContent { get; set; }

		[NotMapped, ImmediatePostData, TypeConverter(typeof(LocalizedClassInfoTypeConverter))]
		public Type DataType {
			get {
				if(ObjectTypeName != null) {
					return ReflectionHelper.GetType(ObjectTypeName);
				}
				return null;
			}
			set {
				String stringValue = (value == null) ? null : value.FullName;
				String savedObjectTypeName = ObjectTypeName;
				try {
					if(stringValue != ObjectTypeName) {
						ObjectTypeName = stringValue;
					}
				}
				catch(Exception) {
					ObjectTypeName = savedObjectTypeName;
				}
				if(!IsInitializing) {
					Criteria = null;
				}
			}
		}
		[NotMapped, Browsable(false)]
		public IList<String> DimensionProperties {
			get { return dimensionProperties; }
		}
		[NotMapped, VisibleInListView(false)]
		public IAnalysisInfo Self {
			get { return this; }
		}
		[NotMapped, Browsable(false)]
		public Type ObjectType {
			get { return DataType; }
		}
		public event EventHandler<AnalysisInfoChangedEventEventArgs> InfoChanged;

		// ISupportInitialize
		public void BeginInit() {
			initializeIndicator.BeginInit();
		}
		public void EndInit() {
			initializeIndicator.EndInit();
			UpdateDimensionProperties();
		}
	}
}
