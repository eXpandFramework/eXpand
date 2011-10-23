using System.Collections.Generic;
using System.Drawing;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Validation;
using DevExpress.ExpressApp.Validation.AllContextsView;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using Xpand.ExpressApp.Win.ListEditors;
using System.Linq;

namespace Xpand.ExpressApp.Validation.Win {

    public class WarningController : Validation.WarningController {
        static readonly Image _errorImage;

        static WarningController() {
            _errorImage = DevExpress.Utils.ResourceImageHelper.CreateImageFromResources("DevExpress.XtraEditors.Images.Warning.png", typeof(DXErrorProvider).Assembly);
        }

        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            if (ListEditor != null) {
                ListEditor.GridView.QueryErrorType += GridViewOnQueryErrorType;
            }
        }
        protected override List<PropertyEditor> CollectPropertyEditors(IEnumerable<DevExpress.Persistent.Validation.RuleSetValidationResultItem> result) {
            var propertyEditors = base.CollectPropertyEditors(result);
            foreach (var baseEdit in propertyEditors.Select(editor => editor.Control).OfType<BaseEdit>()) {
                baseEdit.ErrorIcon = _errorImage;
            }
            return propertyEditors;
        }

        void GridViewOnQueryErrorType(object sender, ErrorTypeEventArgs errorTypeEventArgs) {
            if (errorTypeEventArgs.Column != null && errorTypeEventArgs.ErrorType == ErrorType.Critical) {
                if (View.ObjectTypeInfo.Type != typeof(DisplayableValidationResultItem)) {
                    var xafGridColumn = ((XafGridColumn)errorTypeEventArgs.Column);
                    errorTypeEventArgs.ErrorType = GetErrorType(xafGridColumn.PropertyName);
                } else {
                    var resultItem = (DisplayableValidationResultItem)((XafGridView)sender).GetRow(errorTypeEventArgs.RowHandle);
                    var warning = ((IModelRuleBaseWarning)((IModelApplicationValidation)Application.Model).Validation.Rules[resultItem.Rule.Id]);
                    errorTypeEventArgs.ErrorType = warning.IsWarning ? ErrorType.Warning : ErrorType.Critical;
                }
            }
        }

        ErrorType GetErrorType(string propertyName) {
            if (!ErrorTypes.ContainsKey(propertyName))
                ErrorTypes.Add(propertyName, ErrorType.Critical);
            return ErrorTypes[propertyName];
        }

        public XpandGridListEditor ListEditor {
            get { return View is ListView ? ((ListView)View).Editor as XpandGridListEditor : null; }
        }



        //        protected override IEnumerable<string> GetUsedColumnNames(IRule rule) {
        //            return ListEditor.GridView != null ? rule.UsedProperties.Select(usedProperty => ((XafGridColumn)ListEditor.GridView.Columns.ColumnByFieldName(usedProperty)).PropertyName).Where(column => column != null).ToList() : new List<string>();
        //        }

    }
}