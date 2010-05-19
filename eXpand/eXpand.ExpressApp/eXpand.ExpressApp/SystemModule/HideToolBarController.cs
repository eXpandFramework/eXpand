using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;

namespace eXpand.ExpressApp.SystemModule
{
    public interface IModelHideViewToolBar : IModelNode
    {
        [Category("eXpand")]
        [DefaultValue(false)]
        bool HideToolBar { get; set; }
    }

    public abstract class HideToolBarController : ViewController<ListView>, IModelExtender
    {
        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders)
        {
            extenders.Add<IModelListView, IModelHideViewToolBar>();
        }
    }
}