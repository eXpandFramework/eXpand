using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win;

namespace eXpand.ExpressApp.Win.SystemModule
{
    [ToolboxItem(true)]
    [DevExpress.Utils.ToolboxTabName(XafAssemblyInfo.DXTabXafModules)]
    [Description("Overrides Controllers from the SystemModule and supplies additional basic Controllers that are specific for Windows Forms applications.")]
    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [ToolboxBitmap(typeof(WinApplication), "Resources.WinSystemModule.ico")]
    [ToolboxItemFilter("Xaf.Platform.Win")]
    public sealed class eXpandSystemWindowsFormsModule : ModuleBase
    {
        public const string ApplicationOneInstanceAttributeName = "ApplicationOneInstance";

        public eXpandSystemWindowsFormsModule()
        {
        }

        public override void UpdateModel(IModelApplication applicationModel)
        {
            //base.UpdateModel(applicationModel);
            //applicationModel.Views.DefaultListEditor = typeof(GridListEditor);
        }

        //        public override void ValidateModel(Dictionary model){
        //            if (model.RootNode.GetChildNode("Options").GetAttributeBoolValue(ApplicationOneInstanceAttributeName,
        //                                                                               false))
        //            {
        //                string processName = Process.GetCurrentProcess().ProcessName;
        //                Process[] processes = Process.GetProcessesByName(processName);
        //                if (processes.Length > 1)
        //                {
        //                    foreach (Process process in processes)
        //                    {
        //                        if (!process.Equals(Process.GetCurrentProcess()))
        //                        {
        //                            MessageBox.Show("Application is already running");
        //                            Environment.Exit(0);
        //                        }
        //                    }
        //                }
        //            }
        //        }

        //        public override Schema GetSchema()
        //        {
        //            const string CommonTypeInfos = @"<Element Name=""Application"">
        //                                                <Element Name=""Options"">
        //                                                    <Attribute Name=""" + ApplicationOneInstanceAttributeName + @""" Choice=""False,True""/>
        //                                                </Element>
        //                                            </Element>";
        //            return new Schema(new DictionaryXmlReader().ReadFromString(CommonTypeInfos));
        //        }
    }
}
