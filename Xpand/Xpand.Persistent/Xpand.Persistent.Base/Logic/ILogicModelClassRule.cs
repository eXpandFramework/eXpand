using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;

namespace Xpand.Persistent.Base.Logic {
    public interface ILogicModelClassRule : ILogicRule {
        [ModelPersistentName("TypeInfo")]
        [DataSourceProperty("Application.BOModel"), Required]
        [Description("Required. Specifies the business class whose properties are affected by the current rule."), Category("Data")]
        [RefreshProperties(RefreshProperties.All)]
        IModelClass ModelClass { get; set; }
    }
}