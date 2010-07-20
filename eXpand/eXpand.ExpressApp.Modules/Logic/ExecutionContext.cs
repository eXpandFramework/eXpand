using System;

namespace eXpand.ExpressApp.Logic {
    [Flags]
    public enum ExecutionContext
    {
        None = 0,
        ViewChanging = 2,
        ObjectChanged = 4,
        ObjectSpaceReloaded = 8,
        CurrentObjectChanged = 16,
        ViewControlsCreated = 32,
        ControllerActivated = 64,
        ViewControlAdding = 128,
        TemplateViewChanged = 256,
        ObjectSpaceCommited=512
    }
}