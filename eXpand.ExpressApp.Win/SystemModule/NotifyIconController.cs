using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.XtraEditors;
using eXpand.ExpressApp.SystemModule;
using eXpand.ExpressApp.Win.Interfaces;

namespace eXpand.ExpressApp.Win.SystemModule
{
    public partial class NotifyIconController : WindowController
    {
        public const string NotifyIconAttributeName = "NotifyIcon";
        public NotifyIconController()
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
            if (Frame.Context == TemplateContext.ApplicationWindow && Application.Info.GetChildNode("Options").GetAttributeBoolValue(NotifyIconAttributeName))
            {
                var form = Frame.Template as XtraForm;
                if (form != null)
                {
                    IContainer  container=new Container();
                    
                    var strip = new ContextMenuStrip(container);
                    strip.Items.Add(GetMenuItem(strip, "Maximize",(o,eventArgs) => changeFormVisibility(form)));
                    strip.Items.Add(GetMenuItem(strip, "Minimize",(o,eventArgs) => changeFormVisibility(form)));
                    if (Application is ILogOut)
                        strip.Items.Add(GetMenuItem(strip, "LogOut", (o, eventArgs) => ((ILogOut) Application).Logout()));
                    strip.Items.Add(GetMenuItem(strip, "Exit", (o, eventArgs) => Environment.Exit(0)));

                    var notifyIcon1 = new NotifyIcon(container){Visible = true, ContextMenuStrip = strip};
                    setIcon(notifyIcon1);
                    notifyIcon1.DoubleClick += (o, eventArgs) => changeFormVisibility(form);
                }
            }
        }

        private ToolStripMenuItem GetMenuItem(ContextMenuStrip strip, string text, EventHandler clickHandler){
            var item = new ToolStripMenuItem(text);
            strip.Click += clickHandler;
            return item;
        }

        private void changeFormVisibility(XtraForm form){
            if (form.Visible)
                form.Hide();
            else
                form.Show();
        }

        private void setIcon(NotifyIcon notifyIcon1){
            string path = Path.Combine(Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath),"ExpressApp.ico");
            if (File.Exists(path))
                notifyIcon1.Icon=new Icon(path);
            else{
                Stream resourceStream = typeof(eXpandSystemModule).Assembly.GetManifestResourceStream("eXpand.ExpressApp.Resources.ExpressApp.ico");
                if (resourceStream != null) notifyIcon1.Icon = new Icon(resourceStream);
            }
        }


        public override Schema GetSchema()
        {
            const string CommonTypeInfos = @"<Element Name=""Application"">
                                                <Element Name=""Options"">
                                                    <Attribute Name=""" +NotifyIconAttributeName+ @""" Choice=""False,True""/>
                                                </Element>
                                            </Element>";
            return new Schema(new DictionaryXmlReader().ReadFromString(CommonTypeInfos));
        }

    }
}
