using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.Persistent.Base;
using eXpand.ExpressApp.Core.DictionaryHelpers;
using System.Linq;

namespace eXpand.ExpressApp.PivotChart.Win.Controllers {
    public partial class AnalysisControlVisibilityController : AnalysisViewControllerBase{
        public const string AnalysisControlVisibilityAttributeName = "AnalysisControlVisibility";

        public AnalysisControlVisibilityController() {
            InitializeComponent();
            RegisterActions(components);
            TargetObjectType = typeof (IAnalysisInfo);
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            var detailViewInfoNodeWrapper = new DetailViewInfoNodeWrapper(View.Info);
            var detailViewItemInfoNodeWrappers = detailViewInfoNodeWrapper.Editors.Items.Where(wrapper => typeof(IAnalysisInfo).IsAssignableFrom(wrapper.PropertyType));
            foreach (var detailViewItemInfoNodeWrapper in detailViewItemInfoNodeWrappers) {
                var analysisControlVisibility = detailViewItemInfoNodeWrapper.Node.GetAttributeEnumValue(AnalysisControlVisibilityAttributeName,AnalysisControlVisibility.Default);
                switch (analysisControlVisibility) {
                    case AnalysisControlVisibility.Pivot:
                        HideChart(detailViewItemInfoNodeWrapper.PropertyName);
                        break;
                    case AnalysisControlVisibility.Chart:
                        HidePivot(detailViewItemInfoNodeWrapper.PropertyName);
                        break;
                }
            }
        }

        void HidePivot(string propertyName) {
            AnalysisControlWin analysisControlWin = GetAnalysisControlWin(propertyName);
            analysisControlWin.ChartControl.Parent = analysisControlWin;
            analysisControlWin.TabControl.Visible = false;
        }

        void HideChart(string propertyName) {
            AnalysisControlWin analysisControlWin = GetAnalysisControlWin(propertyName);
            analysisControlWin.PivotGrid.Parent = analysisControlWin;
            analysisControlWin.TabControl.Visible = false;
        }


        AnalysisControlWin GetAnalysisControlWin(string propertyName) {
            try {
                return AnalysisEditors.Where(@base => @base.PropertyName==propertyName).OfType<AnalysisEditorWin>().Single().Control;
            }
            catch (InvalidOperationException) {
                throw new UserFriendlyException(
                    new Exception("Use " + typeof (AnalysisEditorWin).FullName + " as your default property editor for " +
                                  typeof (IAnalysisInfo).Name));
            }
        }

        public override Schema GetSchema() {
            DictionaryNode dictionaryNode =
                new SchemaHelper().InjectAttribute(AnalysisControlVisibilityAttributeName,typeof(AnalysisControlVisibility), ModelElement.DetailViewPropertyEditors);
            return new Schema(dictionaryNode);
        }
    }

    public enum AnalysisControlVisibility {
        Default,
        Pivot,
        Chart
    }
}