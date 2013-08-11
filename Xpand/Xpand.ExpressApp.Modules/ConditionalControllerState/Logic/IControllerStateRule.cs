using System;
using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.Persistent.Base;
using Xpand.ExpressApp.ArtifactState.Logic;

namespace Xpand.ExpressApp.ConditionalControllerState.Logic {
    public interface IControllerStateRule : IArtifactStateRule {
        [Category("Data")]
        [Required]
        [DataSourceProperty("Controllers")]
        [TypeConverter(typeof(StringToTypeConverter))]
        Type ControllerType { get; set; }

        [Category("Behavior")]
        [ModelPersistentName("State")]
        ControllerState ControllerState { get; set; }
    }
}