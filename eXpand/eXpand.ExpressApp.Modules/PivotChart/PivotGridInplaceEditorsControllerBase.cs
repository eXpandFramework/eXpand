using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.ExpressApp.PivotChart;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;

namespace eXpand.ExpressApp.PivotChart {
    public abstract class PivotGridInplaceEditorsControllerBase : ViewController<DetailView>{
        protected PivotGridInplaceEditorsControllerBase() {
            TargetObjectType = typeof (IAnalysisInfo);
        }
        protected override void OnFrameAssigned()
        {
            base.OnFrameAssigned();
            Frame.TemplateChanged += FrameOnTemplateChanged;
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            Frame.GetController<AnalysisReadOnlyController>().Active[GetType().Name] = false;
        }
        void FrameOnTemplateChanged(object sender, EventArgs eventArgs)
        {
            var supportViewControlAdding = (Frame.Template) as ISupportViewControlAdding;
            if (supportViewControlAdding != null)
                supportViewControlAdding.ViewControlAdding += (o, args) => CreateEditors();
        }

        void CreateEditors() {
            if (Frame.View is DetailView) {
                var detailViewInfoNodeWrapper = new DetailViewInfoNodeWrapper(View.Info);
                IEnumerable<DetailViewItemInfoNodeWrapper> detailViewItemInfoNodeWrappers =
                    detailViewInfoNodeWrapper.Editors.Items.Where(wrapper =>typeof (IAnalysisInfo).IsAssignableFrom(wrapper.PropertyType)&&wrapper.AllowEdit);
                var analysisEditors = View.GetItems<AnalysisEditorBase>();
                foreach (var viewItemInfoNodeWrapper in detailViewItemInfoNodeWrappers) {
                    var memberInfo = View.ObjectTypeInfo.FindMember(viewItemInfoNodeWrapper.PropertyName);
                    AnalysisEditorBase analysisControl = analysisEditors.Where(@base => @base.MemberInfo == memberInfo).FirstOrDefault();
                    if (analysisControl != null) CreateEditors(analysisControl);
                }
            }
        }


        protected abstract void CreateEditors(AnalysisEditorBase analysisEditorBase);
    }
}