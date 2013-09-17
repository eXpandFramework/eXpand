using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Validation;
using DevExpress.ExpressApp.Validation.AllContextsView;
using DevExpress.Persistent.Validation;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using Xpand.ExpressApp.Validation.RuleType;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView;

namespace Xpand.ExpressApp.Validation.Win.RuleType {
    public class RuleTypeController : Validation.RuleType.RuleTypeController {
        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            if (ListEditor != null) {
                var gridView = ListEditor.GridView();
                if (gridView != null) {
                    gridView.CustomDrawCell += GridViewOnCustomDrawCell;
                }
            }
        }

        void GridViewOnCustomDrawCell(object sender, RowCellCustomDrawEventArgs e) {
            BaseEditViewInfo info = ((GridCellInfo)e.Cell).ViewInfo;
            var enumDescriptor = new EnumDescriptor(typeof(ErrorType));
            var row = ((GridView)sender).GetRow(e.RowHandle);
            var resultItem = row as DisplayableValidationResultItem;
            Image errorIcon = null;
            if (resultItem != null) {
                errorIcon = ErrorIcon(resultItem, enumDescriptor);
            } else if (Columns.Any()) {
                var keyValuePairs = Columns.SelectMany(types => types);
                var propertyName = e.Column.PropertyName();
                Func<KeyValuePair<ColumnWrapper, Validation.RuleType.RuleType>, bool> predicate = pair => propertyName == pair.Key.PropertyName;
                if (keyValuePairs.Any(predicate)) {
                    var caption = keyValuePairs.Last(predicate).Value.ToString();
                    errorIcon = DXErrorProvider.GetErrorIconInternal((ErrorType)enumDescriptor.ParseCaption(caption));
                }
            }
            if (errorIcon != null) {
                info.ErrorIcon = errorIcon;
                info.CalcViewInfo(e.Graphics);
            }
        }

        Image ErrorIcon(DisplayableValidationResultItem resultItem, EnumDescriptor enumDescriptor) {
            if (resultItem.Rule != null) {
                var ruleType =
                    ((IModelRuleBaseRuleType)
                     ((IModelApplicationValidation)Application.Model).Validation.Rules[resultItem.Rule.Id]);
                if (ruleType != null) {
                    var errorType = (ErrorType)enumDescriptor.ParseCaption(ruleType.RuleType.ToString());
                    return DXErrorProvider.GetErrorIconInternal(errorType);
                }
            }
            return null;
        }

        protected override Dictionary<PropertyEditor, Validation.RuleType.RuleType> CollectPropertyEditors(IEnumerable<RuleSetValidationResultItem> result, Validation.RuleType.RuleType ruleType) {
            var propertyEditors = base.CollectPropertyEditors(result, ruleType);
            foreach (var keyValuePair in propertyEditors) {
                var baseEdit = keyValuePair.Key.Control as BaseEdit;
                if (baseEdit != null)
                    baseEdit.ErrorIcon = CreateImageFromResources(keyValuePair.Value);
            }
            return propertyEditors;
        }

        Image CreateImageFromResources(Validation.RuleType.RuleType ruleType) {
            return ImageLoader.Instance.GetEnumValueImageInfo(ruleType).Image;
        }

    }
}