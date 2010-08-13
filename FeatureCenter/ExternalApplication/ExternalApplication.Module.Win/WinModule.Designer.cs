using eXpand.ExpressApp.IO;
using eXpand.ExpressApp.IO.Win;
using eXpand.ExpressApp.ModelDifference;
using eXpand.ExpressApp.ModelDifference.Win;
using eXpand.ExpressApp.SystemModule;
using eXpand.ExpressApp.Win.SystemModule;
using eXpand.ExpressApp.WorldCreator;
using eXpand.ExpressApp.WorldCreator.SqlDBMapper;
using eXpand.ExpressApp.WorldCreator.Win;

namespace ExternalApplication.Module.Win
{
    partial class ExternalApplicationWindowsFormsModule
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
            // ExternalApplicationWindowsFormsModule
            // 
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
            AdditionalBusinessClasses.Add(typeof(eXpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos.PersistentKeyAttribute));

            this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Win.SystemModule.SystemWindowsFormsModule));
            this.RequiredModuleTypes.Add(typeof(ModelDifferenceModule));
            this.RequiredModuleTypes.Add(typeof(ModelDifferenceWindowsFormsModule));
            this.RequiredModuleTypes.Add(typeof(IOModule));
            this.RequiredModuleTypes.Add(typeof(IOWinModule));
            this.RequiredModuleTypes.Add(typeof(eXpandSystemModule));
            this.RequiredModuleTypes.Add(typeof(eXpandSystemWindowsFormsModule));
            this.RequiredModuleTypes.Add(typeof(WorldCreatorModule));
            this.RequiredModuleTypes.Add(typeof(WorldCreatorWinModule));
            this.RequiredModuleTypes.Add(typeof(WorldCreatorSqlDBMapperModule));
            this.RequiredModuleTypes.Add(typeof(eXpand.ExpressApp.Validation.eXpandValidationModule));

        }

        #endregion
    }
}
