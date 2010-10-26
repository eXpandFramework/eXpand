using System;
using DevExpress.Persistent.Validation;

namespace Xpand.Persistent.Base.Validation.AtLeast1PropertyIsRequired {
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class RuleRequiredForAtLeast1PropertyAttribute : RuleBaseAttribute,
                                                            IRuleRequiredForAtLeast1PropertyProperties {
        public RuleRequiredForAtLeast1PropertyAttribute(string id, string targetContextIDs, string targetProperties)
            : base(id, targetContextIDs) {
            Properties.TargetProperties = targetProperties;
            SkipNullOrEmptyValues = false;
        }

        public RuleRequiredForAtLeast1PropertyAttribute(string id, DefaultContexts targetContexts,
                                                        string targetProperties)
            : base(id, targetContexts) {
            Properties.TargetProperties = targetProperties;
            SkipNullOrEmptyValues = false;
        }

        protected override Type RuleType {
            get { return typeof (RuleRequiredForAtLeast1Property); }
        }

        protected override Type PropertiesType {
            get { return typeof (RuleRequiredForAtLeast1PropertyProperties); }
        }

        public new RuleRequiredForAtLeast1PropertyProperties Properties {
            get { return (RuleRequiredForAtLeast1PropertyProperties) base.Properties; }
        }
        #region IRuleRequiredForAtLeast1PropertyProperties Members
        public string MessageTemplateMustNotBeEmpty {
            get { return Properties.MessageTemplateMustNotBeEmpty; }
            set { Properties.MessageTemplateMustNotBeEmpty = value; }
        }

        public string TargetProperties {
            get { return Properties.TargetProperties; }
            set { Properties.TargetProperties = value; }
        }

        public string Delimiters {
            get { return Properties.Delimiters; }
            set { Properties.Delimiters = value; }
        }
        #endregion
                                                            }
}