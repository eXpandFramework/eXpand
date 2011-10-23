using System;
using System.Collections.Generic;
using System.Drawing;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web;

namespace Xpand.ExpressApp.Validation.Web {
    public class WarningController : Validation.WarningController {
        protected override List<PropertyEditor> CollectPropertyEditors(IEnumerable<DevExpress.Persistent.Validation.RuleSetValidationResultItem> result) {
            var propertyEditors = base.CollectPropertyEditors(result);
            foreach (var propertyEditor in propertyEditors) {
                if (propertyEditor.Control is TableEx) {
                    EventHandler[] eventHandler = { null };
                    PropertyEditor result1 = propertyEditor;
                    eventHandler[0] = (sender, args) => {
                        var tableEx = ((TableEx)sender);
                        tableEx.PreRender -= eventHandler[0];
                        CreateWarning(result1, tableEx);
                    };
                    ((TableEx)propertyEditor.Control).PreRender += eventHandler[0];
                }
            }

            return propertyEditors;
        }


        void CreateWarning(PropertyEditor propertyEditor, TableEx tableEx) {
            var tableCell = tableEx.Rows[0].Cells[0];
            var image = new System.Web.UI.WebControls.Image();
            ImageInfo imageInfo = ImageLoader.Instance.GetImageInfo("Warning");
            image.AlternateText = "Warning";
            image.ImageUrl = imageInfo.ImageUrl;
            image.Width = imageInfo.Width;
            image.Height = imageInfo.Height;
            image.ToolTip = propertyEditor.ErrorMessage;
            image.Style["margin"] = "5px";
            tableCell.Controls.Clear();
            tableCell.Controls.Add(image);
        }

    }
}