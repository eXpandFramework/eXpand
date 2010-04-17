using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;

namespace eXpand.ExpressApp.SystemModule
{
    public interface IModelHideViewToolBar : IModelNode
    {
        [DefaultValue(false)]
        bool HideToolBar { get; set; }
    }

    public abstract class HideToolBarController : ViewController<ListView>
    {
        public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders)
        {
            base.ExtendModelInterfaces(extenders);
            extenders.Add<IModelListView, IModelHideViewToolBar>();
        }
    }
}