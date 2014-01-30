using System;
using System.ComponentModel;
using System.Drawing.Design;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.TreeListEditors.Win;

namespace Xpand.ExpressApp.TreeListEditors.Win.Model {
    [ModelAbstractClass]
    public interface IModelListViewTreeUseServerMode : IModelListView{
        [ModelValueCalculator("Application.Options", "UseServerMode")]
        [Category("Behavior")]
        [ModelBrowsable(typeof (TreeUseServerModeVisibilityCalculator))]
        new bool UseServerMode { get; set; }

        [Category("Data")]
        [CriteriaOptions("ModelClass.TypeInfo")]
        [Editor("DevExpress.ExpressApp.Win.Core.ModelEditor.CriteriaModelEditorControl, DevExpress.ExpressApp.Win" +
            XafAssemblyInfo.VersionSuffix + XafAssemblyInfo.AssemblyNamePostfix, typeof (UITypeEditor))]
        [ModelBrowsable(typeof(TreeUseServerModeVisibilityCalculator))]
        new string Criteria { get; set; }

        [DefaultValue("")]
        [Category("Data")]
        [CriteriaOptions("ModelClass.TypeInfo")]
        [Editor("DevExpress.ExpressApp.Win.Core.ModelEditor.CriteriaModelEditorControl, DevExpress.ExpressApp.Win" +
            XafAssemblyInfo.VersionSuffix + XafAssemblyInfo.AssemblyNamePostfix, typeof (UITypeEditor))]
        [ModelBrowsable(typeof(TreeUseServerModeVisibilityCalculator))]
        new string Filter { get; set; }
    }

    public class TreeUseServerModeVisibilityCalculator:IModelIsVisible{
        public bool IsVisible(IModelNode node, string propertyName){
            return !typeof (TreeListEditor).IsAssignableFrom(((IModelListView) node).EditorType);
        }
    }

    public class TreeListEditorVisibilityCalculatorHelper : TreeListEditors.Model.TreeListEditorVisibilityCalculatorHelper {
        public override Type[] TreelistEditorType(){
            return new []{typeof(TreeListEditor),typeof(CategorizedListEditor)};
        }
    }
}
