using System;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.XtraEditors;

namespace eXpand.ExpressApp.Win.SystemModule
{
    public partial class MinimizeOnCloseController : WindowController
    {
        private static bool editing;
        public const string MinimizeOnCloseAttributeName = "MinimizeOnClose";
        public MinimizeOnCloseController()
        {
            InitializeComponent();
            RegisterActions(components);
        }
        protected override void OnFrameAssigned()
        {
            base.OnFrameAssigned();
            Frame.TemplateChanged += FrameOnTemplateChanged;
        }

        private void FrameOnTemplateChanged(object sender, EventArgs args)
        {
            if (Frame.Context == TemplateContext.ApplicationWindow &&
                Application.Info.GetChildNode("Options").GetAttributeBoolValue(MinimizeOnCloseAttributeName)){
                var form = Frame.Template as XtraForm;
                if (form != null){
                    form.FormClosing += FormOnFormClosing;
                    SimpleAction action =
                    Frame.GetController<DevExpress.ExpressApp.Win.SystemModule.EditModelController>().Action;
                    action.Executing+=(o,eventArgs) =>  editing = true;
                    action.ExecuteCompleted += (o, eventArgs) => editing = false;
                }
            }
        }

        private void FormOnFormClosing(object sender, FormClosingEventArgs e){
            if (!editing && e.CloseReason == CloseReason.UserClosing)
            {
                if (Application != null) e.Cancel = Application.Model.RootNode.GetAttributeBoolValue(MinimizeOnCloseAttributeName, true);
                if (e.Cancel)
                    ((XtraForm)sender).Hide();
            }
        }


        public override Schema GetSchema()
        {
            const string CommonTypeInfos = @"<Element Name=""Application"">
                                                <Element Name=""Options"">
                                                    <Attribute Name=""" + MinimizeOnCloseAttributeName + @""" Choice=""False,True""/>
                                                </Element>
                                            </Element>";
            return new Schema(new DictionaryXmlReader().ReadFromString(CommonTypeInfos));
        }

    }
}
