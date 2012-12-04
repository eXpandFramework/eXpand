using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Validation;
using DevExpress.ExpressApp.Validation.AllContextsView;
using DevExpress.Persistent.Validation;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraGrid.Views.Grid;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.GridView;

namespace Xpand.ExpressApp.Validation.Win {

    public class RuleTypeController : Validation.RuleTypeController {

        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            if (ListEditor != null && ListEditor.GridView() is IQueryErrorType) {
                ((IQueryErrorType)((ListEditor).GridView())).QueryErrorType += GridViewOnQueryErrorType;
            }
        }

        protected override Dictionary<PropertyEditor, RuleType> CollectPropertyEditors(IEnumerable<RuleSetValidationResultItem> result, RuleType ruleType) {
            var propertyEditors = base.CollectPropertyEditors(result, ruleType);
            foreach (var keyValuePair in propertyEditors.Where(IsNotCritical)) {
                ((BaseEdit)keyValuePair.Key.Control).ErrorIcon = CreateImageFromResources(keyValuePair.Value);
            }
            return propertyEditors;
        }

        private bool IsNotCritical(KeyValuePair<PropertyEditor, RuleType> pair) {
            return pair.Value != RuleType.Critical && pair.Key.Control is BaseEdit;
        }

        Image CreateImageFromResources(RuleType ruleType) {
            return ImageLoader.Instance.GetEnumValueImageInfo(ruleType).Image;
        }

        void GridViewOnQueryErrorType(object sender, ErrorTypeEventArgs errorTypeEventArgs) {
            var enumDescriptor = new EnumDescriptor(typeof(ErrorType));
            var critical = (ErrorType)enumDescriptor.ParseCaption(RuleType.Critical.ToString());
            if (errorTypeEventArgs.Column != null && errorTypeEventArgs.ErrorType == critical) {
                if (View.ObjectTypeInfo.Type != typeof(DisplayableValidationResultItem)) {
                    var xafGridColumn = errorTypeEventArgs.Column;
                    var caption = GetRuleType(xafGridColumn.PropertyName()).ToString();
                    errorTypeEventArgs.ErrorType = (ErrorType)enumDescriptor.ParseCaption(caption);
                } else {
                    var resultItem = (DisplayableValidationResultItem)((GridView)sender).GetRow(errorTypeEventArgs.RowHandle);
                    if (resultItem.Rule != null) {
                        var warning = ((IModelRuleBaseRuleType)((IModelApplicationValidation)Application.Model).Validation.Rules[resultItem.Rule.Id]);
                        if (warning != null)
                            errorTypeEventArgs.ErrorType = (ErrorType)enumDescriptor.ParseCaption(warning.RuleType.ToString());
                    }
                }
            }
        }

        RuleType GetRuleType(string propertyName) {
            return (from pair in Dictionary let ruleSetValidationResultItem = pair.Value.FirstOrDefault(item => item.Rule.UsedProperties.Contains(propertyName)) where ruleSetValidationResultItem != null select pair.Key).FirstOrDefault();
        }

        public ColumnsListEditor ListEditor {
            get { return View is ListView ? ((ListView)View).Editor as ColumnsListEditor : null; }
        }
    }
}