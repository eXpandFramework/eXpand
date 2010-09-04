using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;

namespace Xpand.ExpressApp.PivotChart.Web {
    public interface IModelMemberLoadingPanel:IModelMember {
        [Category("eXpand.PivotChart")]
        [Description("It controls the visibility of the ajax loading panel for analysisproperty editors")]
        bool LoadingPanel { get; set; }    
    }
    public interface IModelPropertyEditorLoadingPanel : IModelPropertyEditor{
        [Category("eXpand.PivotChart")]
        [Description("It controls the visibility of the ajax loading panel for analysisproperty editors")]
        [ModelValueCalculator("((IModelMemberLoadingPanel)ModelMember)", "LoadingPanel")]
        [DefaultValue(true)]
        bool LoadingPanel { get; set; }    
    }
    public class LoadingPanelController:ViewController,IModelExtender {
        

        #region IModelExtender Members

        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders)
        {
            extenders.Add<IModelMember,IModelMemberLoadingPanel>();
            extenders.Add<IModelPropertyEditor, IModelPropertyEditorLoadingPanel>();
        }

        #endregion
    }
}