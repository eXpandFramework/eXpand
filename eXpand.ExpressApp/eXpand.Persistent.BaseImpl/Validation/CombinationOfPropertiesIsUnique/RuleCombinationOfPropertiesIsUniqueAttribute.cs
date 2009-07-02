using System;
using DevExpress.Persistent.Validation;

namespace eXpand.Persistent.BaseImpl.Validation.CombinationOfPropertiesIsUnique
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true,Inherited = true)]
    public class RuleCombinationOfPropertiesIsUniqueAttribute : RuleBaseAttribute
    {
        public RuleCombinationOfPropertiesIsUniqueAttribute(string id, string targetContextIDs, string targetProperties)
            : base(id, targetContextIDs)
        {
            Properties.TargetProperties = targetProperties;
        }

        public RuleCombinationOfPropertiesIsUniqueAttribute(string id, DefaultContexts targetContexts,
                                                            string targetProperties)
            : base(id, targetContexts)
        {
            Properties.TargetProperties = targetProperties;
        }

        protected override Type RuleType
        {
            get { return typeof (RuleCombinationOfPropertiesIsUnique); }
        }

        protected override Type PropertiesType
        {
            get { return typeof (RuleCombinationOfPropertiesIsUniqueProperties); }
        }

        public new RuleCombinationOfPropertiesIsUniqueProperties Properties
        {
            get { return (RuleCombinationOfPropertiesIsUniqueProperties) base.Properties; }
        }

        public string TargetProperties
        {
            get { return Properties.TargetProperties; }
        }

        public string Delimiters
        {
            get { return Properties.Delimiters; }
            set { Properties.Delimiters = value; }
        }
    }
}