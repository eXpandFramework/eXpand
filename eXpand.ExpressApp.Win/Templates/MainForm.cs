using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using eXpand.ExpressApp.Win.Interfaces;

namespace eXpand.ExpressApp.Win.Templates
{
    public class MainForm : DevExpress.ExpressApp.Win.Templates.MainForm
    {
        private readonly XafApplication application;
        private ToolStripMenuItem AboutToolStripMenuItem;
        private IContainer components;
        private ToolStripMenuItem ExitToolStripMenuItem;
        private ToolStripMenuItem MaximizeToolStripMenuItem;
        private ToolStripMenuItem MinimizeToolStripMenuItem;
        private NotifyIcon notifyIcon1;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripSeparator toolStripSeparator2;
        private DevExpress.Utils.ToolTipController toolTipController1;
        private ToolStripMenuItem logOutToolStripMenuItem;
        private ContextMenuStrip TrayContextMenuStrip;


        public MainForm(XafApplication application)
        {
            this.application = application;
            string processName = Process.GetCurrentProcess().ProcessName;
            Process[] processes = Process.GetProcessesByName(processName);
            if (processes.Length > 1)
            {
                foreach (Process process in processes)
                {
                    if (!process.Equals(Process.GetCurrentProcess()))
                    {
                        MessageBox.Show("Use the tray icon");
                        Environment.Exit(0);
                    }
                }
            }
            InitializeComponent();
        }

        ///<summary>
        ///Raises the <see cref="E:System.Windows.Forms.Form.Load"></see> event.
        ///</summary>
        ///
        ///<param name="e">An <see cref="T:System.EventArgs"></see> that contains the event data. </param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            mainBarManager.ToolTipController=toolTipController1;
            notifyIcon1.Visible = application.Model.RootNode.GetAttributeBoolValue("NotifyIcon");
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            if (Visible) MinimizeToolStripMenuItem_Click(MinimizeToolStripMenuItem, null);
            else MaximizeToolStripMenuItem_Click(MaximizeToolStripMenuItem, null);
        }

        private void MinimizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MinimizeToolStripMenuItem.Enabled = false;
            MaximizeToolStripMenuItem.Enabled = true;
            Hide();
        }

        private void MaximizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MinimizeToolStripMenuItem.Enabled = true;
            MaximizeToolStripMenuItem.Enabled = false;
            Show();
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        #region Windows Form Designer generated code
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.TrayContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.AboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.MinimizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MaximizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.logOutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ExitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.toolTipController1 = new DevExpress.Utils.ToolTipController(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.mainBarAndDockingController)).BeginInit();
            this.dockPanelNavigation.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.viewSitePanel)).BeginInit();
            this.TrayContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainBarAndDockingController
            // 
            this.mainBarAndDockingController.PropertiesBar.AllowLinkLighting = false;
            // 
            // dockPanelNavigation
            // 
            this.dockPanelNavigation.Options.AllowDockBottom = false;
            this.dockPanelNavigation.Options.AllowDockTop = false;
            // 
            // viewSitePanel
            // 
            this.viewSitePanel.Location = new System.Drawing.Point(146, 49);
            this.viewSitePanel.Size = new System.Drawing.Size(646, 495);
            // 
            // TrayContextMenuStrip
            // 
            this.TrayContextMenuStrip.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.TrayContextMenuStrip.Font = new System.Drawing.Font("Tahoma", 10F);
            this.TrayContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.AboutToolStripMenuItem,
            this.toolStripSeparator2,
            this.MinimizeToolStripMenuItem,
            this.MaximizeToolStripMenuItem,
            this.toolStripSeparator1,
            this.logOutToolStripMenuItem,
            this.ExitToolStripMenuItem});
            this.TrayContextMenuStrip.Name = "contextMenuStrip1";
            this.TrayContextMenuStrip.Size = new System.Drawing.Size(170, 136);
            // 
            // AboutToolStripMenuItem
            // 
            this.AboutToolStripMenuItem.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Underline);
            this.AboutToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("AboutToolStripMenuItem.Image")));
            this.AboutToolStripMenuItem.Name = "AboutToolStripMenuItem";
            this.AboutToolStripMenuItem.Size = new System.Drawing.Size(169, 24);
            this.AboutToolStripMenuItem.Text = "About";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(166, 6);
            // 
            // MinimizeToolStripMenuItem
            // 
            this.MinimizeToolStripMenuItem.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.MinimizeToolStripMenuItem.ForeColor = System.Drawing.SystemColors.MenuText;
            this.MinimizeToolStripMenuItem.Name = "MinimizeToolStripMenuItem";
            this.MinimizeToolStripMenuItem.Size = new System.Drawing.Size(169, 24);
            this.MinimizeToolStripMenuItem.Text = "Minimize";
            this.MinimizeToolStripMenuItem.Click += new System.EventHandler(this.MinimizeToolStripMenuItem_Click);
            // 
            // MaximizeToolStripMenuItem
            // 
            this.MaximizeToolStripMenuItem.Enabled = false;
            this.MaximizeToolStripMenuItem.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.MaximizeToolStripMenuItem.ForeColor = System.Drawing.Color.LightYellow;
            this.MaximizeToolStripMenuItem.Name = "MaximizeToolStripMenuItem";
            this.MaximizeToolStripMenuItem.Size = new System.Drawing.Size(169, 24);
            this.MaximizeToolStripMenuItem.Text = "Maximize";
            this.MaximizeToolStripMenuItem.Click += new System.EventHandler(this.MaximizeToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(166, 6);
            // 
            // logOutToolStripMenuItem
            // 
            this.logOutToolStripMenuItem.Name = "logOutToolStripMenuItem";
            this.logOutToolStripMenuItem.Size = new System.Drawing.Size(169, 24);
            this.logOutToolStripMenuItem.Text = "Log out";
            this.logOutToolStripMenuItem.Click += new System.EventHandler(this.logOutToolStripMenuItem_Click);
            // 
            // ExitToolStripMenuItem
            // 
            this.ExitToolStripMenuItem.Name = "ExitToolStripMenuItem";
            this.ExitToolStripMenuItem.Size = new System.Drawing.Size(169, 24);
            this.ExitToolStripMenuItem.Text = "Exit";
            this.ExitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItem_Click);
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.ContextMenuStrip = this.TrayContextMenuStrip;
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.DoubleClick += new System.EventHandler(this.notifyIcon1_DoubleClick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.ClientSize = new System.Drawing.Size(792, 566);
            this.Name = "MainForm";
            this.Controls.SetChildIndex(this.dockPanelNavigation, 0);
            this.Controls.SetChildIndex(this.viewSitePanel, 0);
            ((System.ComponentModel.ISupportInitialize)(this.mainBarAndDockingController)).EndInit();
            this.dockPanelNavigation.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.viewSitePanel)).EndInit();
            this.TrayContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        private void logOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ((ILogOut) application).Logout();
        }
    }
}