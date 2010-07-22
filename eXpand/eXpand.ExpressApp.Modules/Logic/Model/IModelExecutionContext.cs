using System.ComponentModel;
using DevExpress.ExpressApp.Model;

namespace eXpand.ExpressApp.Logic.Model {
    [ModelAbstractClass]
    [KeyProperty("Name")]
    [DisplayProperty("Name")]
    public interface IModelExecutionContext : IModelNode {
        [Required]
        [ReadOnly(true)]
        string Name { get; set; }
    }

    

    public interface IModelViewChanging : IModelExecutionContext {
    }

    public interface IModelObjectChanged : IModelExecutionContext {
    }

    public interface IModelObjectSpaceReloaded : IModelExecutionContext {
    }

    public interface IModelCurrentObjectChanged : IModelExecutionContext {
    }

    public interface IModelViewControlsCreated : IModelExecutionContext {
    }

    public interface IModelControllerActivated : IModelExecutionContext {
    }

    public interface IModelViewControlAdding : IModelExecutionContext {
    }

    public interface IModelTemplateViewChanged : IModelExecutionContext {
    }
    public interface IModelObjectSpaceCommited : IModelExecutionContext {
    }
}