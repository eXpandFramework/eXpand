using System.ComponentModel;
using DevExpress.ExpressApp.Model;

namespace Xpand.Persistent.Base.ModelAdapter {

    public interface IModelNodeEnabled : IModelNode {
        [DefaultValue(true)]
        [Category("Activation")]
        bool NodeEnabled { get; set; }

    }
}
