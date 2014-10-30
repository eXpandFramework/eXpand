namespace Xpand.ExpressApp.XtraDashboard.Win.Templates {
    partial class DashboardDesignerForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if(disposing) {
                if(components != null)
                    components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.dashboardBarController1 = new DevExpress.DashboardWin.Bars.DashboardBarController();
            ((System.ComponentModel.ISupportInitialize)(this.dashboardBarController1)).BeginInit();
            this.SuspendLayout();
            // 
            // DashboardDesignerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(880, 505);
            this.Name = "DashboardDesignerForm";
            this.Text = "Dashboard Designer";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.DashboardDesignerForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dashboardBarController1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.DashboardWin.Bars.DashboardBarController dashboardBarController1;


    }
}
