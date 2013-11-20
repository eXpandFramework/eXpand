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
    public class PivotGridInplaceEditorsControllerBase : ViewController<DetailView>,IModelExtender{

        public PivotGridInplaceEditorsControllerBase() {
            TargetObjectType = typeof (IAnalysisInfo);
        }

        protected override void OnActivated() {
            base.OnActivated();
            foreach (var analysisEditor in GetAnalysisEditors()) {
                analysisEditor.ControlCreated += AnalysisEditorOnControlCreated;
            }
        }

        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            foreach (var analysisEditor in GetAnalysisEditors()) {
                analysisEditor.ControlCreated += AnalysisEditorOnControlCreated;
            }
        }
        
        void AnalysisEditorOnControlCreated(object sender, EventArgs eventArgs) {
            var analysisEditorBase = (AnalysisEditorBase) sender;
            analysisEditorBase.ControlCreated-=AnalysisEditorOnControlCreated;
            CreateEditors(analysisEditorBase);
            OnAnalysisEditorCreated(analysisEditorBase);
        }

        protected virtual void OnAnalysisEditorCreated(AnalysisEditorBase analysisEditorBase) {
            
        }

        IEnumerable<AnalysisEditorBase> GetAnalysisEditors() {
            var modelPropertyEditors = View.Model.Items.OfType<IModelPropertyEditorPivotGridInPlaceEdit>().Where(
                        editor => editor.InPlaceEdit && typeof(IAnalysisInfo).IsAssignableFrom(
                            ((IModelPropertyEditor)editor).ModelMember.MemberInfo.MemberType));

            var analysisEditors = View.GetItems<AnalysisEditorBase>();
            IEnumerable<IMemberInfo> memberInfos = modelPropertyEditors.OfType<IModelPropertyEditor>().Select(modelPropertyEditor => View.ObjectTypeInfo.FindMember(modelPropertyEditor.PropertyName));
            return memberInfos.Select(memberInfo => analysisEditors.FirstOrDefault(@base => @base.MemberInfo == memberInfo)).Where(analysisEditorBase => analysisEditorBase != null);
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