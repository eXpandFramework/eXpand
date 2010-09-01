using DevExpress.ExpressApp.ScriptRecorder;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using eXpand.ExpressApp.ConditionalDetailViews;
using eXpand.ExpressApp.MemberLevelSecurity;
using eXpand.ExpressApp.WorldCreator.SqlDBMapper;
using eXpand.Persistent.BaseImpl.ExceptionHandling;
using eXpand.Persistent.BaseImpl.PersistentMetaData;
using System.Linq;
using eXpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos;

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
            AdditionalBusinessClassAssemblies.Add(typeof(Analysis).Assembly);
            AdditionalBusinessClassAssemblies.Add(typeof(PersistentAssemblyInfo).Assembly);
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
            this.RequiredModuleTypes.Add(typeof(ConditionalDetailViewModule));
            this.RequiredModuleTypes.Add(typeof(MemberLevelSecurityModule));
        }

        #endregion
    }
}
