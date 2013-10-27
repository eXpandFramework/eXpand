using System.ComponentModel;
using DevExpress.ExpressApp.Model;

namespace Xpand.ExpressApp.Email.Model {
    public interface IModelApplicationEmail : IModelNode {
        [DefaultValue(true)]
        bool CreateControllersOnLogon { get; set; }

        IModelLogicEmail Email { get; }
    }
}