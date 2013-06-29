using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;

namespace Xpand.Persistent.Base.Logic.Model {
    [KeyProperty("Name")]
    [DisplayProperty("Name")]
    public interface IModelViewContext : IModelNode {
        [DataSourceProperty("ViewContexts")]
        [Required]
        string Name { get; set; }
        [Browsable(false)]
        IEnumerable<string> ViewContexts { get; }
    }
}