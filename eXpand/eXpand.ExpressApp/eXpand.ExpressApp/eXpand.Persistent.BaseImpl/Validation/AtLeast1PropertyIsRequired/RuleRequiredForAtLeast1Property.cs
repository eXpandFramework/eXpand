using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;

namespace eXpand.Persistent.BaseImpl.Validation.RuleRequiredForAtLeast1Property
{
    [RulePropertiesDefaultValue("Delimiters", ";,.:")]
    [RulePropertiesDefaultValue("SkipNullOrEmptyValues", false)]
    public class RuleRequiredForAtLeast1Property : RuleBase
    {
        private static SimpleValueManager<string> defaultMessageTemplateMustNotBeEmpty;
        private readonly List<string> properties = new List<string>();

        public RuleRequiredForAtLeast1Property()
        {
        }

        public RuleRequiredForAtLeast1Property(RuleSearchObjectProperties properties) : base(properties)
        {
        }


        public RuleRequiredForAtLeast1Property(string id, ContextIdentifiers targetContextIDs, Type objectType)
            : base(id, targetContextIDs, objectType)
        {
        }

        public static string DefaultMessageTemplateMustNotBeEmpty
        {
            get {
                if (defaultMessageTemplateMustNotBeEmpty == null)
                    defaultMessageTemplateMustNotBeEmpty = new SimpleValueManager<string>();
                return defaultMessageTemplateMustNotBeEmpty.Value ??
                       (defaultMessageTemplateMustNotBeEmpty.Value =
                        @"""At least one of {TargetProperties}"" must not be empty.");
            }
            set { defaultMessageTemplateMustNotBeEmpty.Value = value; }
        }

        public override ReadOnlyCollection<string> UsedProperties
        {
            get
            {
                return
                    new ReadOnlyCollection<string>(Properties.TargetProperties.Split(Properties.Delimiters.ToCharArray()));
            }
        }

        public new RuleRequiredForAtLeast1PropertyProperties Properties
        {
            get { return (RuleRequiredForAtLeast1PropertyProperties) base.Properties; }
        }

        public override Type PropertiesType
        {
            get { return typeof (RuleRequiredForAtLeast1PropertyProperties); }
        }

        protected override bool IsValidInternal(object target, out string errorMessageTemplate)
        {
            Dictionary<string, object> values = GetValues(target);
            int emptyFound = values.Count(value => Validator.RuleSet.IsEmptyValue(TargetObject, value.Key, value.Value));
            errorMessageTemplate = Properties.MessageTemplateMustNotBeEmpty;
            return (emptyFound != values.Count);
        }

        private Dictionary<string, object> GetValues(object target)
        {
            properties.Clear();
            properties.AddRange(Properties.TargetProperties.Split(Properties.Delimiters.ToCharArray()));
            ITypeInfo targetTypeInfo = XafTypesInfo.Instance.FindTypeInfo(Properties.TargetType);
            return properties.ToDictionary(property => property, property => targetTypeInfo.FindMember(property).GetValue(target));
        }
    }
}