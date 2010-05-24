using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;

namespace eXpand.ExpressApp.SystemModule
{
    public interface IModelClassHideViewToolBar : IModelNode
    {
        [Category("eXpand")]
        [Description("Hides list view toolbar")]
        bool HideToolBar { get; set; }
    }

    public interface IModelListViewHideViewToolBar : IModelNode
    {
        [Category("eXpand")]
        [ModelValueCalculator("((IModelClassHideViewToolBar)ModelClass)", "HideToolBar")]
        [Description("Hides list view toolbar")]
        bool HideToolBar { get; set; }
    }

    public abstract class HideToolBarController : ViewController<ListView>, IModelExtender
    {
        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders)
        {
            extenders.Add<IModelClass, IModelClassHideViewToolBar>();
            extenders.Add<IModelListView, IModelListViewHideViewToolBar>();
        }
    }
}