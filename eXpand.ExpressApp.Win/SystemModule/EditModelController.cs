using System;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Win.Templates;

namespace eXpand.ExpressApp.Win.SystemModule
{
    public partial class EditModelController : ViewController
    {
        private static bool editing;

        public EditModelController()
        {
            InitializeComponent();
            RegisterActions(components);
        }

        public static bool Editing
        {
            get { return editing; }
        }

        protected override void OnActivated
        {
            base.OnActivated();
            MessageBox.Show("");
            if (Application.MainWindow != null && Application.MainWindow.Template is MainForm)
                ((MainForm) Application.MainWindow.Template).FormClosing += MainForm_FormClosing;

            SimpleAction action =
                Frame.GetController<DevExpress.ExpressApp.Win.SystemModule.EditModelController>().Action;
            action.Executing += (sender, args) => editing = true;
            action.ExecuteCompleted += (sender, args) => editing = false;
                
        }





        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!Editing && e.CloseReason == CloseReason.UserClosing)
            {
                if (Application != null) e.Cancel = !Application.Model.RootNode.GetAttributeBoolValue("CanClose", true);
                if (e.Cancel)
                    ((MainForm) sender).Hide();
            }
        }

    }
}