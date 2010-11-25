using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;

namespace Xpand.ExpressApp.Logic.Model {
    [KeyProperty("Name")]
    [DisplayProperty("Name")]
    public interface IModelFrameTemplateContext : IModelNode {
        [DataSourceProperty("FrameTemplateContexts")]
        [Required]
        string Name { get; set; }
        [Browsable(false)]
        IEnumerable<string> FrameTemplateContexts { get; }
    }
}