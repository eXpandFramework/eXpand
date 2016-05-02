using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Scheduler.Web;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.SystemModule;
using DevExpress.ExpressApp.Web.Templates;
using DevExpress.Persistent.Base.General;
using Xpand.Persistent.Base.General.Model;
using Xpand.Persistent.Base.General.Model.VisibilityCalculators;

namespace Xpand.ExpressApp.Scheduler.Web.Controllers{
    public interface IModelListViewEventCreation{
        [Category(AttributeCategoryNameProvider.Xpand)]
        [ModelBrowsable(typeof(EditorTypeVisibilityCalculator<ASPxSchedulerListEditor,IModelListView>))]
        bool CreateEventOnDoubleClick { get; set; }
    }

    public class ASPxSchedulerEventCreationController : ObjectViewController<ListView, IEvent>, IXafCallbackHandler,IModelExtender{
        protected override void OnViewControlsCreated(){
            base.OnViewControlsCreated();
            if (((IModelListViewEventCreation) View.Model).CreateEventOnDoubleClick){
                ASPxSchedulerListEditor editor = View.Editor as ASPxSchedulerListEditor;
                if (editor != null){
                    XafCallbackManager.RegisterHandler("ASPxSchedulerEventCreationController", this);
                    editor.SchedulerControl.ClientSideEvents.Init = @"function(s,e) {
s.GetMainElement().ondblclick = function() { " + XafCallbackManager.GetScript("ASPxSchedulerEventCreationController", "") +
                                                                    @" }
}";
                }
            }
        }

        public void ProcessAction(string parameter){
            if (View.CurrentObject == null){
                Frame.GetController<NewObjectViewController>()
                    .NewObjectAction.DoExecute(new ChoiceActionItem("", View.ObjectTypeInfo.Type));
            }
            else{
                Frame.GetController<ListViewController>().EditAction.DoExecute();
            }
        }

        protected XafCallbackManager XafCallbackManager{
            get { return ((ICallbackManagerHolder) WebWindow.CurrentRequestPage).CallbackManager; }
        }

        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders){
            extenders.Add<IModelListView,IModelListViewEventCreation>();
        }
    }
}