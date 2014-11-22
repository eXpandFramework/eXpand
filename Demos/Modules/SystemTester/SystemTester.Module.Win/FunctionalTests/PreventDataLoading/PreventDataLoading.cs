using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Win.Editors;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView;

namespace SystemTester.Module.Win.FunctionalTests.PreventDataLoading {
    public class PreventDataLoading : Module.FunctionalTests.PreventDataLoading.PreventDataLoading {
        private const string ActiveFilterEnabled = "ActiveFilterEnabled";
        private const string ActiveFilter = "ActiveFilter";
        public PreventDataLoading(){
            SingleChoiceAction.Items.Add(new ChoiceActionItem(ActiveFilterEnabled, null));
            SingleChoiceAction.Items.Add(new ChoiceActionItem(ActiveFilter, null));
            SingleChoiceAction.Execute+=SingleChoiceActionOnExecute;
        }

        private void SingleChoiceActionOnExecute(object sender, SingleChoiceActionExecuteEventArgs e){
            var gridView = ((WinColumnsListEditor)View.Editor).GridView();
            if (e.SelectedChoiceActionItem.Id == ActiveFilter){
                gridView.ActiveFilterCriteria = CriteriaOperator.Parse("Contains([Name], 'eXpandFramework')");
            }
            else if (e.SelectedChoiceActionItem.Id == ActiveFilterEnabled){
                gridView.ActiveFilterEnabled = !gridView.ActiveFilterEnabled;
            }
        }
    }
}
