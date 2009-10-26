using eXpand.ExpressApp.WorldCreator.Win;

namespace Solution3.Module.Win
{
    partial class Solution3WindowsFormsModule
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
            // Solution3WindowsFormsModule
            // 
            this.AdditionalBusinessClasses.Add(typeof(eXpand.Persistent.BaseImpl.PersistentMetaData.PersistentAssociationAttribute));
            this.AdditionalBusinessClasses.Add(typeof(eXpand.Persistent.BaseImpl.PersistentMetaData.PersistentClassInfo));
            this.AdditionalBusinessClasses.Add(typeof(eXpand.Persistent.BaseImpl.PersistentMetaData.PersistentReferenceMemberInfo));
            this.AdditionalBusinessClasses.Add(typeof(eXpand.Persistent.BaseImpl.PersistentMetaData.PersistentCoreTypeMemberInfo));
            this.AdditionalBusinessClasses.Add(typeof(eXpand.Persistent.BaseImpl.PersistentMetaData.PersistentCollectionMemberInfo));
            this.AdditionalBusinessClasses.Add(typeof(eXpand.Persistent.BaseImpl.PersistentMetaData.ExtendedMemberInfo));
            this.RequiredModuleTypes.Add(typeof(Solution3.Module.Solution3Module));
            this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Win.SystemModule.SystemWindowsFormsModule));
            this.RequiredModuleTypes.Add(typeof(eXpand.ExpressApp.Win.SystemModule.eXpandSystemWindowsFormsModule));
            this.RequiredModuleTypes.Add(typeof(eXpand.ExpressApp.ModelDifference.Win.ModelDifferenceWindowsFormsModule));
            this.RequiredModuleTypes.Add(typeof(eXpand.ExpressApp.AdditionalViewControlsProvider.Win.AdditionalViewControlsProviderWindowsFormsModule));
            this.RequiredModuleTypes.Add(typeof(eXpand.ExpressApp.ViewVariants.Win.eXpandViewVariantsWin));
            this.RequiredModuleTypes.Add(typeof(eXpand.ExpressApp.ModelArtifactState.Win.ModelArtifactStateWindowsFormsModule));
            this.RequiredModuleTypes.Add(typeof(WorldCreatorWinModule));

        }

        #endregion
    }
}
