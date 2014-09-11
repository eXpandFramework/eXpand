using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.General.Model;

namespace Xpand.ExpressApp.Model {
    public interface IModelClassProccessViewShortcuts {
        [Category(AttributeCategoryNameProvider.Xpand)]
        [DefaultValue(true)]
        bool ViewShortcutProccesor { get; set; }
    }
    [ModelInterfaceImplementor(typeof(IModelClassProccessViewShortcuts), "ModelClass")]
    public interface IModelDetailViewProccessViewShortcuts : IModelClassProccessViewShortcuts {
    }

}
