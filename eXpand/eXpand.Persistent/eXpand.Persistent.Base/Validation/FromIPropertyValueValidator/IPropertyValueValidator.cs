using DevExpress.Persistent.Validation;

namespace eXpand.Persistent.Base.Validation.FromIPropertyValueValidator {
    public interface IPropertyValueValidator {
        bool IsPropertyValueValid(string propertyName, ref string errorMessageTemplate,
                                  ContextIdentifiers contextIdentifiers, string ruleId);
    }
}