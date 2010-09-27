using System.ComponentModel;
using DevExpress.ExpressApp.Model;

namespace Xpand.ExpressApp.Model {
    public interface IModelBOModelRuntimeMember : IModelNode
    {
        [Category("eXpand")]
        bool IsRuntimeMember { get; set; }
        [Category("eXpand")]
        [Description("Using an expression here it will force the creation of a calculated property insted of a normal one")]
        string AliasExpression { get; set; }
    }
}