using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.ExpressApp.Model;

namespace Xpand.ExpressApp.Security.Registration {
    [ModelAbstractClass]
    public interface IModelOptionsRegistration : IModelOptions {
        IModelRegistration Registration { get; }
    }

    public interface IModelRegistration : IModelNode {
        bool Enabled { get; set; }
    }
}
