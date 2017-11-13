using System;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.PivotChart;
using DevExpress.ExpressApp.PivotChart.Web;
using DevExpress.ExpressApp.Updating;
using DevExpress.Persistent.BaseImpl;
using MainDemo.Module.DatabaseUpdate;

namespace MainDemo.Module.Web.DatabaseUpdate {
	public class Updater : ModuleUpdater {
		public Updater(IObjectSpace  objectSpace, Version currentDBVersion) : base(objectSpace, currentDBVersion) { }
		public override void UpdateDatabaseAfterUpdateSchema() {
			base.UpdateDatabaseAfterUpdateSchema();
            new TaskAnalysis1LayoutUpdater(ObjectSpace).Update(ObjectSpace.FindObject<Analysis>(CriteriaOperator.Parse("Name='Completed tasks'")));
            new TaskAnalysis2LayoutUpdater(ObjectSpace).Update(ObjectSpace.FindObject<Analysis>(CriteriaOperator.Parse("Name='Estimated and actual work comparison'")));
            ObjectSpace.CommitChanges();
        }
	}
	public class TaskAnalysis1LayoutUpdater : TaskAnalysis1LayoutUpdaterBase {
		protected override IAnalysisControl CreateAnalysisControl() {
			return new AnalysisControlWeb();
		}
		protected override DevExpress.Persistent.Base.IPivotGridSettingsStore CreatePivotGridSettingsStore(IAnalysisControl control) {
			return new ASPxPivotGridSettingsStore(((AnalysisControlWeb)control).PivotGrid);
		}
        public TaskAnalysis1LayoutUpdater(IObjectSpace objectSpace)
            : base(objectSpace) {
        }
	}
	public class TaskAnalysis2LayoutUpdater : TaskAnalysis2LayoutUpdaterBase {
		protected override IAnalysisControl CreateAnalysisControl() {
			return new AnalysisControlWeb();
		}
		protected override DevExpress.Persistent.Base.IPivotGridSettingsStore CreatePivotGridSettingsStore(IAnalysisControl control) {
			return new ASPxPivotGridSettingsStore(((AnalysisControlWeb)control).PivotGrid);
		}
        public TaskAnalysis2LayoutUpdater(IObjectSpace objectSpace)
            : base(objectSpace) {
        }
	}
}
