namespace MainDemo.Module {
    partial class MainDemoModule {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if(disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            // 
            // MainDemoModule
            // 
            this.AdditionalExportedTypes.Add(typeof(DevExpress.Persistent.BaseImpl.BaseObject));
            this.AdditionalExportedTypes.Add(typeof(DevExpress.Persistent.BaseImpl.Task));
            this.AdditionalExportedTypes.Add(typeof(DevExpress.Persistent.BaseImpl.PhoneNumber));
            this.AdditionalExportedTypes.Add(typeof(DevExpress.Persistent.BaseImpl.PhoneType));
            this.AdditionalExportedTypes.Add(typeof(DevExpress.Persistent.BaseImpl.Party));
            this.AdditionalExportedTypes.Add(typeof(DevExpress.Persistent.BaseImpl.Person));
            this.AdditionalExportedTypes.Add(typeof(DevExpress.Persistent.BaseImpl.Note));
            this.AdditionalExportedTypes.Add(typeof(DevExpress.Persistent.BaseImpl.Event));
            this.AdditionalExportedTypes.Add(typeof(DevExpress.Persistent.BaseImpl.Resource));
            this.AdditionalExportedTypes.Add(typeof(DevExpress.Persistent.BaseImpl.Organization));
            this.AdditionalExportedTypes.Add(typeof(DevExpress.Persistent.BaseImpl.HCategory));
            this.AdditionalExportedTypes.Add(typeof(DevExpress.Persistent.BaseImpl.PropertyBagDescriptor));
            this.AdditionalExportedTypes.Add(typeof(DevExpress.Persistent.BaseImpl.Country));
            this.AdditionalExportedTypes.Add(typeof(DevExpress.Persistent.BaseImpl.State));
            this.AdditionalExportedTypes.Add(typeof(DevExpress.Persistent.BaseImpl.Address));
            this.AdditionalExportedTypes.Add(typeof(DevExpress.Persistent.BaseImpl.FileAttachmentBase));
            this.AdditionalExportedTypes.Add(typeof(DevExpress.Persistent.BaseImpl.AuditDataItemPersistent));
            this.AdditionalExportedTypes.Add(typeof(DevExpress.Persistent.BaseImpl.AuditedObjectWeakReference));
            this.AdditionalExportedTypes.Add(typeof(DevExpress.Persistent.BaseImpl.Analysis));
            this.AdditionalExportedTypes.Add(typeof(DevExpress.Persistent.BaseImpl.FileData));
            this.AdditionalExportedTypes.Add(typeof(DevExpress.Persistent.BaseImpl.PropertyDescriptor));
            this.Description = "MainDemo module";
            this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Objects.BusinessClassLibraryCustomizationModule));
            this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.ViewVariantsModule.ViewVariantsModule));
            this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Validation.ValidationModule));
            this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.SystemModule.SystemModule));
            this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Security.SecurityModule));
            this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.ConditionalAppearance.ConditionalAppearanceModule));
            this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.ReportsV2.ReportsModuleV2));
            this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.PivotChart.PivotChartModuleBase));
//            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.SystemModule.XpandSystemModule));
//            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.WorldCreator.WorldCreatorModule));

        }

        #endregion
    }
}
