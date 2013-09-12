using System.ComponentModel;
using DevExpress.ExpressApp.Model;

namespace Xpand.ExpressApp.Model {
    public interface IModelClassProccessViewShortcuts {
        [Category("eXpand")]
        [DefaultValue(true)]
        bool ViewShortcutProccesor { get; set; }
    }
    [ModelInterfaceImplementor(typeof(IModelClassProccessViewShortcuts), "ModelClass")]
    public interface IModelDetailViewProccessViewShortcuts : IModelClassProccessViewShortcuts {
    }

}
