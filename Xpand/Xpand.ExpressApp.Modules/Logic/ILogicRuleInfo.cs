using DevExpress.ExpressApp;
using Xpand.Persistent.Base.Logic;

namespace Xpand.ExpressApp.Logic {
    public interface ILogicRuleInfo {
        /// <summary>
        /// Represents a string that describes the current rule.
        /// </summary>
        ILogicRuleObject Rule { get; }

        /// <summary>
        /// Currently processed object in the View.
        /// </summary>
        object Object { get; }

        /// <summary>
        /// Gets or sets whether the selected customization should be applied to the selected artifacts.
        /// </summary>
        bool Active { get; }

        bool InvertingCustomization { get; set; }
        View View { get; set; }
    }
}