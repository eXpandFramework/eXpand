using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;

namespace Xpand.ExpressApp.Logic.Model {
    [KeyProperty("Name")]
    [DisplayProperty("Name")]
    public interface IModelExecutionContext : IModelNode {
        [DataSourceProperty("ExecutionContexts")]
        [Required]
        string Name { get; set; }
        [Browsable(false)]
        IEnumerable<string> ExecutionContexts { get; }
    }
}