using System;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.SystemModule {
    public interface IModelMemberToolTip : IModelNode {
        [Localizable(true)]
        [Description("Specifies the tooltip for the current property.")]
        [Category("eXpand")]
        string ToolTip { get; set; }
    }
    [ModelInterfaceImplementor(typeof(IModelMemberToolTip), "ModelMember")]
    public interface IModelPropertyEditorTooltip : IModelMemberToolTip {
    }
    public class TooltipCalculator {
        readonly object _currentObject;

        public TooltipCalculator(object currentObject) {
            _currentObject = currentObject;
        }

        public string GetToolTip(IModelMemberViewItem member) {
            return ((IModelMemberToolTip)member).ToolTip;
        }

        public string GetToolTip(IModelMemberViewItem member, object editValue) {
            return GetToolTip(member) + ConcatOtherToolTips(member,editValue);
        }

        public string ConcatOtherToolTips(IModelMemberViewItem model, object editValue) {
            return model.ModelMember.Type.IsEnum && _currentObject != null ? GetToolTipCore(model,editValue) : null;
        }

        string GetToolTipCore(IModelMemberViewItem model, object editValue) {
            string name = Enum.GetName(model.ModelMember.Type, editValue);
            if (!(string.IsNullOrEmpty(name))) {
                var tooltipAttribute = XafTypesInfo.Instance.FindTypeInfo(model.ModelMember.Type).FindMember(name).FindAttribute<TooltipAttribute>();
                return tooltipAttribute != null ? Environment.NewLine + tooltipAttribute.Value : null;
            }
            return null;
        }
    }

    public class ToolTipsController : ViewController, IModelExtender {
        protected TooltipCalculator TooltipCalculator;
        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            TooltipCalculator=new TooltipCalculator(View.CurrentObject);
            if (View is ListView) SetListViewToolTips();
            if (View is DetailView) SetDetailViewToolTips();
        }
        protected virtual void SetListViewToolTips() { }
        protected virtual void SetDetailViewToolTips() { }
        
        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelMember, IModelMemberToolTip>();
            extenders.Add<IModelPropertyEditor, IModelPropertyEditorTooltip>();
            extenders.Add<IModelColumn, IModelPropertyEditorTooltip>();
        }
    }
}
