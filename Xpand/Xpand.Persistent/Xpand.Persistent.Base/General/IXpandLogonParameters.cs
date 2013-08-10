
using DevExpress.ExpressApp.Utils;

namespace Xpand.Persistent.Base.General {
    public interface IXpandLogonParameters : ICustomObjectSerialize {
        bool RememberMe { get; set; }
        SettingsStorage Storage { get; }
    }
}
