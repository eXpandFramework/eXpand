using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using Xpand.ExpressApp.PivotChart.Core;

namespace Xpand.ExpressApp.PivotChart.ShowInAnalysis {
    public class ShowInAnalysisViewController : ViewController<ObjectView> {
        readonly SingleChoiceAction _showInAnalysisActionCore;

        public ShowInAnalysisViewController() {
            _showInAnalysisActionCore = new SingleChoiceAction(this, "ShowInAnalysis", PredefinedCategory.RecordEdit) {
                Caption = "Show in Analysis",
                ToolTip = "Show selected records in a analysis",
                ImageName = "BO_Analysis"
            };
            _showInAnalysisActionCore.Execute += showInReportAction_Execute;
            _showInAnalysisActionCore.ItemType = SingleChoiceActionItemType.ItemIsOperation;
            _showInAnalysisActionCore.SelectionDependencyType = SelectionDependencyType.RequireMultipleObjects;

        }

        public SingleChoiceAction ShowInAnalysisAction {
            get { return _showInAnalysisActionCore; }
        }

        void showInReportAction_Execute(object sender, SingleChoiceActionExecuteEventArgs e) {
            if (View.SelectedObjects.Count == 0) {
                return;
            }
            ShowInAnalysis(e);
        }

        protected void ShowInAnalysis(SingleChoiceActionExecuteEventArgs e) {
            var typeInfoContainer = (ITypeInfoContainer)Application.Modules.Single(@base => @base is ITypeInfoContainer);
            var os = Application.CreateObjectSpace(typeInfoContainer.TypesInfo.AnalysisType);
            var report =
                os.GetObjectByKey(typeInfoContainer.TypesInfo.AnalysisType, e.SelectedChoiceActionItem.Data) as IAnalysisInfo;
            e.ShowViewParameters.CreatedView = Application.CreateDetailView(os, report);
            e.ShowViewParameters.TargetWindow = TargetWindow.Default;
            e.ShowViewParameters.Context = TemplateContext.View;
            e.ShowViewParameters.CreateAllControllers = true;

            var keys = new ArrayList();
            foreach (object selectedObject in View.SelectedObjects) {
                keys.Add(ObjectSpace.GetKeyValue(selectedObject));
            }
            e.ShowViewParameters.Controllers.Add(
                                                    new AssignCustomAnalysisDataSourceDetailViewController(
                                                        new InOperator(ObjectSpace.GetKeyPropertyName(View.ObjectTypeInfo.Type), keys)));
        }

        int SortByCaption(ChoiceActionItem left, ChoiceActionItem right) {
            return Comparer<string>.Default.Compare(left.Caption, right.Caption);
        }

        protected override void OnActivated() {
            List<object> reportList = InplaceAnalysisCacheController.GetAnalysisDataList(Application, View.ObjectTypeInfo.Type);
            var typeInfoContainer = (ITypeInfoContainer)Application.Modules.Single(@base => @base is ITypeInfoContainer);
            var os = Application.CreateObjectSpace(typeInfoContainer.TypesInfo.AnalysisType);
            List<ChoiceActionItem> items = (from id in reportList
                                            let report =
                                                os.GetObjectByKey(typeInfoContainer.TypesInfo.AnalysisType, id) as IAnalysisInfo
                                            where report != null
                                            select new ChoiceActionItem(report.ToString(), id)).ToList();
            items.Sort(SortByCaption);
            _showInAnalysisActionCore.Items.Clear();
            _showInAnalysisActionCore.Items.AddRange(items);
            UpdateActionActivity(_showInAnalysisActionCore);
            base.OnActivated();
        }

        protected override void UpdateActionActivity(ActionBase action) {
            base.UpdateActionActivity(action);
            action.Active["VisibleInReports"] = ((IModelClassReportsVisibility)View.Model.ModelClass).IsVisibleInReports;
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                _showInAnalysisActionCore.Execute -= showInReportAction_Execute;
            }
            base.Dispose(disposing);
        }
    }
}