using System;
using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.Persistent.Base;
using eXpand.ExpressApp.ArtifactState.Logic;

namespace eXpand.ExpressApp.ConditionalControllerState.Logic {
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