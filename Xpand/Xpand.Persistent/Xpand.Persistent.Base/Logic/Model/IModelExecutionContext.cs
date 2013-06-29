using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;

namespace Xpand.Persistent.Base.Logic.Model {
    [KeyProperty("Name")]
    [DisplayProperty("Name")]
    public interface IModelExecutionContext : IModelNode {
        [DataSourceProperty("ExecutionContexts")]
        [Required]
        string Name { get; set; }
        [Browsable(false)]
        IEnumerable<string> ExecutionContexts { get; }
    }
    [KeyProperty("Name")]
    [DisplayProperty("Name")]
    public interface IModelActionExecutionContext : IModelNode {
        [DataSourceProperty("ExecutionContexts")]
        [Required]
        string Name { get; set; }
        [Browsable(false)]
        IEnumerable<string> ExecutionContexts { get; }
    }

}