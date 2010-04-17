using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;

namespace eXpand.Persistent.Base.General
{
    public interface ILogicRule
    {
        [Required]
        string ID { get; set; }
        // TODO: DefaultValue
        string ExecutionContextGroup { get; set; }
        ViewType ViewType { get; set; }
        Nesting Nesting { get; set; }
        string Description { get; set; }
        [Required, DataSourceProperty("Application.Model.BOModel")]
        ITypeInfo TypeInfo { get; set; }
        [DataSourceProperty("Application.Model.Views")]
        IModelView ViewId { get; set; }
        int Index { get; set; }
    }
}