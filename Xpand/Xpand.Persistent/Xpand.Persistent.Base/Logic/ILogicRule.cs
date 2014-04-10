using System.ComponentModel;
using System.Drawing.Design;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;

namespace Xpand.Persistent.Base.Logic {
    [ModelAbstractClass]
    public interface ILogicRule :IRule {

        [Category("ConditionalLogic.Behavior")]
        [Description("Specifies the criteria string which is used when determining whether logic should be executed.")]
        [CriteriaOptions("TypeInfo")]
        [Editor("DevExpress.ExpressApp.Win.Core.ModelEditor.CriteriaModelEditorControl, DevExpress.ExpressApp.Win"+AssemblyInfo.VSuffix, typeof(UITypeEditor))]
        string NormalCriteria { get; set; }

        [Category("ConditionalLogic.Behavior")]
        [Description("Specifies the criteria string which is used when determining whether logic should be executed only used for listviews with no records.")]
        [CriteriaOptions("TypeInfo")]
        [Editor("DevExpress.ExpressApp.Win.Core.ModelEditor.CriteriaModelEditorControl, DevExpress.ExpressApp.Win" + AssemblyInfo.VSuffix, typeof(UITypeEditor))]
        string EmptyCriteria { get; set; }

        [Category("ConditionalLogic.Behavior")]
        bool? IsNew { get; set; }

        [Category("Logic.Behavior")]
        bool? IsRootView { get; set; }

        [Category("Logic.Behavior")]
        [Description("Specifies the View type in which the current rule is in effect.")]
        [RefreshProperties(RefreshProperties.All)]
        ViewType ViewType { get; set; }

        [Category("Logic.Behavior")]
        [Description("Specifies the View type in which the current rule is in effect.")]
        [DataSourceProperty("Views")]
        IModelView View { get; set; }

        [Category("Logic.Behavior")]
        [Description("Specifies the Nesting type in which the current rule is in effect.")]
        Nesting Nesting { get; set; }

        [Category("Logic.Behavior")]
        ViewEditMode? ViewEditMode { get; set; }

        [Browsable(false)]
        ITypeInfo TypeInfo { get; set; }

        
    }

}