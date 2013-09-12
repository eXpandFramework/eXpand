using System.ComponentModel;
using DevExpress.ExpressApp.Model;

namespace Xpand.ExpressApp.Model {
    public interface IModelClassPersistModelModifications : IModelNode {
        [Category("eXpand")]
        [DefaultValue(true)]
        bool PersistModelModifications { get; set; }
    }
    [ModelInterfaceImplementor(typeof(IModelClassPersistModelModifications), "ModelClass")]
    public interface IModelObjectViewPersistModelModifications : IModelClassPersistModelModifications {
    }

}