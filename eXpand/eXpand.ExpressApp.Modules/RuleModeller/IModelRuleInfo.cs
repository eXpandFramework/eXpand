using DevExpress.ExpressApp;

namespace eXpand.ExpressApp.RuleModeller {
    public interface IModelRuleInfo<TModelRule> where TModelRule : ModelRule
    {
        /// <summary>
        /// Represents a string that describes the current rule.
        /// </summary>
        TModelRule Rule { get; }

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