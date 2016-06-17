using System;
using System.ComponentModel;
using System.IO;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;

namespace Xpand.Persistent.Base.Validation{
    public class RuleValidFileName : RulePropertyValue<object> {
        public const string PropertiesMessageTemplateMustNotBeEmpty = "MessageTemplateFileIsValid";
        private const string RuleValidFileNameDefaultMessageTemplate = "RuleValidFileName_defaultMessageTemplate";
        public static string DefaultMessageValidFileName
        {
            get{
                IValueManager<string> manager = ValueManager.GetValueManager<string>(RuleValidFileNameDefaultMessageTemplate);
                return manager.Value ??
                       (manager.Value = RuleDefaultMessageTemplates.ValidFileName);
            }
            set { ValueManager.GetValueManager<string>(RuleValidFileNameDefaultMessageTemplate).Value = value; }
        }
        protected override bool IsValueValid(object value, out string errorMessageTemplate) {
            errorMessageTemplate = Properties.MessageTemplateFileIsValid;
            var s = value as string;
            return s != null && s.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0;
        }
        public RuleValidFileName(string id, IMemberInfo property, ContextIdentifiers targetContextIDs) : base(id, property, targetContextIDs) { }
        public RuleValidFileName(string id, IMemberInfo property, ContextIdentifiers targetContextIDs, Type objectType) : base(id, property, targetContextIDs, objectType) { }
        public RuleValidFileName() { }
        public RuleValidFileName(IRuleValidFileNameProperties properties) : base(properties) { }
        public new IRuleValidFileNameProperties Properties => (IRuleValidFileNameProperties)base.Properties;

        public override Type PropertiesType => typeof(RuleValidFileNameProperties);
    }

    [GenerateMessageTemplatesModel("RuleRequiredField")]
    [DomainComponent]
    [Description("Defines templates for the messages displayed when RuleRequiredField validation rules are broken.")]
    public interface IRuleValidFileNameProperties : IRulePropertyValueProperties {
        [DefaultValue(false)]
        [Category("Behavior")]
        new bool SkipNullOrEmptyValues { get; set; }
        [Localizable(true)]
        [DefaultValue(DevExpress.Persistent.Validation.RuleDefaultMessageTemplates.MustNotBeEmpty)]
        [Category("Format")]
        string MessageTemplateFileIsValid { get; set; }
    }

    [DomainComponent]
    public class RuleValidFileNameProperties : RulePropertyValueProperties, IRuleValidFileNameProperties {
        public string MessageTemplateFileIsValid { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class RuleValidFileNameAttribute : RuleBaseAttribute, IRuleValidFileNameProperties {
        protected override bool CheckIfCollectionPropertyRuleAttributeCore() {
            return !string.IsNullOrEmpty(Properties.TargetPropertyName);
        }
        public RuleValidFileNameAttribute(string id, string targetContextIDs)
            : base(id, targetContextIDs) {
        }
        public RuleValidFileNameAttribute(DefaultContexts targetContexts)
            : base(null, targetContexts) {
        }
        public RuleValidFileNameAttribute(string id, DefaultContexts targetContexts)
            : base(id, targetContexts) {
        }
        public RuleValidFileNameAttribute(string id, string targetContextIDs, string messageTemplate)
            : base(id, targetContextIDs, messageTemplate) {
        }
        public RuleValidFileNameAttribute(string id, DefaultContexts targetContexts, string messageTemplate)
            : base(id, targetContexts, messageTemplate) {
        }
        public RuleValidFileNameAttribute()
            : base(null, DefaultContexts.Save) {
        }
        protected new IRuleValidFileNameProperties Properties => (IRuleValidFileNameProperties)base.Properties;

        protected override Type RuleType => typeof(RuleRequiredField);

        protected override Type PropertiesType => typeof(RuleValidFileNameProperties);

        public string TargetPropertyName{
            get { return Properties.TargetPropertyName; }
            set { Properties.TargetPropertyName = value; }
        }

        string IRuleValidFileNameProperties.MessageTemplateFileIsValid{
            get { return Properties.MessageTemplateFileIsValid; }
            set { Properties.MessageTemplateFileIsValid = value; }
        }
    }

}
