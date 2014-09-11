using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.General.Model;

namespace Xpand.ExpressApp.SystemModule {
    public interface IModelClassHideViewToolBar {
        [Category(AttributeCategoryNameProvider.Xpand)]
        [Description("Hides view toolbar")]
        bool? HideToolBar { get; set; }
    }
    [ModelInterfaceImplementor(typeof(IModelClassHideViewToolBar), "ModelClass")]
    public interface IModelViewHideViewToolBar : IModelClassHideViewToolBar {
    }

    public class HideToolBarController : ViewController<ObjectView>, IModelExtender {
        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelClass, IModelClassHideViewToolBar>();
            extenders.Add<IModelObjectView, IModelViewHideViewToolBar>();
        }
    }
}