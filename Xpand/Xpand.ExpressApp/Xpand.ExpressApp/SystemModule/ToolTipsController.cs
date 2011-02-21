using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;

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

    public class ToolTipsController : ViewController, IModelExtender {
        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            if (View is ListView) SetListViewToolTips();
            if (View is DetailView) SetDetailViewToolTips();
        }
        protected virtual void SetListViewToolTips() { }
        protected virtual void SetDetailViewToolTips() { }
        protected string GetToolTip(IModelMember member) {
            return ((IModelMemberToolTip)member).ToolTip;
        }
        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelMember, IModelMemberToolTip>();
            extenders.Add<IModelPropertyEditor, IModelPropertyEditorTooltip>();
        }
    }
}
