using DevExpress.ExpressApp.ScriptRecorder;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using eXpand.ExpressApp.MemberLevelSecurity;
using eXpand.ExpressApp.WorldCreator.SqlDBMapper;
using eXpand.Persistent.BaseImpl.ExceptionHandling;
using eXpand.Persistent.BaseImpl.PersistentMetaData;
using System.Linq;
namespace FeatureCenter.Module
{
    partial class FeatureCenterModule
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            // 
            // FeatureCenterModule
            // 
            // 
            // Solution1Module
            // 

            AdditionalBusinessClasses.Add(typeof(ExceptionObject));
            AdditionalBusinessClasses.Add(typeof(DataStoreLogonObject));
            AdditionalBusinessClasses.Add(typeof(eXpand.Persistent.BaseImpl.ImportExport.ClassInfoGraphNode));
            AdditionalBusinessClasses.Add(typeof(eXpand.Persistent.BaseImpl.ImportExport.SerializationConfiguration));
            AdditionalBusinessClasses.Add(typeof(eXpand.Persistent.BaseImpl.ImportExport.SerializationConfigurationGroup));
            AdditionalBusinessClasses.Add(typeof(eXpand.Persistent.BaseImpl.ImportExport.XmlFileChooser));
            AdditionalBusinessClasses.Add(typeof(eXpand.Persistent.BaseImpl.PersistentMetaData.TemplateInfo));
            AdditionalBusinessClasses.Add(typeof(eXpand.Persistent.BaseImpl.PersistentMetaData.CodeTemplate));
            AdditionalBusinessClasses.Add(typeof(eXpand.Persistent.BaseImpl.PersistentMetaData.CodeTemplateInfo));
            AdditionalBusinessClasses.Add(typeof(eXpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos.PersistentAttributeInfo));
            AdditionalBusinessClasses.Add(typeof(eXpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos.PersistentMapInheritanceAttribute));
            AdditionalBusinessClasses.Add(typeof(eXpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos.PersistentAggregatedAttribute));
            AdditionalBusinessClasses.Add(typeof(eXpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos.PersistentCustomAttribute));
            AdditionalBusinessClasses.Add(typeof(eXpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos.PersistentDefaultClassOptionsAttribute));
            AdditionalBusinessClasses.Add(typeof(eXpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos.PersistentPivotedPropertyAttribute));
            AdditionalBusinessClasses.Add(typeof(eXpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos.PersistentRuleRequiredFieldAttribute));
            AdditionalBusinessClasses.Add(typeof(eXpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos.PersistentSizeAttribute));
            AdditionalBusinessClasses.Add(typeof(eXpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos.PersistentValueConverter));
            AdditionalBusinessClasses.Add(typeof(eXpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos.PersistentPersistentAttribute));
            AdditionalBusinessClasses.Add(typeof(eXpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos.PersistentKeyAttribute));
            AdditionalBusinessClasses.Add(typeof(eXpand.Persistent.BaseImpl.PersistentMetaData.PersistentTypeInfo));
            AdditionalBusinessClasses.Add(typeof(eXpand.Persistent.BaseImpl.PersistentMetaData.PersistentTemplatedTypeInfo));
            AdditionalBusinessClasses.Add(typeof(eXpand.Persistent.BaseImpl.PersistentMetaData.StrongKeyFile));
            AdditionalBusinessClasses.Add(typeof(eXpand.Persistent.BaseImpl.PersistentMetaData.ExtendedMemberInfo));
            AdditionalBusinessClasses.Add(typeof(eXpand.Persistent.BaseImpl.PersistentMetaData.ExtendedCollectionMemberInfo));
            AdditionalBusinessClasses.Add(typeof(eXpand.Persistent.BaseImpl.PersistentMetaData.ExtendedCoreTypeMemberInfo));
            AdditionalBusinessClasses.Add(typeof(eXpand.Persistent.BaseImpl.PersistentMetaData.ExtendedReferenceMemberInfo));
            AdditionalBusinessClasses.Add(typeof(eXpand.Persistent.BaseImpl.PersistentMetaData.InterfaceInfo));
            AdditionalBusinessClasses.Add(typeof(eXpand.Persistent.BaseImpl.PersistentMetaData.PersistentAssemblyInfo));
            AdditionalBusinessClasses.Add(typeof(eXpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos.PersistentAssociationAttribute));
            AdditionalBusinessClasses.Add(typeof(eXpand.Persistent.BaseImpl.PersistentMetaData.PersistentClassInfo));
            AdditionalBusinessClasses.Add(typeof(eXpand.Persistent.BaseImpl.PersistentMetaData.PersistentMemberInfo));
            AdditionalBusinessClasses.Add(typeof(eXpand.Persistent.BaseImpl.PersistentMetaData.PersistentCollectionMemberInfo));
            AdditionalBusinessClasses.Add(typeof(eXpand.Persistent.BaseImpl.PersistentMetaData.PersistentCoreTypeMemberInfo));
            AdditionalBusinessClasses.Add(typeof(eXpand.Persistent.BaseImpl.PersistentMetaData.PersistentReferenceMemberInfo));
            AdditionalBusinessClasses.Add(typeof(eXpand.Persistent.BaseImpl.PivotChart.Web.PivotGridOptionsOLAP));
            AdditionalBusinessClasses.Add(typeof(eXpand.Persistent.BaseImpl.PivotChart.PivotOptionsBehavior));
            AdditionalBusinessClasses.Add(typeof(eXpand.Persistent.BaseImpl.PivotChart.PivotOptionsChartDataSource));
            AdditionalBusinessClasses.Add(typeof(eXpand.Persistent.BaseImpl.PivotChart.PivotOptionsCustomization));
            AdditionalBusinessClasses.Add(typeof(eXpand.Persistent.BaseImpl.PivotChart.PivotOptionsData));
            AdditionalBusinessClasses.Add(typeof(eXpand.Persistent.BaseImpl.PivotChart.PivotOptionsDataField));
            AdditionalBusinessClasses.Add(typeof(eXpand.Persistent.BaseImpl.PivotChart.PivotOptionsFilterPopup));
            AdditionalBusinessClasses.Add(typeof(eXpand.Persistent.BaseImpl.PivotChart.PivotOptionsHint));
            AdditionalBusinessClasses.Add(typeof(eXpand.Persistent.BaseImpl.PivotChart.PivotOptionsMenu));
            AdditionalBusinessClasses.Add(typeof(eXpand.Persistent.BaseImpl.PivotChart.PivotOptionsSelection));
            AdditionalBusinessClasses.Add(typeof(eXpand.Persistent.BaseImpl.PivotChart.PivotOptionsView));
            AdditionalBusinessClasses.Add(typeof(eXpand.Persistent.BaseImpl.PivotChart.Web.PivotGridWebOptionsChartDataSource));
            AdditionalBusinessClasses.Add(typeof(eXpand.Persistent.BaseImpl.PivotChart.Web.PivotGridWebOptionsCustomization));
            AdditionalBusinessClasses.Add(typeof(eXpand.Persistent.BaseImpl.PivotChart.Web.PivotGridWebOptionsLoadingPanel));
            AdditionalBusinessClasses.Add(typeof(eXpand.Persistent.BaseImpl.PivotChart.Web.PivotGridWebOptionsPager));
            AdditionalBusinessClasses.Add(typeof(eXpand.Persistent.BaseImpl.PivotChart.Web.PivotGridWebOptionsView));
            this.AdditionalBusinessClasses.Add(typeof(Analysis));
            this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.SystemModule.SystemModule));
            this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Objects.BusinessClassLibraryCustomizationModule));
            this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.CloneObject.CloneObjectModule));
            this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.ConditionalFormatting.ConditionalFormattingModule));
            this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Validation.ValidationModule));
            this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.ViewVariantsModule.ViewVariantsModule));
            this.RequiredModuleTypes.Add(typeof(eXpand.ExpressApp.SystemModule.eXpandSystemModule));
            this.RequiredModuleTypes.Add(typeof(eXpand.ExpressApp.ModelArtifactState.ModelArtifactStateModule));
            this.RequiredModuleTypes.Add(typeof(eXpand.ExpressApp.ModelDifference.ModelDifferenceModule));
            this.RequiredModuleTypes.Add(typeof(eXpand.ExpressApp.Security.eXpandSecurityModule));
            this.RequiredModuleTypes.Add(typeof(eXpand.ExpressApp.Validation.eXpandValidationModule));
            this.RequiredModuleTypes.Add(typeof(eXpand.ExpressApp.ViewVariants.eXpandViewVariantsModule));
            this.RequiredModuleTypes.Add(typeof(eXpand.ExpressApp.WorldCreator.WorldCreatorModule));
            this.RequiredModuleTypes.Add(typeof(eXpand.ExpressApp.IO.IOModule));
            this.RequiredModuleTypes.Add(typeof(eXpand.ExpressApp.Validation.eXpandValidationModule));
            this.RequiredModuleTypes.Add(typeof(eXpand.ExpressApp.MasterDetail.MasterDetailModule));
            this.RequiredModuleTypes.Add(typeof(eXpand.ExpressApp.PivotChart.PivotChartModule));
            this.RequiredModuleTypes.Add(typeof(eXpand.ExpressApp.FilterDataStore.FilterDataStoreModule));
            this.RequiredModuleTypes.Add(typeof(eXpand.ExpressApp.AdditionalViewControlsProvider.AdditionalViewControlsModule));
            this.RequiredModuleTypes.Add(typeof(ScriptRecorderModuleBase));
            this.RequiredModuleTypes.Add(typeof(WorldCreatorSqlDBMapperModule));
            this.RequiredModuleTypes.Add(typeof(MemberLevelSecurityModule));
        }

        #endregion
    }
}
