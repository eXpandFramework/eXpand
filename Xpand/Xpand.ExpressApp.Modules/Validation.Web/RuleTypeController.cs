using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web;
using System.Linq;
using DevExpress.Persistent.Validation;

namespace Xpand.ExpressApp.Validation.Web {
    public class RuleTypeController : Validation.RuleTypeController {
        protected override Dictionary<PropertyEditor, RuleType> CollectPropertyEditors(IEnumerable<RuleSetValidationResultItem> result, RuleType ruleType) {
            var dictionary = base.CollectPropertyEditors(result, ruleType);
            foreach (var keyValuePair in dictionary.Where(pair => pair.Value!=RuleType.Critical)) {
                var ex = keyValuePair.Key.Control as TableEx;
                if (ex != null) {
                    EventHandler[] eventHandler = { null };
                    var pair = keyValuePair;
                    eventHandler[0] = (sender, args) => {
                        var tableEx = ((TableEx)sender);
                        tableEx.PreRender -= eventHandler[0];
                        CreateRuleImage(pair, tableEx);
                    };
                    ex.PreRender += eventHandler[0];
                }
            }

            return dictionary;
        }

        protected override void OnValidationFail(ValidationCompletedEventArgs validationCompletedEventArgs) {
            base.OnValidationFail(validationCompletedEventArgs);
            if (!validationCompletedEventArgs.Handled) {
                validationCompletedEventArgs.Handled = true;
                var ruleSetValidationResult = new RuleSetValidationResult();
                foreach (var result in validationCompletedEventArgs.Exception.Result.Results.Where(item => GetRuleType(item.Rule)==RuleType.Critical)) {
                    ruleSetValidationResult.AddResult(result);
                }
                throw new ValidationException(ruleSetValidationResult.GetFormattedErrorMessage(), ruleSetValidationResult);
            }
        }

        void CreateRuleImage(KeyValuePair<PropertyEditor, RuleType> keyValuePair, TableEx tableEx) {
            var tableCell = tableEx.Rows[0].Cells[0];
            var image = new System.Web.UI.WebControls.Image();
            var imageName = keyValuePair.Value.ToString();
            ImageInfo imageInfo = ImageLoader.Instance.GetImageInfo(imageName);
            image.AlternateText = imageName;
            image.ImageUrl = imageInfo.ImageUrl;
            image.Width = imageInfo.Width;
            image.Height = imageInfo.Height;
            image.ToolTip = keyValuePair.Key.ErrorMessage;
            image.Style["margin"] = "5px";
            tableCell.Controls.Clear();
            tableCell.Controls.Add(image);
        }

    }
}