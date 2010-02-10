using DevExpress.ExpressApp;

namespace eXpand.ExpressApp.RuleModeller {
    /// <summary>
    /// A helper class that is used to store the information about the artifact
    /// </summary>
    public abstract class ModelRuleInfo<TModelRule> : IModelRuleInfo<TModelRule> where TModelRule:ModelRule{

        /// <summary>
        /// Represents a string that describes the current rule.
        /// </summary>
        public TModelRule Rule { get; internal set; }

        /// <summary>
        /// Currently processed object in the View.
        /// </summary>
        public object Object { get; internal set; }


        /// <summary>
        /// Gets or sets whether the selected customization should be applied to the selected artifacts.
        /// </summary>
        public bool Active { get; internal set; }

        public bool InvertingCustomization { get; set; }

        public View View { get; set; }
    }
}