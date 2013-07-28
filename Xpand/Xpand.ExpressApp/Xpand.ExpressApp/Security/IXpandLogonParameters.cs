
using DevExpress.ExpressApp.Utils;

namespace Xpand.ExpressApp.Security {
    public interface IXpandLogonParameters : ICustomObjectSerialize {
        bool RememberMe { get; set; }
        SettingsStorage Storage { get; }
    }
}
