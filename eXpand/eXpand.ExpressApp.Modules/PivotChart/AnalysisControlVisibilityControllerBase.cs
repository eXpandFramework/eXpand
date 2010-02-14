using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.ExpressApp.PivotChart;
using DevExpress.Persistent.Base;
using eXpand.ExpressApp.Core.DictionaryHelpers;

namespace eXpand.ExpressApp.PivotChart {
    public abstract class AnalysisControlVisibilityControllerBase<TAnalysisEditor,TAnalysisControl> : AnalysisViewControllerBase
        where TAnalysisEditor : AnalysisEditorBase where TAnalysisControl:IAnalysisControl{
        public const string AnalysisControlVisibilityAttributeName = "AnalysisControlVisibility";

        protected AnalysisControlVisibilityControllerBase() {
            TargetObjectType = typeof (IAnalysisInfo);
        }

        IAnalysisControl GetAnalysisControl(string propertyName)
        {
            try {
                return AnalysisEditors.Where(@base => @base.PropertyName == propertyName).OfType<TAnalysisEditor>().Single().Control;
            }
            catch (InvalidOperationException) {
                throw new UserFriendlyException(
                    new Exception("Use " + typeof (TAnalysisEditor).FullName + " as your default property editor for " +
                                  typeof (IAnalysisInfo).Name));
            }
        }

        public override Schema GetSchema() {
            DictionaryNode dictionaryNode =
                new SchemaHelper().InjectAttribute(AnalysisControlVisibilityAttributeName,
                                                   typeof (AnalysisControlVisibility),ModelElement.DetailViewPropertyEditors);
            return new Schema(dictionaryNode);
        }

        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            var detailViewInfoNodeWrapper = new DetailViewInfoNodeWrapper(View.Info);
            IEnumerable<DetailViewItemInfoNodeWrapper> detailViewItemInfoNodeWrappers =
                detailViewInfoNodeWrapper.Editors.Items.Where(wrapper =>typeof (IAnalysisInfo).IsAssignableFrom(wrapper.PropertyType));
            foreach (DetailViewItemInfoNodeWrapper detailViewItemInfoNodeWrapper in detailViewItemInfoNodeWrappers) {
                AnalysisControlVisibility analysisControlVisibility =
                    detailViewItemInfoNodeWrapper.Node.GetAttributeEnumValue(AnalysisControlVisibilityAttributeName,AnalysisControlVisibility.Default);
                var analysisControl = (TAnalysisControl) GetAnalysisControl(detailViewItemInfoNodeWrapper.PropertyName);
                switch (analysisControlVisibility) {
                    case AnalysisControlVisibility.Pivot:
                        HideChart(analysisControl);
                        break;
                    case AnalysisControlVisibility.Chart:
                        HidePivot(analysisControl);
                        break;
                }
            }
        }

        protected abstract void HidePivot(TAnalysisControl analysisControl);

        protected abstract void HideChart(TAnalysisControl analysisControl);
        }
}