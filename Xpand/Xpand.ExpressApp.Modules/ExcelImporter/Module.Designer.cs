using System.ComponentModel;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Validation;
using Xpand.ExpressApp.Validation;

namespace Xpand.ExpressApp.ExcelImporter {
	partial class ExcelImporterModule {
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private IContainer components = null;

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
			// ExcelImporterModule
			// 
			this.RequiredModuleTypes.Add(typeof(SystemModule));
			this.RequiredModuleTypes.Add(typeof(ValidationModule));
			this.RequiredModuleTypes.Add(typeof(XpandValidationModule));
			this.RequiredModuleTypes.Add(typeof(ConditionalAppearanceModule));
		}

		#endregion
	}
}
