using System.ComponentModel;
using System.Drawing.Design;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using Xpand.Persistent.Base.General.Model;

namespace Xpand.Persistent.Base.Logic {
    [ModelAbstractClass]
    public interface ILogicRule :IRule {
        [Category(AttributeCategoryNameProvider.LogicBehavior)]
        [Description("Specifies the criteria string which is used when determining whether logic should be executed.")]
        [CriteriaOptions("TypeInfo")]
        [Editor("DevExpress.ExpressApp.Win.Core.ModelEditor.CriteriaModelEditorControl, DevExpress.ExpressApp.Win"+AssemblyInfo.VSuffix, typeof(UITypeEditor))]
        string NormalCriteria { get; set; }

        [Category(AttributeCategoryNameProvider.LogicBehavior)]
        [Description("Specifies the criteria string which is used when determining whether logic should be executed only used for listviews with no records.")]
        [CriteriaOptions("TypeInfo")]
        [Editor("DevExpress.ExpressApp.Win.Core.ModelEditor.CriteriaModelEditorControl, DevExpress.ExpressApp.Win" + AssemblyInfo.VSuffix, typeof(UITypeEditor))]
        string EmptyCriteria { get; set; }

        [Category(AttributeCategoryNameProvider.LogicBehavior)]
        bool? IsNew { get; set; }

        [Category(AttributeCategoryNameProvider.LogicBehavior)]
        bool? IsRootView { get; set; }

        [Category(AttributeCategoryNameProvider.LogicBehavior)]
        [Description("Specifies the View type in which the current rule is in effect.")]
        [RefreshProperties(RefreshProperties.All)]
        ViewType ViewType { get; set; }

        [Category(AttributeCategoryNameProvider.LogicBehavior)]
        [Description("Specifies the View type in which the current rule is in effect.")]
        [DataSourceProperty("Views")]
        IModelView View { get; set; }

        [Category(AttributeCategoryNameProvider.LogicBehavior)]
        [Description("Specifies the Nesting type in which the current rule is in effect.")]
        Nesting Nesting { get; set; }

        [Category(AttributeCategoryNameProvider.LogicBehavior)]
        ViewEditMode? ViewEditMode { get; set; }

        [Browsable(false)]
        ITypeInfo TypeInfo { get; set; }

        
    }

}