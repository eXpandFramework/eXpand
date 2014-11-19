using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.ExpressApp.Web.SystemModule;
using Xpand.Persistent.Base.General.Model;

namespace Xpand.ExpressApp.Web.SystemModule.MasterDetail {
    
    [ModelAbstractClass]
    public interface IModelListViewAutoCommitWhenNested:IModelListView{
        [Category(AttributeCategoryNameProvider.Xpand)]
        bool AutoCommitWhenNested { get; set; }
    }

    public class NestedListViewAutoCommitController:ViewController<ListView>,IModelExtender{
        private SingleChoiceAction _newObjectAction;
        private SimpleAction _deleteAction;
        private SimpleAction _editAction;

        private bool Enabled(){
            return View.Editor is ASPxGridListEditor&& View.CollectionSource.Collection!=null&&Frame is NestedFrame && ((IModelListViewAutoCommitWhenNested) View.Model).AutoCommitWhenNested;
        }

        protected override void OnActivated(){
            base.OnActivated();
            if (Enabled()){
                _newObjectAction = Frame.GetController<NewObjectViewController>().NewObjectAction;
                _newObjectAction.ExecuteCompleted+=NewObjectActionOnExecuteCompleted;
                _deleteAction = Frame.GetController<DeleteObjectsViewController>().DeleteAction;
                _deleteAction.Execute+=DeleteActionOnExecute;
                _editAction = Frame.GetController<ListViewController>().EditAction;
                _editAction.ExecuteCompleted+=EditActionOnExecuteCompleted;
            }
        }

        protected override void OnDeactivated(){
            base.OnDeactivated();
            if (_newObjectAction != null){
                _newObjectAction.ExecuteCompleted-=NewObjectActionOnExecuteCompleted;
                _deleteAction.Execute-=DeleteActionOnExecute;
                _editAction.ExecuteCompleted-=EditActionOnExecuteCompleted;
            }
        }

        private void EditActionOnExecuteCompleted(object sender, ActionBaseEventArgs actionBaseEventArgs){
            actionBaseEventArgs.ShowViewParameters.CreatedView.ObjectSpace.Committing += (o, args) => Commit();
        }

        private void DeleteActionOnExecute(object sender, SimpleActionExecuteEventArgs simpleActionExecuteEventArgs){
            Commit();
        }

        private void NewObjectActionOnExecuteCompleted(object sender, ActionBaseEventArgs actionBaseEventArgs){
            actionBaseEventArgs.ShowViewParameters.CreatedView.ObjectSpace.Committed += (o, args) => Commit();
        }

        protected override void OnViewControlsCreated(){
            base.OnViewControlsCreated();
            if (Enabled()){
                var asPxGridListEditor = (View.Editor as ASPxGridListEditor);
                if (asPxGridListEditor != null){
                    var asPxGridView = asPxGridListEditor.Grid;
                    asPxGridView.RowInserting += (sender, args) => Commit();
                }
            }
        }

        private void Commit(){
            ((NestedFrame) Frame).ViewItem.View.ObjectSpace.CommitChanges();
        }

        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders){
            extenders.Add<IModelListView,IModelListViewAutoCommitWhenNested>();
        }
    }
}
