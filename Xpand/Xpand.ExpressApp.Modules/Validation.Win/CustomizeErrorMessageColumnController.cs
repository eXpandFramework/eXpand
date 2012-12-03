using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Utils;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.GridView;

namespace Xpand.ExpressApp.Validation.Win {
    public class CustomizeErrorMessageColumnController : DevExpress.ExpressApp.Validation.Win.CustomizeErrorMessageColumnController {

        void SetupGridView(GridListEditorBase columnViewEditor) {
            var gridView = columnViewEditor.GridView as XpandGridView;
            if ((gridView != null) && (columnViewEditor.DataSource != null)) {
                gridView.OptionsView.ShowIndicator = false;
                var errorMessages = new ErrorMessages();
                foreach (object obj in ListHelper.GetList(columnViewEditor.DataSource)) {
                    errorMessages.AddMessage("ErrorMessage", obj, CaptionHelper.GetLocalizedText("Messages", "ValidationErrorMessage"));
                }
                gridView.ErrorMessages = errorMessages;
            }
        }

        void Editor_ControlsCreated(object sender, EventArgs e) {
            SetupGridView((GridListEditorBase)sender);
        }

        void Editor_DataSourceChanged(object sender, EventArgs e) {
            SetupGridView((GridListEditorBase)sender);
        }

        protected override void OnActivated() {
            var listEditor = ((ListView)View).Editor;
            if (!(listEditor is GridListEditorBase)) {
                base.OnActivated();
                return;
            }

            listEditor.ControlsCreated += Editor_ControlsCreated;
            listEditor.DataSourceChanged += Editor_DataSourceChanged;
        }

        protected override void OnDeactivated() {
            ((ListView)View).Editor.ControlsCreated -= Editor_ControlsCreated;
            ((ListView)View).Editor.DataSourceChanged -= Editor_DataSourceChanged;
            base.OnDeactivated();
        }
    }
}