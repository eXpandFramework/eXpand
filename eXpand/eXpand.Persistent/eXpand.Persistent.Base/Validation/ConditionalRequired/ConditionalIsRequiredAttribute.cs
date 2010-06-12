using System;
using DevExpress.Persistent.Validation;

namespace eXpand.Persistent.Base.Validation.ConditionalRequired {
    /// <summary>
    /// Conditionally RequiredField
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = true)]
    public sealed class RuleConditionalIsRequiredAttribute : RuleBaseAttribute {
        /// <summary>
        /// Initializes a new instance of the RuleConditionalIsRequiredAttribute class.
        /// </summary>
        public RuleConditionalIsRequiredAttribute() {
        }

        /// <summary>
        /// Initializes a new instance of the RuleConditionalIsRequiredAttribute class.
        /// </summary>
        /// <param name="contextType"></param>
        /// <param name="condition">The Condition to execute, must return bool</param>
        /// <param name="cstmErrorMessage">Custom Error Message to Display</param>
        /// <param name="id"></param>
        public RuleConditionalIsRequiredAttribute(string id, DefaultContexts contextType, string condition,
                                                  string cstmErrorMessage) : base(id, contextType) {
            Condition = condition;
            CustomErrorMessage = cstmErrorMessage;
        }

        /// <summary>
        /// Gets or sets the condition.
        /// </summary>
        /// <value>The condition.</value>
        public string Condition { get; set; }

        /// <summary>
        /// Gets or sets the custom error message.
        /// </summary>
        /// <value>The custom error message.</value>
        public string CustomErrorMessage {
            get { return Properties.CustomMessageTemplate; }

            set { Properties.CustomMessageTemplate = value; }
        }

        protected override Type RuleType {
            get { return typeof (RuleConditionalRequired); }
        }
    }
}