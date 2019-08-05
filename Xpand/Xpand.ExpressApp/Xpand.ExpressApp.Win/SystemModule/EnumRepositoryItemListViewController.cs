using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using Xpand.Utils.Helpers;
using EnumPropertyEditor = Xpand.ExpressApp.Win.PropertyEditors.EnumPropertyEditor;

namespace Xpand.ExpressApp.Win.SystemModule {
    public class EnumRepositoryItemGridListEditorController:ViewController<ListView> {
        private IEnumerable<IMemberInfo> _memberInfos;
        private IEnumerable<(GridColumn column, RepositoryItem repositoryItem,IMemberInfo memberInfo)> _repos;

        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            if (View.Editor is GridListEditor gridListEditor) {
                _memberInfos = View.Model.Columns.Where(_ =>_.Index > -1 && (_.ModelMember.MemberInfo.MemberType.IsEnum ||
                            _.ModelMember.MemberInfo.MemberType.IsNullableType() && _.ModelMember.MemberInfo.MemberType.GetGenericArguments().First().IsEnum))
                    .Select(_ => _.ModelMember.MemberInfo).ToArray();
                _repos = _memberInfos.Select(info => {
                    var gridColumn = gridListEditor.GridView.Columns[info.Name];
                    var tuple = (column: gridColumn,repositoryItem: gridColumn.ColumnEdit,info);
                    gridColumn.ColumnEdit=new RepositoryItemTextEdit();
                    return tuple;
                }).ToArray();
                gridListEditor.GridView.CustomRowCellEditForEditing+=GridViewOnCustomRowCellEdit;
            }
        }

        private void GridViewOnCustomRowCellEdit(object sender, CustomRowCellEditEventArgs e) {
            var data = _repos.FirstOrDefault(_ => _.column==e.Column);
            if (data!=default) {
                var hasFlagsAttribute = EnumPropertyEditor.TypeHasFlagsAttribute(data.memberInfo);
                var values = EnumsNET.NonGeneric.NonGenericEnums.GetValues(data.memberInfo.MemberType).ToArray();
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
