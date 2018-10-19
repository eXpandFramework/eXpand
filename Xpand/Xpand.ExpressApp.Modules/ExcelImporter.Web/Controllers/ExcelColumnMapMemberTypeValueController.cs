using System.Linq;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.Web;
using Xpand.ExpressApp.ExcelImporter.BusinessObjects;

namespace Xpand.ExpressApp.ExcelImporter.Web.Controllers{
    public class ExcelColumnMapMemberTypeValueController:ExcelImporter.Controllers.ExcelColumnMapMemberTypeValueController {
        

        protected override void PopulateTypes() {
            base.PopulateTypes();
            var grid = ((ASPxGridListEditor) View.Editor).Grid;
            var dataComboBoxColumn = grid.Columns
                .OfType<GridViewDataComboBoxColumn>()
                .First(column => column.FieldName == nameof(ExcelColumnMapMemberTypeValue.Type));
            var comboBox = dataComboBoxColumn.PropertiesComboBox;
            comboBox.AllowNull = false;
            comboBox.DropDownStyle=DropDownStyle.DropDownList;
            comboBox.Items.Clear();
            var types = GetTypes();
            var sources = types
                .Select(type => new ListEditItem(CaptionHelper.GetClassCaption(type.FullName),type)).ToArray();
            comboBox.Items.AddRange(sources);

        }


    }
}