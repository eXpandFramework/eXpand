using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using Xpand.Persistent.Base.General.Model;
using Xpand.Persistent.Base.Xpo.MetaData;
using Xpand.Xpo;

namespace Xpand.Persistent.Base.General.Controllers.Actions{
    [NonPersistent]
    public class ChangeViewModel:XpandCustomObject {
        public ChangeViewModel(Session session) : base(session){
        }
    }

    [ModelAbstractClass]
    public interface IModelViewConfigureViewView:IModelView{
        [DataSourceProperty("Application.Views")]
        [Category(AttributeCategoryNameProvider.Xpand)]
        [DataSourceCriteria("(AsObjectView Is Not Null) And (AsObjectView.ModelClass Is Not Null)")]
        IModelDetailView ConfigureView { get; set; }
        
    }

    public class ChangeViewModelController:ModifyModelActionControllerBase,IModelExtender{
        private ChoiceActionItem _choiceActionItems;
        private const string ChangeViewModel = "Change View Model";

        protected override void OnActivated(){
            base.OnActivated();
            _choiceActionItems.Active.BeginUpdate();
            _choiceActionItems.Active["ModelNotConfigured"]= ((IModelViewConfigureViewView) View.Model).ConfigureView!=null;
            _choiceActionItems.Active.EndUpdate();
        }

        protected override void ModifyModelActionOnExecute(object sender, SingleChoiceActionExecuteEventArgs e){
            if (e.SelectedChoiceActionItem.Id == ChangeViewModel){
                var showViewParameters = e.ShowViewParameters;
                var objectSpace = Application.CreateObjectSpace();
                var modelDetailView = ((IModelViewConfigureViewView)View.Model).ConfigureView;
                var changeViewModel = objectSpace.CreateObject<ChangeViewModel>();
                showViewParameters.CreatedView = Application.CreateDetailView(objectSpace, modelDetailView,true,changeViewModel);
                showViewParameters.TargetWindow=TargetWindow.NewModalWindow;
                var dialogController = new DialogController();
                var viewToConfigure = View;
                dialogController.Accepting += (o, args) =>{
                    var modelMemberInfoController = dialogController.Frame.GetController<XpandModelMemberInfoController>();
                    modelMemberInfoController.SynchronizeModel(viewToConfigure.Model, dialogController.Frame.View.CurrentObject);
                };
                dialogController.Disposed += (o, args) => viewToConfigure.SetModel(viewToConfigure.Model);
                showViewParameters.Controllers.Add(dialogController);
            }
        }

        protected override IEnumerable<ChoiceActionItem> GetChoiceActionItems(){
            _choiceActionItems = new ChoiceActionItem(ChangeViewModel,null);
            yield return _choiceActionItems;
        }

        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders){
            extenders.Add<IModelView,IModelViewConfigureViewView>();
        }
    }
}