using DevExpress.ExpressApp;

namespace eXpand.ExpressApp.Logic {
    /// <summary>
    /// A helper class that is used to store the information about the artifact
    /// </summary>
    public class LogicRuleInfo<TLogicRule> : ILogicRuleInfo<TLogicRule> where TLogicRule:ILogicRule{

        /// <summary>
        /// Represents a string that describes the current rule.
        /// </summary>
        public TLogicRule Rule { get; set; }

        /// <summary>
        /// Currently processed object in the View.
        /// </summary>
        public object Object { get; set; }


        /// <summary>
        /// Gets or sets whether the selected customization should be applied to the selected artifacts.
        /// </summary>
        public bool Active { get; set; }

        public bool InvertingCustomization { get; set; }

        public View View { get; set; }

        public ExecutionContext ExecutionContext { get; set; }
    }
}