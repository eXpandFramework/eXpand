using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.ExpressApp.Win;
using eXpand.ExpressApp.Win.ListEditors;
using MDIDemo.Win;

namespace eXpand.ExpressApp.Win.SystemModule {
    [ToolboxItem(true)]
    [DevExpress.Utils.ToolboxTabName(XafAssemblyInfo.DXTabXafModules)]
    [Description("Overrides Controllers from the SystemModule and supplies additional basic Controllers that are specific for Windows Forms applications.")]
    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [ToolboxBitmap(typeof(WinApplication), "Resources.WinSystemModule.ico")]
    [ToolboxItemFilter("Xaf.Platform.Win")]
    public sealed partial class eXpandSystemWindowsFormsModule : ModuleBase {
        public const string ApplicationOneInstanceAttributeName = "ApplicationOneInstance";
        public const string MDIStrategy = "MDIStrategy";
        public eXpandSystemWindowsFormsModule() {
            InitializeComponent();
        }
        public override void Setup(XafApplication application)
        {
            base.Setup(application);
            application.CreateCustomTemplate+=ApplicationOnCreateCustomTemplate;
            application.SetupComplete += (sender, args) => {
                var mdi = application.Model.RootNode.GetChildNode("Options").GetAttributeBoolValue("MDIStrategy");
                if (mdi)
                    application.ShowViewStrategy = new MDIStrategy(application);
            };
        }

        void ApplicationOnCreateCustomTemplate(object sender, CreateCustomTemplateEventArgs e) {
            if (e.Context == TemplateContext.ApplicationWindow) {
                e.Template = new MDIMainForm();
            }
            else {
                e.Template = e.Context == TemplateContext.View ? new MDIChildForm() : null;
            }
        }

        public override void UpdateModel(Dictionary model)
        {
            base.UpdateModel(model);
            new ApplicationNodeWrapper(model).Views.Node.SetAttribute("DefaultListEditor", typeof(GridListEditor).FullName);
        }
        
        public override void ValidateModel(Dictionary model){
            if (model.RootNode.GetChildNode("Options").GetAttributeBoolValue(ApplicationOneInstanceAttributeName,
                                                                               false))
            {
                string processName = Process.GetCurrentProcess().ProcessName;
                Process[] processes = Process.GetProcessesByName(processName);
                if (processes.Length > 1)
                {
                    foreach (Process process in processes)
                    {
                        if (!process.Equals(Process.GetCurrentProcess()))
                        {
                            MessageBox.Show("Application is already running");
                            Environment.Exit(0);
                        }
                    }
                }
            }

        }

        public override Schema GetSchema()
        {
            const string CommonTypeInfos = @"<Element Name=""Application"">
                                                <Element Name=""Options"">
                                                    <Attribute Name=""" + ApplicationOneInstanceAttributeName + @""" Choice=""False,True""/>
                                                    <Attribute Name=""" + MDIStrategy + @""" Choice=""False,True""/>
                                                </Element>
                                            </Element>";
            return new Schema(new DictionaryXmlReader().ReadFromString(CommonTypeInfos));
        }
    }
}
