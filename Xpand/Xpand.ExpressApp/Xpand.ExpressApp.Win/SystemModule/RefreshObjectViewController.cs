using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.ExpressApp.Win.SystemModule;
using Xpand.ExpressApp.SystemModule;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.Model;

namespace Xpand.ExpressApp.Win.SystemModule{
	public interface IModelListViewAutoRefreshWhenNotFocus{
		[Category(AttributeCategoryNameProvider.Xpand)]
        bool AutoRefreshWhenNotFocus{ get; set; }
	    [Category(AttributeCategoryNameProvider.Xpand)]
	    [DefaultValue(true)]
        bool AutoRefreshWhenDocumentManagerPageIsActive{ get; set; }
	}
	public class RefreshObjectViewController : ViewController,IModelExtender{
	    protected override void OnActivated(){
	        base.OnActivated();
	        if (Application.MainWindow != null)
	            Application.MainWindow.GetController<ActiveDocumentViewController>().ActiveViewChanged +=OnActiveViewChanged;
	    }

	    protected override void OnDeactivated(){
	        base.OnDeactivated();
	        Application.MainWindow?.GetController<ActiveDocumentViewController>(controller =>
	            controller.ActiveViewChanged -=OnActiveViewChanged);
	    }

	    private void OnActiveViewChanged(object sender, ActiveViewChangedEventArgs e){
	        _activeDocumentView = e.View;
	    }

	    private static View _activeDocumentView;

	    protected  void Refresh(){
			
			var listView = View as ListView;
			var gridListEditor = ((GridListEditor)listView?.Editor);
			if (gridListEditor?.Grid != null && ((gridListEditor.Grid.IsFocused && ((IModelListViewAutoRefreshWhenNotFocus)View.Model).AutoRefreshWhenNotFocus)))
				return;
	        if (((IModelOptionsWin) Application.Model.Options).UIType == UIType.TabbedMDI &&
	            ((IModelListViewAutoRefreshWhenNotFocus) View.Model).AutoRefreshWhenDocumentManagerPageIsActive&&_activeDocumentView!=null){
	            if (View is ListView && _activeDocumentView is DetailView &&!((IModelDetailView) _activeDocumentView.Model).GetModelPropertyEditors()
	                .Any(editor =>editor.ModelMember.MemberInfo.IsList && editor.View.Id == View.Id) )
		            return;
                if (View is ListView && _activeDocumentView is ListView&&View!=_activeDocumentView)
                    return;
                if (View is DetailView&& _activeDocumentView is ListView)
                    return;
                if (View is DetailView&&_activeDocumentView is DetailView&&View!=_activeDocumentView)
                    return;
                    
	            
		    }
			View.RefreshDataSource();

		}

        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelListView, IModelListViewAutoRefreshWhenNotFocus>();
        }
    }
}