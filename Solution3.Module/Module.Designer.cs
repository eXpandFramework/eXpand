using DevExpress.ExpressApp.ViewVariantsModule;
using eXpand.ExpressApp.AdditionalViewControlsProvider;
using eXpand.ExpressApp.ModelArtifactState;
using eXpand.ExpressApp.ModelDifference;
using eXpand.ExpressApp.SystemModule;
using eXpand.ExpressApp.ViewVariants;
using eXpand.ExpressApp.WorldCreator;
using eXpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos;

namespace Solution3.Module {
    partial class Solution3Module
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
            // Solution3Module
            // 
            this.AdditionalBusinessClasses.Add(typeof(eXpand.Persistent.BaseImpl.PersistentMetaData.PersistentClassInfo));
            this.AdditionalBusinessClasses.Add(typeof(eXpand.Persistent.BaseImpl.PersistentMetaData.ExtendedCollectionMemberInfo));
            this.AdditionalBusinessClasses.Add(typeof(eXpand.Persistent.BaseImpl.PersistentMetaData.ExtendedReferenceMemberInfo));
            this.AdditionalBusinessClasses.Add(typeof(PersistentAssociationAttribute));
            this.AdditionalBusinessClasses.Add(typeof(eXpand.Persistent.BaseImpl.PersistentMetaData.PersistentCollectionMemberInfo));
            this.AdditionalBusinessClasses.Add(typeof(eXpand.Persistent.BaseImpl.PersistentMetaData.PersistentCoreTypeMemberInfo));
            this.AdditionalBusinessClasses.Add(typeof(eXpand.Persistent.BaseImpl.PersistentMetaData.PersistentReferenceMemberInfo));
            this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.SystemModule.SystemModule));
            this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Objects.BusinessClassLibraryCustomizationModule));
            this.RequiredModuleTypes.Add(typeof(eXpand.ExpressApp.SystemModule.eXpandSystemModule));
            this.RequiredModuleTypes.Add(typeof(eXpand.ExpressApp.AdditionalViewControlsProvider.AdditionalViewControlsProviderModule));
            this.RequiredModuleTypes.Add(typeof(eXpand.ExpressApp.ModelDifference.ModelDifferenceModule));
            this.RequiredModuleTypes.Add(typeof(eXpand.ExpressApp.ViewVariants.eXpandViewVariantsModule));
            this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.ViewVariantsModule.ViewVariantsModule));
            this.RequiredModuleTypes.Add(typeof(eXpand.ExpressApp.ModelArtifactState.ModelArtifactStateModule));
            this.RequiredModuleTypes.Add(typeof(eXpand.ExpressApp.WorldCreator.WorldCreatorModule));

        }

        #endregion
    }
}