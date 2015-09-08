using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.Data;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.Base;
using Xpand.Persistent.Base.General.Model;

namespace Xpand.ExpressApp.SystemModule {
    [ModelAbstractClass]
    public interface IModelListViewPositionColumn:IModelListView{
        [DataSourceProperty("SortableColumns")]
        [Localizable(false)]
        [Category(AttributeCategoryNameProvider.Xpand)]
        IModelColumn PositionColumn { get; set; }

        IEnumerable<IModelColumn> SortableColumns { get; }
    }

    [DomainLogic((typeof(IModelListViewPositionColumn)))]
    public class ListViewPositionColumnDomainLogic{
        public static void Set_PositionColumn(IModelListViewPositionColumn listView, IModelColumn modelColumn){
            listView.SetValue("PositionColumn", modelColumn);
            modelColumn.SortIndex = 0;
            modelColumn.SortOrder=ColumnSortOrder.Ascending;
            modelColumn.Index = -1;
        }

        public static IEnumerable<IModelColumn> Get_SortableColumns(IModelListViewPositionColumn column) {
            var modelColumns = column.Columns.Where(modelColumn => modelColumn.ModelMember != null && modelColumn.ModelMember.MemberInfo.MemberType == typeof(int));
            return new CalculatedModelNodeList<IModelColumn>(modelColumns);
        }
    }

    public class PositionInListViewController:ViewController<ListView>,IModelExtender{
        private IModelColumn _positionColumn;
        private IMemberInfo _memberInfo;
        private readonly SimpleAction _positionUpAction;
        private readonly SimpleAction _positionDownAction;

        public PositionInListViewController(){
            _positionUpAction = new SimpleAction(this,"PositionUp",PredefinedCategory.RecordsNavigation){Caption = "Up"};
            _positionUpAction.Execute+=PositionUpActionOnExecute;
            _positionUpAction.SelectionDependencyType=SelectionDependencyType.RequireSingleObject;
            _positionDownAction = new SimpleAction(this,"PositionDown",PredefinedCategory.RecordsNavigation){Caption = "Down"};
            _positionDownAction.Execute += PositionDownActionOnExecute;
            _positionDownAction.SelectionDependencyType=SelectionDependencyType.RequireSingleObject;
        }

        protected override void OnActivated(){
            base.OnActivated();
            _positionColumn = ((IModelListViewPositionColumn)View.Model).PositionColumn;
            foreach (var action in Actions) {
                action.Active["PositionColumn"] = _positionColumn != null;
            }
            if (_positionColumn != null){
                _memberInfo = _positionColumn.ModelMember.MemberInfo;
            }
        }

        protected override void OnFrameAssigned(){
            base.OnFrameAssigned();
            var recordsNavigationController = Frame.GetController<RecordsNavigationController>();
            recordsNavigationController.NextObjectAction.Enabled.ResultValueChanged += (sender, args) =>{
                _positionDownAction.Enabled["NextObjectAction"] = args.NewValue;
            };
            recordsNavigationController.PreviousObjectAction.Enabled.ResultValueChanged += (sender, args) =>{
                _positionUpAction.Enabled["PreviousObjectAction"] = args.NewValue;
            };
            
        }

        private void PositionDownActionOnExecute(object sender, SimpleActionExecuteEventArgs e){
            Move(true,e.SelectedObjects.Cast<object>().First());
        }

        private void Move(bool down, object selectedObject){
            int currentPosition = (int) _memberInfo.GetValue(selectedObject);
            object otherObject = GetOtherObject(down, currentPosition);
            if (otherObject!=null){
                var otherPosition = _memberInfo.GetValue(otherObject);
                _memberInfo.SetValue(selectedObject, otherPosition);
                _memberInfo.SetValue(otherObject, currentPosition);
                ObjectSpace.CommitChanges();
            }
        }

        private object GetOtherObject(bool down, int currentPosition){
            return ((IEnumerable) View.CollectionSource.Collection).Cast<object>()
                .OrderBy(o => o).FirstOrDefault(o => IsOtherObject(down, currentPosition, o));
        }

        private bool IsOtherObject(bool down, int currentPosition, object o){
            var otherObjectPosition = (int)_memberInfo.GetValue(o);
            if (down) 
                return otherObjectPosition > currentPosition;
            else 
                return otherObjectPosition < currentPosition;
        }

        private void PositionUpActionOnExecute(object sender, SimpleActionExecuteEventArgs e){
            Move(false, e.SelectedObjects.Cast<object>().First());
        }

        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders){
            extenders.Add<IModelListView,IModelListViewPositionColumn>();
        }
    }
}
