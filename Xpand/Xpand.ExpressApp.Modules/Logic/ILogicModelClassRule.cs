using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;

namespace Xpand.ExpressApp.Logic {
    public interface ILogicModelClassRule : ILogicRule {
        [ModelPersistentName("TypeInfo")]
        [DataSourceProperty("Application.BOModel"), Required]
        [Description("Required. Specifies the business class whose properties are affected by the current rule."), Category("Data")]
        [RefreshPropertiesAttribute(RefreshProperties.All)]
        IModelClass ModelClass { get; set; }
    }
}