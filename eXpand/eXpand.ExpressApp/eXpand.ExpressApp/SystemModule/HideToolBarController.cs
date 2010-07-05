using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;

namespace eXpand.ExpressApp.SystemModule
{
    public interface IModelClassHideViewToolBar 
    {
        [Category("eXpand")]
        [Description("Hides view toolbar")]
        bool HideToolBar { get; set; }
    }
    [ModelInterfaceImplementor(typeof(IModelClassHideViewToolBar), "ModelClass")]
    public interface IModelViewHideViewToolBar : IModelClassHideViewToolBar
    {
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