using System;
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
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.Model;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.SystemModule {
    [ModelAbstractClass]
    public interface IModelListViewPositionColumn:IModelListView{
        [DataSourceProperty("SortableColumns")]
        [Localizable(false)]
        [Category(AttributeCategoryNameProvider.Xpand)]
        IModelColumn PositionColumn { get; set; }
        [Browsable(false)]
        IEnumerable<IModelColumn> SortableColumns { get; }
    }

    [DomainLogic((typeof(IModelListViewPositionColumn)))]
    public class ListViewPositionColumnDomainLogic{
        public static void Set_PositionColumn(IModelListViewPositionColumn listView, IModelColumn modelColumn){
            listView.SetValue("PositionColumn", modelColumn);
            var settingsBehavior = listView.GetNodeByPath("GridViewOptions/SettingsBehavior");
            settingsBehavior?.ClearValue("AllowSort");
            settingsBehavior?.ClearValue("AllowSelectSingleRowOnly");
            if (modelColumn != null){
                settingsBehavior?.SetValue("AllowSort", true);
                settingsBehavior?.SetValue("AllowSort", false);
                settingsBehavior?.SetValue("AllowSelectSingleRowOnly", true);
                modelColumn.SortIndex = 0;
                modelColumn.SortOrder=ColumnSortOrder.Ascending;
                modelColumn.Index = -1;
                foreach (var column in listView.Columns.Where(column => column!=modelColumn)){
                    column.SortOrder=ColumnSortOrder.None;
                    column.SortIndex = -1;
                }
                
            }
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
            _positionUpAction = new SimpleAction(this,"PositionUp",PredefinedCategory.View){Caption = "Up"};
            _positionUpAction.Execute+=PositionUpActionOnExecute;
            _positionUpAction.SelectionDependencyType=SelectionDependencyType.RequireSingleObject;
            _positionDownAction = new SimpleAction(this,"PositionDown",PredefinedCategory.View) {Caption = "Down"};
            _positionDownAction.Execute += PositionDownActionOnExecute;
            _positionDownAction.SelectionDependencyType=SelectionDependencyType.RequireSingleObject;
        }

        protected override void OnDeactivated(){
            base.OnDeactivated();
            if (_positionColumn != null)
                View.Editor.DataSourceChanged += EditorOnDataSourceChanged;
        }

        protected override void OnActivated(){
            base.OnActivated();
            
            _positionColumn = ((IModelListViewPositionColumn)View.Model).PositionColumn;
            foreach (var action in Actions) {
                action.Active["PositionColumn"] = _positionColumn != null;
            }
            if (_positionColumn != null){
                
                _memberInfo = _positionColumn.ModelMember.MemberInfo;
                View.Editor.DataSourceChanged+=EditorOnDataSourceChanged;
            }
        }

        private void EditorOnDataSourceChanged(object sender, EventArgs eventArgs){
            if (View?.CollectionSource.List != null && _positionColumn != null){
                var orderedObjects = GetObjects(null).ToArray();
                for (var index = 0; index < orderedObjects.Length; index++){
                    var orderedObject = orderedObjects[index];
                    _memberInfo.SetValue(orderedObject, index);
                }
            }
        }

        protected override void OnFrameAssigned(){
            base.OnFrameAssigned();
            Frame.GetController<RecordsNavigationController>(recordsNavigationController => {
                recordsNavigationController.NextObjectAction.Enabled.ResultValueChanged += (sender, args) => {
                    _positionDownAction.Enabled["Move"] = args.NewValue;
                };
                recordsNavigationController.PreviousObjectAction.Enabled.ResultValueChanged += (sender, args) => {
                    _positionUpAction.Enabled["Move"] = args.NewValue;
                };
            });
            
            
        }

        private void PositionDownActionOnExecute(object sender, SimpleActionExecuteEventArgs e){
            Move(true,e.SelectedObjects.Cast<object>().First());
        }

        private void Move(bool down, object selectedObject){
            int currentPosition = (int) _memberInfo.GetValue(selectedObject);
            object otherObject = GetOtherObject(down, currentPosition,selectedObject);
            var otherPosition = _memberInfo.GetValue(otherObject);
            _memberInfo.SetValue(selectedObject, otherPosition);
            _memberInfo.SetValue(otherObject, currentPosition);
            ObjectSpace.CommitChanges();

            currentPosition = (int)_memberInfo.GetValue(selectedObject);
            otherObject = GetOtherObject(down, currentPosition, selectedObject);
            if (down){
                _positionUpAction.Enabled["Move"] = true;
                _positionDownAction.Enabled["Move"] = otherObject != null;
            }
            else{
                _positionDownAction.Enabled["Move"] = true;
                _positionUpAction.Enabled["Move"] = otherObject != null;
            }
        }

        private object GetOtherObject(bool down, int currentPosition, object selectedObject){
            var objects = GetObjects(selectedObject);
            if (!down)
                objects = objects.Reverse();
            return objects.FirstOrDefault(o => IsOtherObject(down, currentPosition, o));
        }

        private IEnumerable<object> GetObjects(object selectedObject){
            return View.CollectionSource.List.Cast<object>().OrderBy(o => _memberInfo.GetValue(o)).Where(o => o != selectedObject);
        }

        private bool IsOtherObject(bool down, int currentPosition, object o){
            var otherObjectPosition = (int)_memberInfo.GetValue(o);
            return down ? otherObjectPosition > currentPosition : otherObjectPosition < currentPosition;
        }

        private void PositionUpActionOnExecute(object sender, SimpleActionExecuteEventArgs e){
            Move(false, e.SelectedObjects.Cast<object>().First());
        }

        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders){
            extenders.Add<IModelListView,IModelListViewPositionColumn>();
        }
    }
}
