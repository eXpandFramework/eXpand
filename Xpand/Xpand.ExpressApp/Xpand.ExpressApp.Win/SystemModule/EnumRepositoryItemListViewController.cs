using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using Xpand.Utils.Helpers;
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
                    if (gridColumn != null){
                        var tuple = (column: gridColumn, repositoryItem: gridColumn.ColumnEdit,
                            info.ModelMember.MemberInfo);
                        gridColumn.ColumnEdit = new RepositoryItemTextEdit();
                        return tuple;
                    }
                    return default;
                })
                .Where(_ => _!=default)
                .ToArray();
        }

        private bool MemberTypeIsEnum(IModelColumn modelColumn) {
            return typeof(EnumPropertyEditor).IsAssignableFrom(modelColumn.PropertyEditorType) &&
                   (modelColumn.Index > -1 && (modelColumn.ModelMember.MemberInfo.MemberType.IsEnum ||
                                               modelColumn.ModelMember.MemberInfo.MemberType.IsNullableType() &&
                                               modelColumn.ModelMember.MemberInfo.MemberType.GetGenericArguments().First().IsEnum));
        }

        GridColumn FindColumnByModel(GridListEditor gridListEditor, IModelColumn columnModel){
            var columnWrapper = gridListEditor.FindColumn(columnModel.Id) as XafGridColumnWrapper;
            return columnWrapper?.Column;
        }

        private void GridViewOnCustomRowCellEdit(object sender, CustomRowCellEditEventArgs e) {
            var data = _data.FirstOrDefault(_ => _.column==e.Column);
            if (data!=default) {
                var hasFlagsAttribute = EnumPropertyEditor.TypeHasFlagsAttribute(data.memberInfo);
                var values = EnumsNET.Enums.GetValues(data.memberInfo.MemberType).ToArray();
                e.RepositoryItem = hasFlagsAttribute? (RepositoryItem) NewRepositoryItemCheckedComboBoxEdit(values,data.repositoryItem): NewRepositoryItemEnumEdit(values,data.repositoryItem);
                var tuple = hasFlagsAttribute
                    ? ((ImageComboBoxItem[] startComboBoxItems, CheckedListBoxItem[] startCheckedListBoxItems)) (null,((RepositoryItemCheckedComboBoxEdit) e.RepositoryItem).Items.ToArray())
                    : (((RepositoryItemEnumEdit) e.RepositoryItem).Items.ToArray(), null);
                EnumPropertyEditor.FilterRepositoryItem(e.RepositoryItem, data.memberInfo,View.SelectedObjects.Cast<object>().FirstOrDefault(), ObjectSpace, tuple);
            }
        }

        private object[] GetNullItem(RepositoryItem repositoryItem) {
            return repositoryItem is RepositoryItemCheckedComboBoxEdit comboBoxEdit? new object[] {comboBoxEdit.Items.First()}
                : new object[] {((RepositoryItemEnumEdit) repositoryItem).Items.First()};
        }

        private  RepositoryItemEnumEdit NewRepositoryItemEnumEdit(object[] values,RepositoryItem repositoryItem) {
            var item = new RepositoryItemEnumEdit();
            item.Items.AddRange(GetNullItem(repositoryItem).Cast<ImageComboBoxItem>().Concat(values.Select(o => new ImageComboBoxItem(o))).ToArray());
            return item;
        }

        private  RepositoryItemCheckedComboBoxEdit NewRepositoryItemCheckedComboBoxEdit(object[] values,RepositoryItem dataRepositoryItem) {
            var item = new RepositoryItemCheckedComboBoxEdit();

            var checkedListBoxItems = GetNullItem(dataRepositoryItem).Cast<CheckedListBoxItem>().Concat(values.Select(o => new CheckedListBoxItem(o))).ToArray();
            item.Items.AddRange(checkedListBoxItems);
            return item;
        }
    }
}
