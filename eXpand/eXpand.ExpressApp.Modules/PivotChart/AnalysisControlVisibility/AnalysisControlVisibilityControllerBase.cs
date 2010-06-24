using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.PivotChart;
using DevExpress.Persistent.Base;
using AnalysisViewControllerBase = eXpand.ExpressApp.PivotChart.Core.AnalysisViewControllerBase;

namespace eXpand.ExpressApp.PivotChart.AnalysisControlVisibility {
    public interface IModelMemberAnalysisControlVisibility:IModelNode {
        [Category("eXpand.PivotChart")]
        [Description("Controls the visibility of Analysis control")]
        AnalysisControlVisibility AnalysisControlVisibility { get; set; }    
    }
    public interface IModelPropertyEditorAnalysisControlVisibility:IModelPropertyEditor {
        [Category("eXpand.PivotChart")]
        [Description("Controls the visibility of Analysis control")]
        [ModelValueCalculator("((IModelMemberAnalysisControlVisibility)ModelMember)", "AnalysisControlVisibility")]
        AnalysisControlVisibility AnalysisControlVisibility { get; set; }    
    }

    public abstract class AnalysisControlVisibilityControllerBase<TAnalysisEditor,TAnalysisControl> : AnalysisViewControllerBase,IModelExtender
        where TAnalysisEditor : AnalysisEditorBase where TAnalysisControl: class, IAnalysisControl{
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


        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            IEnumerable<IModelPropertyEditorAnalysisControlVisibility> modelPropertyEditorAnalysisControlVisibilitys =
                View.Model.Items.OfType<IModelPropertyEditorAnalysisControlVisibility>().Where(
                    item => item.AnalysisControlVisibility != AnalysisControlVisibility.Default);
            foreach (var controlVisibility in modelPropertyEditorAnalysisControlVisibilitys) {
                var analysisControl = GetAnalysisControl(controlVisibility.PropertyName) as TAnalysisControl;
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

        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders)
        {
            extenders.Add<IModelMember,IModelMemberAnalysisControlVisibility>();
            extenders.Add<IModelPropertyEditor,IModelPropertyEditorAnalysisControlVisibility>();
        }

        #endregion
        }
}