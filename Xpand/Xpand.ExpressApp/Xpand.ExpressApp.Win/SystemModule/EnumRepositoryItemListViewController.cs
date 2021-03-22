using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using Xpand.Extensions.ReflectionExtensions;
using EnumPropertyEditor = Xpand.ExpressApp.Win.PropertyEditors.EnumPropertyEditor;

namespace Xpand.ExpressApp.Win.SystemModule {
    public class EnumRepositoryItemGridListEditorController:ViewController<ListView> {
        
        private IEnumerable<(GridColumn column, RepositoryItem repositoryItem,IMemberInfo memberInfo)> _data;

        protected override void OnViewControlsCreated(){
            if (View.Editor is GridListEditor gridListEditor){
                _data = GetData(gridListEditor);
                if (_data.Any()) {
                    gridListEditor.GridView.CustomRowCellEditForEditing += GridViewOnCustomRowCellEdit;
                }
            }
        }

        private (GridColumn column, RepositoryItem repositoryItem, IMemberInfo MemberInfo)[] GetData(GridListEditor gridListEditor){
            var columns = View.Model.Columns.Where(MemberTypeIsEnum).ToArray();
            return columns.Select(info =>{
                    var gridColumn = FindColumnByModel(gridListEditor, info);
                    return gridColumn != null ? (column: gridColumn, repositoryItem: gridColumn.ColumnEdit,info.ModelMember.MemberInfo) : default;
                })
                .Where(_ => _!=default)
                .ToArray();
        }

        private bool MemberTypeIsEnum(IModelColumn modelColumn) {
            var memberInfoMemberType = modelColumn.ModelMember.MemberInfo.MemberType;
            return typeof(EnumPropertyEditor).IsAssignableFrom(modelColumn.PropertyEditorType) &&
                   (modelColumn.Index > -1 && (memberInfoMemberType.IsEnum || memberInfoMemberType.IsNullableType() &&
                                               memberInfoMemberType.GetGenericArguments().First().IsEnum));
        }

        GridColumn FindColumnByModel(GridListEditor gridListEditor, IModelColumn columnModel){
            var columnWrapper = gridListEditor.FindColumn(columnModel.Id) as XafGridColumnWrapper;
            return columnWrapper?.Column;
        }

        private void GridViewOnCustomRowCellEdit(object sender, CustomRowCellEditEventArgs e) {
            var data = _data.FirstOrDefault(_ => _.column==e.Column);
            if (data!=default) {
                var hasFlagsAttribute = EnumPropertyEditor.TypeHasFlagsAttribute(data.memberInfo);
                var repositoryItem = GetRepositoryItem(data, hasFlagsAttribute);
                var tuple = !hasFlagsAttribute ? (((RepositoryItemEnumEdit) repositoryItem).Items.ToArray(), null)
                : ((ImageComboBoxItem[] startComboBoxItems, CheckedListBoxItem[] startCheckedListBoxItems)) (null,((RepositoryItemCheckedComboBoxEdit) repositoryItem).Items.ToArray());
                EnumPropertyEditor.FilterRepositoryItem(repositoryItem, data.memberInfo,((GridView) sender).GetRow(e.RowHandle), ObjectSpace, tuple);
                e.RepositoryItem=repositoryItem;
            }
        }

        private RepositoryItem GetRepositoryItem((GridColumn column, RepositoryItem repositoryItem, IMemberInfo memberInfo) data, bool hasFlagsAttribute){
            var memberType = PropertyEditorHelper.CalcUnderlyingType(data.memberInfo);
            var values = EnumsNET.Enums.GetValues(memberType).ToArray();
            return hasFlagsAttribute ? (RepositoryItem) NewRepositoryItemCheckedComboBoxEdit(values, data.repositoryItem)
                : new RepositoryItemEnumEdit(memberType);
        }

        private object[] GetNullItem(RepositoryItem repositoryItem) {
            return repositoryItem is RepositoryItemCheckedComboBoxEdit comboBoxEdit? new object[] {comboBoxEdit.Items.First()}
                : new object[] {((RepositoryItemEnumEdit) repositoryItem).Items.First()};
        }

        private  RepositoryItemCheckedComboBoxEdit NewRepositoryItemCheckedComboBoxEdit(object[] values,RepositoryItem dataRepositoryItem) {
            var item = new RepositoryItemCheckedComboBoxEdit();
            var checkedListBoxItems = GetNullItem(dataRepositoryItem).Cast<CheckedListBoxItem>().Concat(values.Select(o => new CheckedListBoxItem(o))).ToArray();
            item.Items.AddRange(checkedListBoxItems);
            return item;
        }
    }
}
