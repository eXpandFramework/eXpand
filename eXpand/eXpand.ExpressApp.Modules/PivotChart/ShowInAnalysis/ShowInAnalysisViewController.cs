using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using eXpand.ExpressApp.PivotChart.Core;

namespace eXpand.ExpressApp.PivotChart.ShowInAnalysis {
    public class ShowInAnalysisViewController : ViewController {
        readonly SingleChoiceAction showInAnalysisActionCore;

        public ShowInAnalysisViewController() {
            showInAnalysisActionCore = new SingleChoiceAction(this, "ShowInAnalysis", PredefinedCategory.RecordEdit) 
                                       {
                                           Caption = "Show in Analysis",
                                           ToolTip = "Show selected records in a analysis",
                                           ImageName = "BO_Analysis"
                                       };
            showInAnalysisActionCore.Execute += showInReportAction_Execute;
            showInAnalysisActionCore.ItemType = SingleChoiceActionItemType.ItemIsOperation;
            showInAnalysisActionCore.SelectionDependencyType = SelectionDependencyType.RequireMultipleObjects;

        }

        public SingleChoiceAction ShowInAnalysisAction {
            get { return showInAnalysisActionCore; }
        }

        void showInReportAction_Execute(object sender, SingleChoiceActionExecuteEventArgs e) {
            if (View.SelectedObjects.Count == 0) {
                return;
            }
            ShowInAnalysis(e);
        }

        protected void ShowInAnalysis(SingleChoiceActionExecuteEventArgs e) {
            ObjectSpace os = Application.CreateObjectSpace();
            var typeInfoContainer = (ITypeInfoContainer)Application.Modules.Where(@base => typeof(ITypeInfoContainer).IsAssignableFrom(@base.GetType())).Single();
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
            ObjectSpace os = Application.CreateObjectSpace();
            List<object> reportList = InplaceAnalysisCacheController.GetAnalysisDataList(Application,View.ObjectTypeInfo.Type);
            var typeInfoContainer = (ITypeInfoContainer)Application.Modules.Where(@base => typeof(ITypeInfoContainer).IsAssignableFrom(@base.GetType())).Single();
            List<ChoiceActionItem> items = (from id in reportList
                                            let report =
                                                os.GetObjectByKey(typeInfoContainer.TypesInfo.AnalysisType, id) as IAnalysisInfo
                                            where report != null
                                            select new ChoiceActionItem(report.ToString(), id)).ToList();
            items.Sort(SortByCaption);
            showInAnalysisActionCore.Items.Clear();
            showInAnalysisActionCore.Items.AddRange(items);
            UpdateActionActivity(showInAnalysisActionCore);
            base.OnActivated();
        }

        protected override void UpdateActionActivity(ActionBase action) {
            base.UpdateActionActivity(action);
            action.Active["VisibleInReports"] =((IModelClassReportsVisibility) View.Model.ModelClass).IsVisibleInReports;
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                showInAnalysisActionCore.Execute -= showInReportAction_Execute;
            }
            base.Dispose(disposing);
        }
    }
}