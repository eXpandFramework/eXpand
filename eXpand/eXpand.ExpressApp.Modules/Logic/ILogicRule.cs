using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;

namespace eXpand.ExpressApp.Logic {
    public interface ILogicRule : IRule {
        [Category("Behavior")]
        [Description("Specifies the View type in which the current rule is in effect.")]
        ViewType ViewType { get; set; }

        [Category("Behavior")]
        [Description("Specifies the View type in which the current rule is in effect.")]
        string ViewId { get; set; }

        [Category("Behavior")]
        [Description("Specifies the Nesting type in which the current rule is in effect.")]
        Nesting Nesting { get; set; }

        [Category("Data")]
        [Required]
        string ExecutionContextGroup { get; set; }

        
        [Browsable(false)]
        ITypeInfo TypeInfo { get; set; }
    }
}