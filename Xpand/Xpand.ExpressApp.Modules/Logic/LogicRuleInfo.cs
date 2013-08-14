using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using Xpand.Persistent.Base.Logic;

namespace Xpand.ExpressApp.Logic {
    /// <summary>
    /// A helper class that is used to store the information about the artifact
    /// </summary>
    public class LogicRuleInfo : ILogicRuleInfo{

        /// <summary>
        /// Represents a string that describes the current rule.
        /// </summary>
        public ILogicRuleObject Rule { get; set; }

        /// <summary>
        /// Currently processed object in the View.
        /// </summary>
        public object Object { get; set; }


        /// <summary>
        /// Gets or sets whether the selected customization should be applied to the selected artifacts.
        /// </summary>
        public bool Active { get; set; }

        public View View { get; set; }

        public ExecutionContext ExecutionContext { get; set; }

        public ActionBaseEventArgs ActionBaseEventArgs { get; set; }

        public EventArgs EventArgs { get; set; }

        public bool InvertCustomization { get; set; }

        public override string ToString() {
            return ReferenceEquals(Rule,null) ? base.ToString() : Rule.ToString();
        }
    }
}