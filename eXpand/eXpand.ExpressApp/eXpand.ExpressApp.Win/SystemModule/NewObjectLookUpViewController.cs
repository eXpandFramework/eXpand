using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.XtraGrid;
using eXpand.ExpressApp.SystemModule;

namespace eXpand.ExpressApp.Win.SystemModule
{
    public partial class NewObjectLookUpViewController : BaseViewController
    {
        public NewObjectLookUpViewController()
        {
            InitializeComponent();
            RegisterActions(components);
            TargetViewType=ViewType.ListView;
        }

        protected override void OnActivated()
        {
            base.OnActivated();

            if (Frame.Template is ILookupPopupFrameTemplate)
                View.ControlsCreated += (sender, e) => { if (View.Control is GridControl)
                    ((GridControl) View.Control).KeyDown += grid_KeyDown; };
        }


        private void grid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Space)
                Frame.GetController<NewObjectViewController>().NewObjectAction.DoExecute(null);
        }

        protected override void OnDeactivating()
        {
            base.OnDeactivating();
            if (Frame.Template is ILookupPopupFrameTemplate && View.Control is GridControl)
                ((GridControl) View.Control).KeyDown -= grid_KeyDown;
        }
    }
}