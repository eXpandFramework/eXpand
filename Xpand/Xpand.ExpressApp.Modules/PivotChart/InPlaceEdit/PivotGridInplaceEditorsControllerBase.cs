using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.PivotChart;
using DevExpress.Persistent.Base;

namespace Xpand.ExpressApp.PivotChart.InPlaceEdit {
    public interface IModelMemberPivotGridInPlaceEdit {
        [Category("eXpand.PivotChart")]
        bool InPlaceEdit { get; set; }
    }
    [ModelInterfaceImplementor(typeof(IModelMemberPivotGridInPlaceEdit), "ModelMember")]
    public interface IModelPropertyEditorPivotGridInPlaceEdit:IModelMemberPivotGridInPlaceEdit {
        
    }
    public class PivotGridInplaceEditorsControllerBase : ViewController<XpandDetailView>,IModelExtender{
        public event EventHandler<EditorCreatedArgs> EditorCreated;

        protected virtual void OnEditorCreated(EditorCreatedArgs e) {
            EventHandler<EditorCreatedArgs> handler = EditorCreated;
            if (handler != null) handler(this, e);
        }

        public PivotGridInplaceEditorsControllerBase() {
            TargetObjectType = typeof (IAnalysisInfo);
        }

        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            foreach (var analysisEditor in GetAnalysisEditors()) {
                AnalysisEditorBase @base = analysisEditor;
                analysisEditor.ValueRead += (sender, args) =>{
                    CreateEditors(@base);
                    OnEditorCreated(new EditorCreatedArgs(@base));
                };
            }
        }

        IEnumerable<AnalysisEditorBase> GetAnalysisEditors() {
            IEnumerable<IModelPropertyEditorPivotGridInPlaceEdit> modelPropertyEditors = View.Model.Items.OfType<IModelPropertyEditorPivotGridInPlaceEdit>().Where(
                        editor => editor.InPlaceEdit && typeof(IAnalysisInfo).IsAssignableFrom(
                            ((IModelPropertyEditor)editor).ModelMember.MemberInfo.MemberType));

            var analysisEditors = View.GetItems<AnalysisEditorBase>();
            IEnumerable<IMemberInfo> memberInfos = modelPropertyEditors.OfType<IModelPropertyEditor>().Select(modelPropertyEditor => View.ObjectTypeInfo.FindMember(modelPropertyEditor.PropertyName));
            return memberInfos.Select(memberInfo => analysisEditors.Where(@base => @base.MemberInfo == memberInfo).FirstOrDefault()).Where(analysisEditorBase => analysisEditorBase != null);
        }





        protected virtual void CreateEditors(AnalysisEditorBase analysisEditorBase) {
            throw new NotImplementedException();
        }
        #region IModelExtender Members

        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders)
        {
            extenders.Add<IModelMember,IModelMemberPivotGridInPlaceEdit>();
            extenders.Add<IModelPropertyEditor, IModelPropertyEditorPivotGridInPlaceEdit>();
        }

        #endregion
    }
}