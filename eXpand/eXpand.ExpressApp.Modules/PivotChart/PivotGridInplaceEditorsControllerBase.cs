using System.Linq;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.ExpressApp.PivotChart;
using DevExpress.Persistent.Base;

namespace eXpand.ExpressApp.PivotChart {
    public abstract class PivotGridInplaceEditorsControllerBase : AnalysisViewControllerBase {
        protected PivotGridInplaceEditorsControllerBase() {
            TargetObjectType = typeof (IAnalysisInfo);
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            Frame.GetController<AnalysisReadOnlyController>().Active[GetType().Name] = false;
            var detailViewInfoNodeWrapper = new DetailViewInfoNodeWrapper(View.Info);
            DetailViewItemInfoNodeWrapper detailViewItemInfoNodeWrapper = detailViewInfoNodeWrapper.Editors.Items.Where(wrapper => typeof(IAnalysisInfo).IsAssignableFrom(wrapper.PropertyType)).FirstOrDefault();
            if (detailViewItemInfoNodeWrapper != null ){
                Frame.GetController<AnalysisDataBindController>().BindDataAction.Execute += (sender, args) => {
                    if (detailViewItemInfoNodeWrapper.AllowEdit) {
                        var memberInfo = View.ObjectTypeInfo.FindMember(detailViewItemInfoNodeWrapper.PropertyName);
                        IAnalysisControl analysisControl = AnalysisEditors.Where(@base => @base.MemberInfo == memberInfo).Select(@base => @base.Control).FirstOrDefault();
                        if (analysisControl != null) CreateEditors(analysisControl);
                    }
                };
            }
        }

        protected abstract void CreateEditors(IAnalysisControl analysisControl);
    }
}