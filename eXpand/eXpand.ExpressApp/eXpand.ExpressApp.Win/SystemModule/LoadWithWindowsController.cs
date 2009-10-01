using System;
using DevExpress.ExpressApp;
using DevExpress.XtraEditors;
using Microsoft.Win32;

namespace eXpand.ExpressApp.Win.SystemModule
{
    public partial class LoadWithWindowsController : WindowController
    {
        public const string LoadWithWindowsAttributeName = "LoadWithWindows";
        public LoadWithWindowsController()
        {
            InitializeComponent();
            RegisterActions(components);
        }
        protected override void OnFrameAssigned()
        {
            base.OnFrameAssigned();
            Frame.TemplateChanged+=FrameOnTemplateChanged;
            
        }

        private void FrameOnTemplateChanged(object sender, EventArgs args){
            if (Frame.Context == TemplateContext.ApplicationWindow)
                ((XtraForm) Frame.Template).Closing += (o, eventArgs) => writeRegistry();
        }

        private void writeRegistry(){
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
            if (key != null){
                if (Application.Info.FindChildNode("Options").GetAttributeBoolValue(LoadWithWindowsAttributeName)){
                    key.SetValue(Application.Title, "\"" + System.Windows.Forms.Application.ExecutablePath + "\"");
                }
                else if (key.GetValue(Application.Title) != null){
                    key.DeleteValue(Application.Title);
                }
            }
        }

        public override Schema GetSchema()
        {
            return new Schema(new DictionaryXmlReader().ReadFromString(
                                  @"<?xml version=""1.0""?>" +
                                  @"<Element Name=""Application"">" +
                                  @"	<Element Name=""Options"">" +
                                  @"			<Attribute	IsNewNode=""True"" Name=""" + LoadWithWindowsAttributeName +@"""" +@" Choice=""True,False""" + @"/>" +
                                  @"	</Element>" +
                                  @"</Element>"));
        }
    }
}
