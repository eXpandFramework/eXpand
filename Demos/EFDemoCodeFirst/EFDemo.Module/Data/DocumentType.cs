using System;
using System.Linq;
using System.Collections.Generic;

namespace EFDemo.Module.Data {
    public enum DocumentType {
        SourceCode = 1,
        Tests = 2,
        Documentation = 3,
        Diagrams = 4,
        ScreenShots = 5,
        Unknown = 6
    };
}
