using System;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Templates;
using DevExpress.XtraBars;

namespace eXpand.ExpressApp.Win.SystemModule
{
    public interface IModelListViewHideToolBar : IModelNode
    {
        [Category("eXpand")]
        bool HideToolBar { get; set; }
    }

    public class HideNestedListViewToolBarViewController : ViewController<ListView>, IModelExtender
    {
        public HideNestedListViewToolBarViewController()
        {
            TargetViewNesting = Nesting.Nested;
        }

        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders)
        {
            extenders.Add<IModelListView, IModelListViewHideToolBar>();
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            View.ControlsCreated += View_OnControlsCreated;
        }

        private void View_OnControlsCreated(object sender, EventArgs e)
        {
            if (Frame.Template is NestedFrameTemplate)
            {
                Bar bar = ((NestedFrameTemplate)Frame.Template).BarManager.Bars[0];
                bar.Visible = !((IModelListViewHideToolBar)View.Model).HideToolBar;
            }
        }
    }
}