using System;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.PivotChart;
using DevExpress.Persistent.Base;
using AnalysisViewControllerBase = Xpand.ExpressApp.PivotChart.Core.AnalysisViewControllerBase;

namespace Xpand.ExpressApp.PivotChart.AnalysisControlVisibility {
    public interface IModelMemberAnalysisControlVisibility : IModelNode {
        [Category("eXpand.PivotChart")]
        [Description("Controls the visibility of Analysis control")]
        AnalysisControlVisibility AnalysisControlVisibility { get; set; }
    }
    [ModelInterfaceImplementor(typeof(IModelMemberAnalysisControlVisibility), "ModelMember")]
    public interface IModelPropertyEditorAnalysisControlVisibility : IModelMemberAnalysisControlVisibility {
    }

    public abstract class AnalysisControlVisibilityControllerBase<TAnalysisEditor, TAnalysisControl> : AnalysisViewControllerBase
        where TAnalysisEditor : AnalysisEditorBase
        where TAnalysisControl : class, IAnalysisControl {
        public const string AnalysisControlVisibilityAttributeName = "AnalysisControlVisibility";

        protected AnalysisControlVisibilityControllerBase() {
            TargetObjectType = typeof(IAnalysisInfo);
        }

        IAnalysisControl GetAnalysisControl(IModelPropertyEditor modelPropertyEditor) {
            try {
                return AnalysisEditors.Where(@base => @base.PropertyName == modelPropertyEditor.PropertyName).OfType<TAnalysisEditor>().Single().Control;
            } catch (InvalidOperationException) {
                throw new UserFriendlyException(
                    new Exception(String.Format("Use {0} as your default property editor for {1}", typeof(TAnalysisEditor).FullName, typeof(IAnalysisInfo).Name)));
            }
        }


        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            var modelPropertyEditorAnalysisControlVisibilitys =
                View.Model.Items.OfType<IModelPropertyEditorAnalysisControlVisibility>().Where(
                    item => item.AnalysisControlVisibility != AnalysisControlVisibility.Default);
            foreach (var controlVisibility in modelPropertyEditorAnalysisControlVisibilitys) {
                var analysisControl = GetAnalysisControl((IModelPropertyEditor)controlVisibility) as TAnalysisControl;
                if (analysisControl == null) continue;
                switch (controlVisibility.AnalysisControlVisibility) {
                    case AnalysisControlVisibility.Pivot:
                        HideChart(analysisControl);
                        break;
                    default:
                        HidePivot(analysisControl);
                        break;
                }
            }
        }

        protected abstract void HidePivot(TAnalysisControl analysisControl);

        protected abstract void HideChart(TAnalysisControl analysisControl);

        #region IModelExtender Members


        #endregion
    }
}