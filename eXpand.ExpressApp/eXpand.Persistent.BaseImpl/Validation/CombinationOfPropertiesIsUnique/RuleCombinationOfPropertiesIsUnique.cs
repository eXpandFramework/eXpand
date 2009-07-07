using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;

namespace eXpand.Persistent.BaseImpl.Validation.CombinationOfPropertiesIsUnique
{
    [RulePropertiesDefaultValue("Delimiters", ";,.:")]
    [RulePropertiesDefaultValue("IncludeCurrentObject", false)]
    public class RuleCombinationOfPropertiesIsUnique : RuleSearchObject
    {
        private readonly List<string> uniqueProperties = new List<string>();
        private string criteria = string.Empty;

//        public RuleCombinationOfPropertiesIsUnique(string id, ContextIdentifiers targetContextIDs, Type objectType) : base(id, targetContextIDs, objectType)
//        {
//        }

        //Specify the properties that will be highlighted when the rule is broken
        public RuleCombinationOfPropertiesIsUnique()
        {

        }

        public RuleCombinationOfPropertiesIsUnique(RuleSearchObjectProperties properties) : base(properties)
        {
        }

        public override ReadOnlyCollection<string> UsedProperties
        {
            get
            {
                return
                    new ReadOnlyCollection<string>(Properties.TargetProperties.Split(Properties.Delimiters.ToCharArray()));
            }
        }

        public new RuleCombinationOfPropertiesIsUniqueProperties Properties
        {
            get { return (RuleCombinationOfPropertiesIsUniqueProperties) base.Properties; }
        }

        public override Type PropertiesType
        {
            get { return typeof (RuleCombinationOfPropertiesIsUniqueProperties); }
        }

        private void BuildCriteriaString()
        {
            criteria = string.Empty;
            uniqueProperties.Clear();
            uniqueProperties.AddRange(Properties.TargetProperties.Split(Properties.Delimiters.ToCharArray()));
            const string criteriaTemplate = "[{0}] == '@{0}' ";
            foreach (string currentProperty in uniqueProperties)
            {
                if (!string.IsNullOrEmpty(criteria))
                {
                    criteria += " AND ";
                }
                criteria += string.Format(criteriaTemplate, currentProperty);
            }
        }

        //Check whether the passed target properties are unique
        protected override bool IsValidInternal(object target, out string errorMessageTemplate)
        {
            if (string.IsNullOrEmpty(criteria))
            {
                BuildCriteriaString();
            }
            string lastSearchResult;
            bool result = IsSearchedObjectsExist((IXPSimpleObject) target, Properties.TargetType, out lastSearchResult,
                                                 criteria);
            errorMessageTemplate = string.Format("Combination of the following properties must be unique: {0}",
                                                 string.Join(", ", uniqueProperties.ToArray()));
            return !result;
        }
    }
}