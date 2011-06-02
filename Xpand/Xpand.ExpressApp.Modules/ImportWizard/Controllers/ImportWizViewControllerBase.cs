using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;

namespace Xpand.ExpressApp.ImportWiz.Controllers
{
    public abstract partial class ImportWizViewControllerBase : ViewController
    {
        protected ImportWizViewControllerBase()
        {
            InitializeComponent();
            RegisterActions(components);
            TargetViewType = ViewType.ListView;
        }

        protected override void OnActivated()
        {
            //Disable importing into Base or Abstract object ListViews
            if (View.ObjectTypeInfo.IsAbstract)
               ImportAction.Active.SetItemValue("test", false);

            base.OnActivated();
        }

        private void ImportAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var objectSpace = View.IsRoot
                ? Application.CreateObjectSpace()
                : View.ObjectSpace.CreateNestedObjectSpace();

            ShowWizard(objectSpace);
            View.Refresh();
            View.ObjectSpace.Refresh();
        }

        public abstract void ShowWizard(IObjectSpace objectSpace);
        //{
            //var wiz = new ExcelImportWizard((ObjectSpace)objectSpace, View.ObjectTypeInfo, this.GetCurrentCollectionSource());

            //wiz.ShowDialog();
        //}

        private void ImportFromExcel_Deactivating(object sender, System.EventArgs e)
        {
            if (View.ObjectTypeInfo.IsAbstract)
                ImportAction.Active.SetItemValue("test", true);
        }
    }
}
