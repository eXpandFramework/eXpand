using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;

namespace eXpand.ExpressApp.SystemModule
{
    public interface IModelClassHideViewToolBar : IModelNode
    {
        [Category("eXpand")]
        [Description("Hides view toolbar")]
        bool HideToolBar { get; set; }
    }

    public interface IModelViewHideViewToolBar : IModelNode
    {
        [Category("eXpand")]
        [ModelValueCalculator("((IModelClassHideViewToolBar)ModelClass)", "HideToolBar")]
        [Description("Hides view toolbar")]
        bool HideToolBar { get; set; }
    }

    public class HideToolBarController : ViewController, IModelExtender
    {
        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders)
        {
            extenders.Add<IModelClass, IModelClassHideViewToolBar>();
            extenders.Add<IModelView, IModelViewHideViewToolBar>();
        }
    }
}