using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Templates;
using DevExpress.XtraBars;
using eXpand.ExpressApp.SystemModule;

namespace eXpand.ExpressApp.Win.SystemModule
{
    public interface IModelListViewHideToolBar : IModelNode
    {
        bool HideToolBar { get; set; }
    }

    public partial class HideNestedListViewToolBarViewController : BaseViewController<ListView>
    {
        public HideNestedListViewToolBarViewController()
        {
            InitializeComponent();
            RegisterActions(components);
            TargetViewNesting=Nesting.Nested;
        }

        public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders)
        {
            base.ExtendModelInterfaces(extenders);
            extenders.Add<IModelListView, IModelListViewHideToolBar>();
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            View.ControlsCreated+=View_OnControlsCreated;
        }

        private void View_OnControlsCreated(object sender, EventArgs e)
        {
            if (Frame.Template is NestedFrameTemplate)
            {
                Bar bar = ((NestedFrameTemplate) Frame.Template).BarManager.Bars[0];
                bar.Visible = !((IModelListViewHideToolBar)View.Model).HideToolBar;
            }
        }
    }
}