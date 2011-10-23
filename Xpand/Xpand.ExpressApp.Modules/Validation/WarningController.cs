using System;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Validation;
using DevExpress.Persistent.Validation;
using DevExpress.XtraEditors.DXErrorProvider;
using System.Linq;

namespace Xpand.ExpressApp.Validation {
    public interface IModelRuleBaseWarning : IModelNode {
        [Category("eXpand")]
        bool IsWarning { get; set; }
    }

    public abstract class WarningController : ViewController<ObjectView> {
        protected Dictionary<string, ErrorType> ErrorTypes = new Dictionary<string, ErrorType>();
        protected List<PropertyEditor> PropertyEditors = new List<PropertyEditor>();

        protected override void OnActivated() {
            base.OnActivated();
            Validator.RuleSet.ValidationCompleted += RuleSetOnValidationCompleted;
        }

        protected override void OnDeactivated() {
            base.OnDeactivated();
            Validator.RuleSet.ValidationCompleted -= RuleSetOnValidationCompleted;
        }
        void RuleSetOnValidationCompleted(object sender, ValidationCompletedEventArgs validationCompletedEventArgs) {
            if (View == null || View.IsDisposed)
                return;
            if (!validationCompletedEventArgs.Successful) {
                var warningResults = GetWarningResults(validationCompletedEventArgs);
                if (View is DetailView)
                    PropertyEditors=CollectPropertyEditors(warningResults);
                else {
                    ErrorTypes=CollectErrorTypes(warningResults);
                }
                validationCompletedEventArgs.Handled = !InvalidNonWarningRulesExist(validationCompletedEventArgs, warningResults);
            }
        }

        protected virtual Dictionary<string, ErrorType> CollectErrorTypes(List<RuleSetValidationResultItem> warningResults) {
            var errorTypes=new Dictionary<string, ErrorType>();
            foreach (var resultItem in warningResults) {
                IEnumerable<string> usedColumns = resultItem.Rule.UsedProperties;
                foreach (var column in usedColumns) {
                    errorTypes[column] = GetErrorType(warningResults, column);
                }
            }
            return errorTypes;
        }

        List<RuleSetValidationResultItem> GetWarningResults(ValidationCompletedEventArgs validationCompletedEventArgs) {
            var warningResults = validationCompletedEventArgs.Exception.Result.Results.Where(IsWarning).ToList();
            var noWarningResults = validationCompletedEventArgs.Exception.Result.Results.Where(item => !warningResults.Contains(item)).ToList();
            return warningResults.Where(OnlyWarningRules(noWarningResults)).ToList();
        }

        Func<RuleSetValidationResultItem, bool> OnlyWarningRules(IEnumerable<RuleSetValidationResultItem> noWarningResults) {
            return item => noWarningResults.Where(resultItem => IsWarning(resultItem, item)).FirstOrDefault() == null;
        }
        private bool IsWarning(RuleSetValidationResultItem propertyValueProperties, RuleSetValidationResultItem item) {
            return propertyValueProperties != null && propertyValueProperties.Rule.UsedProperties.Any(s => item.Rule.UsedProperties.Contains(s));
        }

        bool InvalidNonWarningRulesExist(ValidationCompletedEventArgs validationCompletedEventArgs, List<RuleSetValidationResultItem> result) {
            return validationCompletedEventArgs.Exception.Result.Results.Where(item => item.State == ValidationState.Invalid && !result.Contains(item)).FirstOrDefault() != null;
        }

        protected virtual List<PropertyEditor> CollectPropertyEditors(IEnumerable<RuleSetValidationResultItem> result) {
            return result.SelectMany(GetPropertyEditors).ToList();
        }

        //        protected abstract void SetWarningIcon(IEnumerable<object> controls);

        IEnumerable<PropertyEditor> GetPropertyEditors(RuleSetValidationResultItem resultItem) {
            return View.GetItems<PropertyEditor>().Where(
                editor => resultItem.Rule.UsedProperties.Contains(editor.MemberInfo.Name)).Where(editor => editor.Control != null);
        }

        //        protected abstract IEnumerable<string> GetUsedColumnNames(IRule rule);

        ErrorType GetErrorType(IEnumerable<RuleSetValidationResultItem> result, string column) {
            return result.FirstOrDefault(item => item.Rule.UsedProperties.Contains(column)) != null ? ErrorType.Warning : ErrorType.Critical;
        }

        bool IsWarning(RuleSetValidationResultItem resultItem) {
            IModelValidationRules modelValidationRules = ((IModelApplicationValidation)Application.Model).Validation.Rules;
            var modelRuleBaseWarning = ((IModelRuleBaseWarning)modelValidationRules.OfType<ModelNode>().SingleOrDefault(node => node.Id == resultItem.Rule.Id));
            return modelRuleBaseWarning != null && modelRuleBaseWarning.IsWarning;
        }

    }
}