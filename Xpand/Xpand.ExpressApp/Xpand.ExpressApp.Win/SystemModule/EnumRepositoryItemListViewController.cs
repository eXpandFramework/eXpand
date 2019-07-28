using System;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win.Editors;
using Xpand.Utils.Helpers;
using EnumPropertyEditor = Xpand.ExpressApp.Win.PropertyEditors.EnumPropertyEditor;

namespace Xpand.ExpressApp.Win.SystemModule {
    public class EnumRepositoryItemGridListEditorController:ViewController<ListView> {
        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            if (View.Editor is GridListEditor gridListEditor) {
                View.SelectionChanged+=ViewOnSelectionChanged;
                FilterRepositories(gridListEditor);
            }
            
        }

        private void FilterRepositories(GridListEditor gridListEditor){
            var memberInfos = View.Model.Columns.Where(_ =>
                    _.Index > -1 && (_.ModelMember.MemberInfo.MemberType.IsEnum ||
                                     _.ModelMember.MemberInfo.MemberType.IsNullableType() && _.ModelMember.MemberInfo
                                         .MemberType.GetGenericArguments().First().IsEnum))
                .Select(_ => _.ModelMember.MemberInfo);
            foreach (var memberInfo in memberInfos){
                var repositoryItem = gridListEditor.GridView.Columns[memberInfo.Name].ColumnEdit;
                var itemsData = EnumPropertyEditor.GetItemsData(repositoryItem, memberInfo);
                EnumPropertyEditor.FilterRepositoryItem(repositoryItem, memberInfo,
                    View.SelectedObjects.Cast<object>().FirstOrDefault(), ObjectSpace, itemsData);
            }
        }

        protected override void OnDeactivated() {
            base.OnDeactivated();
            View.SelectionChanged-=ViewOnSelectionChanged;
        }

        private void ViewOnSelectionChanged(object sender, EventArgs e) {
            
            FilterRepositories((GridListEditor) View.Editor);
        }
    }
}
