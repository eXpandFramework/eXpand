using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Layout;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using eXpand.ExpressApp.SystemModule;

namespace eXpand.ExpressApp.Win.SystemModule
{
    public interface IModelDetailViewTabStopForReadOnly : IModelNode
    {
        bool TabStopForReadOnly { get; set; }
    }

    public class TabStopViewController : BaseViewController<DetailView>, IModelExtender
    {
        public const string TabStopForReadOnly = "TabStopForReadOnly";

        public TabStopViewController() {}

        protected override void OnActivated()
        {
            base.OnActivated();
            View.ControlsCreated+=View_OnControlsCreated;
        }

        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders)
        {
            extenders.Add<IModelOptions, IModelDetailViewTabStopForReadOnly>();
        }

        private void View_OnControlsCreated(object sender, EventArgs e)
        {
            if (!((IModelDetailViewTabStopForReadOnly)Application.Model.Options).TabStopForReadOnly)
            {
                LayoutControl layout = ((WinLayoutManager) (View.LayoutManager)).Container;
                layout.OptionsFocus.EnableAutoTabOrder = false;
                foreach (ViewItem item in View.GetItems<ViewItem>())
                {
                    if (item is PropertyEditor && !((PropertyEditor)item).AllowEdit)
                    {
                        if (item.Control is TextEdit)
                            ((TextEdit)item.Control).TabStop = false;

                        continue;
                    }
                }
            }
        }
    }
}