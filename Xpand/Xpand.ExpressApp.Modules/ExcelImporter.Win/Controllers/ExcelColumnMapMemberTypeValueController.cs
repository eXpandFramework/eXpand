using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using Xpand.ExpressApp.ExcelImporter.BusinessObjects;

namespace Xpand.ExpressApp.ExcelImporter.Win.Controllers {
    public class ExcelColumnMapMemberTypeValueController :ExcelImporter.Controllers.ExcelColumnMapMemberTypeValueController{
        private RepositoryItemPredefinedValuesStringEdit _repositoryItem;

        protected override void OnViewControlsCreated() {
            var gridView = ((GridListEditor) View.Editor).GridView;
            var gridColumn = gridView.Columns.First(column => column.FieldName==nameof(ExcelColumnMapMemberTypeValue.Type));
            _repositoryItem = ((RepositoryItemPredefinedValuesStringEdit) gridColumn.RealColumnEdit);
            _repositoryItem.CustomItemDisplayText+=RepositoryItemOnCustomItemDisplayText;
            _repositoryItem.CustomDisplayText+=RepositoryItemOnCustomDisplayText;
            base.OnViewControlsCreated();
            
        }

        private void RepositoryItemOnCustomDisplayText(object sender, CustomDisplayTextEventArgs e) {
            var typeInfo =  XafTypesInfo.Instance.FindTypeInfo($"{e.Value}");
            if (typeInfo != null) e.DisplayText = CaptionHelper.GetClassCaption(typeInfo.FullName);
        }

        private void RepositoryItemOnCustomItemDisplayText(object sender, CustomItemDisplayTextEventArgs e) {
            var typeInfo = XafTypesInfo.Instance.FindTypeInfo($"{e.Item}");
            if (typeInfo != null) e.DisplayText = CaptionHelper.GetClassCaption(typeInfo.FullName);
        }


        protected override void PopulateTypes(){
            _repositoryItem.Items.Clear();
            _repositoryItem.Items.AddRange(GetTypes().Select(type => type.FullName).Cast<object>().ToArray());
        }
    }
}
