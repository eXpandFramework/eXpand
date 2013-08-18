using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;

namespace Xpand.Persistent.Base.Logic.Model {
    [ModelAbstractClass]
    public interface IModelLogicRule : IModelNode, IContextLogicRule {

        [ModelPersistentName("TypeInfo")]
        [DataSourceProperty("Application.BOModel"), Required]
        [Description("Required. Specifies the business class whose properties are affected by the current rule."), Category("Logic.Data")]
        [RefreshProperties(RefreshProperties.All)]
        IModelClass ModelClass { get; set; }

        [Browsable(false)]
        IEnumerable<IModelView> Views { get; }
        [Browsable(false)]
        IEnumerable<string> ActionExecutionContexts { get; }
        [Browsable(false)]
        IEnumerable<string> ExecutionContexts { get; }
        [Browsable(false)]
        IEnumerable<string> FrameTemplateContexts { get; }
        [Browsable(false)]
        IEnumerable<string> ViewContexts { get; }
        [Browsable(false)]
        IModelLogicWrapper ModelLogicWrapper { get; set; }
    }

}