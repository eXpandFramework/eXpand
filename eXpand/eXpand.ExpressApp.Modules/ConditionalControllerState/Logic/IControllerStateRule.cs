using System;
using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using eXpand.ExpressApp.ArtifactState.Logic;

namespace eXpand.ExpressApp.ConditionalControllerState.Logic {
    public interface IControllerStateRule : IArtifactStateRule {
        [Category("Data")]
        [Required]
        [DataSourceProperty("Controllers")]
        Type ControllerType { get; set; }

        [Category("Behavior")]
        ControllerState State { get; set; }

        
    }
}