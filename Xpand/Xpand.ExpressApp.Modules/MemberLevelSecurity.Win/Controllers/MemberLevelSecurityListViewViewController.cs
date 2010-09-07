using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using System.Linq;

namespace Xpand.ExpressApp.MemberLevelSecurity.Win.Controllers
{
    public class MemberLevelSecurityListViewViewController : ViewController<XpandListView>
    {
        private GridControl gridControl;


        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            gridControl = (View.Control) as GridControl;
            if (gridControl != null) {
                XafGridView xafGridView = ((GridListEditor) (View).Editor).GridView;
                xafGridView.CustomRowCellEdit += CustomRowCellEdit;
                xafGridView.ShowingEditor += XafGridViewOnShowingEditor;
            }
        }

        void XafGridViewOnShowingEditor(object sender, CancelEventArgs cancelEventArgs) {
            var gridView = ((GridView)sender);
            GridColumn column = gridView.FocusedColumn;
            var baseObject = gridView.GetRow(gridView.FocusedRowHandle);
            bool canNotWrite = CanNotWrite(column.FieldName, baseObject);
            cancelEventArgs.Cancel = canNotWrite;
        }



        bool CanNotRead(string propertyName, object currentObject) {
            bool content = !(View.ObjectTypeInfo.FindMember(propertyName) == null || DataManipulationRight.CanRead(View.ObjectTypeInfo.Type, propertyName, null,View.CollectionSource));
            var fit=((MemberLevelObjectAccessComparer)ObjectAccessComparerBase.CurrentComparer).Fit(currentObject,MemberOperation.Read);
            return content && fit;
        }

        private void CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
        {
            if (View == null) return;
            var gridView = ((GridView)sender);
            var baseObject = gridView.GetRow(e.RowHandle);
            bool canNotRead = CanNotRead(e.Column.FieldName, baseObject);
            IMemberInfo memberInfo = View.ObjectTypeInfo.FindMember(e.Column.FieldName);
            IModelColumn modelColumn = GetModelColumn(memberInfo);
            if (modelColumn != null)
                e.RepositoryItem = ((GridListEditor)View.Editor).RepositoryFactory.CreateRepositoryItem(canNotRead , modelColumn, View.ObjectTypeInfo.Type);
        }

        IModelColumn GetModelColumn(IMemberInfo memberInfo) {
            return View.Model.Columns.Where(column =>column.ModelMember!=null&& column.ModelMember.MemberInfo == memberInfo).SingleOrDefault();
        }

        bool CanNotWrite(string fieldName, object baseObject) {
            return !(View.ObjectTypeInfo.FindMember(fieldName) == null || DataManipulationRight.CanEdit(View.ObjectTypeInfo.Type, fieldName, baseObject, null));
        }
    }
}