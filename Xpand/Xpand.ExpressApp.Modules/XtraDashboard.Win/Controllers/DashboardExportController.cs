using System;
using System.IO;
using System.Windows.Forms;
using DevExpress.DashboardCommon;
using DevExpress.DashboardWin;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using Fasterflect;
using Xpand.ExpressApp.Dashboard.BusinessObjects;
using Xpand.ExpressApp.XtraDashboard.Win.PropertyEditors;
using ListView = DevExpress.ExpressApp.ListView;

namespace Xpand.ExpressApp.XtraDashboard.Win.Controllers {
    public class DashboardExportController : ObjectViewController<ListView,IDashboardDefinition> {
        private readonly SingleChoiceAction _exportDashboardAction;
        private const string ExportToPdf = "PDF";
        private const string ExportToImage = "Image";
        private const string ExportToExcel = "Excel";
        private const string PrintPreview = "PrintPreview";
        public DashboardExportController() {
            _exportDashboardAction = new SingleChoiceAction(this, "ExportDashboardTo", PredefinedCategory.Export) { Caption = "Export Dashboard To" };
            _exportDashboardAction.Items.Add(new ChoiceActionItem(PrintPreview, PrintPreview) { ImageName = "Action_Printing_Preview" });
            _exportDashboardAction.Items.Add(new ChoiceActionItem(ExportToPdf, ExportToPdf) { ImageName = "Action_Export_ToPDF" });
            _exportDashboardAction.Items.Add(new ChoiceActionItem(ExportToImage, ExportToImage) { ImageName = "Action_Export_ToImage" });
            _exportDashboardAction.Items.Add(new ChoiceActionItem(ExportToExcel, ExportToExcel) { ImageName = "Action_Export_ToXlsx" });
            
            _exportDashboardAction.SelectionDependencyType = SelectionDependencyType.RequireSingleObject;
            _exportDashboardAction.ItemType = SingleChoiceActionItemType.ItemIsOperation;
            _exportDashboardAction.EmptyItemsBehavior = EmptyItemsBehavior.Deactivate;
            _exportDashboardAction.Execute += ExportDashboardActionOnExecute;
        }

        public SingleChoiceAction ExportDashboardAction => _exportDashboardAction;

        private void ExportDashboardActionOnExecute(object sender, SingleChoiceActionExecuteEventArgs e) {
            var dashboardViewer = CreateDashboardViewer(null);
            dashboardViewer.DashboardChanged += (o, args) => {
                if ((string)e.SelectedChoiceActionItem.Data == PrintPreview) {
                    ShowRibbonPrintPreview(dashboardViewer);
                    return;
                }

                ShowExportForm(e, dashboardViewer);
            };
        }

        protected virtual void ShowExportForm(SingleChoiceActionExecuteEventArgs e, DashboardViewer dashboardViewer){
            dashboardViewer.CallMethod("ShowExportForm",
                (DashboardExportFormat) Enum.Parse(typeof(DashboardExportFormat), (string) e.SelectedChoiceActionItem.Data));
        }

        protected virtual void ShowRibbonPrintPreview(DashboardViewer dashboardViewer){
            dashboardViewer.PrintPreviewType = DashboardPrintPreviewType.RibbonPreview;
            dashboardViewer.ShowRibbonPrintPreview();
        }

        protected virtual DashboardViewer CreateDashboardViewer(Action<MemoryStream> exported) {
            var dashboardViewer = new DashboardViewer {
                Width = ((Form)Application.MainWindow.Template).Width,
                Height = ((Form)Application.MainWindow.Template).Height
            };
            ((Control) Frame.Template).BeginInvoke(new Action(() => dashboardViewer.Show(Application, (IDashboardDefinition) View.CurrentObject)));
            return dashboardViewer;
        }

    }
}