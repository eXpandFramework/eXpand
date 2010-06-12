using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.PivotChart;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;

namespace eXpand.ExpressApp.PivotChart.InPlaceEdit {
    public abstract class PivotGridInplaceEditorsControllerBase : ViewController<DetailView>{
        public event EventHandler<EditorCreatedArgs> EditorCreated;

        protected virtual void OnEditorCreated(EditorCreatedArgs e) {
            EventHandler<EditorCreatedArgs> handler = EditorCreated;
            if (handler != null) handler(this, e);
        }

        protected PivotGridInplaceEditorsControllerBase() {
            TargetObjectType = typeof (IAnalysisInfo);
        }
        protected override void OnFrameAssigned()
        {
            base.OnFrameAssigned();
            Frame.TemplateChanged+=FrameOnTemplateChanged;
        }

        void FrameOnTemplateChanged(object sender, EventArgs eventArgs) {
            var supportViewControlAdding = (Frame.Template) as ISupportViewControlAdding;
            if (supportViewControlAdding != null) {
                supportViewControlAdding.ViewControlAdding -= SupportViewControlAddingOnViewControlAdding;
                supportViewControlAdding.ViewControlAdding += SupportViewControlAddingOnViewControlAdding;
            }
        }

        protected override void OnDeactivating() {
            base.OnDeactivating();
            var supportViewControlAdding = (Frame.Template) as ISupportViewControlAdding;
            if (supportViewControlAdding != null) {
                supportViewControlAdding.ViewControlAdding -= SupportViewControlAddingOnViewControlAdding;
            }
        }
        void SupportViewControlAddingOnViewControlAdding(object sender, EventArgs eventArgs) {
            CreateEditors();
        }

        void CreateEditors() {
            if (Frame.View is DetailView) {
                IEnumerable<IModelPropertyEditor> modelPropertyEditors =View.Items.OfType<IModelPropertyEditor>().Where(
                        editor =>editor.AllowEdit &&typeof (IAnalysisInfo).IsAssignableFrom(editor.ModelMember.MemberInfo.MemberType));
                var analysisEditors = View.GetItems<AnalysisEditorBase>();
                IEnumerable<IMemberInfo> memberInfos = modelPropertyEditors.Select(modelPropertyEditor => View.ObjectTypeInfo.FindMember(modelPropertyEditor.PropertyName));
                IEnumerable<AnalysisEditorBase> analysisEditorBases = memberInfos.Select(memberInfo => analysisEditors.Where(@base => @base.MemberInfo == memberInfo).FirstOrDefault());
                foreach (AnalysisEditorBase analysisEditorBase in analysisEditorBases.Where(analysisEditorBase => analysisEditorBase != null)) {
                    CreateEditors(analysisEditorBase);
                    OnEditorCreated(new EditorCreatedArgs(analysisEditorBase));
                }
            }
        }




        protected abstract void CreateEditors(AnalysisEditorBase analysisEditorBase);
    }
}