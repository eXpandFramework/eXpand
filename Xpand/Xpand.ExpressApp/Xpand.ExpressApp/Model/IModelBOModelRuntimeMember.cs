using System.ComponentModel;
using DevExpress.ExpressApp.Model;

namespace Xpand.ExpressApp.Model {
    public interface IModelBOModelRuntimeMember : IModelNode
    {
        [Category("eXpand")]
        bool IsRuntimeMember { get; set; }
    }
}