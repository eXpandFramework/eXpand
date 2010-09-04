using DevExpress.Persistent.Validation;

namespace Xpand.Persistent.Base.Validation.FromIPropertyValueValidator {
    public interface IPropertyValueValidator {
        bool IsPropertyValueValid(string propertyName, ref string errorMessageTemplate,
                                  ContextIdentifiers contextIdentifiers, string ruleId);
    }
}