//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Diagnostics;
//using System.Linq;
//using System.Text;
//using DevExpress.ExpressApp;
//using DevExpress.ExpressApp.Model;
//using DevExpress.ExpressApp.Model.DomainLogics;
//using DevExpress.ExpressApp.Web.Editors.ASPx;
//using DevExpress.ExpressApp.Web.SystemModule;
//using DevExpress.ExpressApp.Web.Utils;
//using DevExpress.Persistent.Base;
//using DevExpress.Web;
//using Xpand.Persistent.Base.General.Model;
//using Xpand.Persistent.Base.ModelAdapter;
//
//namespace ExcelImporterTester.Module.Web.Controllers {
//    [ModelAbstractClass]
//    public interface IModelColumnEditModeDataSource:IModelColumn{
//        [Category(AttributeCategoryNameProvider.Xpand)]
//        [ModelReadOnly(typeof(ModelColumnEditModeDataSourceVisibilityCalculator))]
//        bool PredifinedValuesInBatchEditMode{ get; set; }
//        [Category(AttributeCategoryNameProvider.Xpand)]
//
//        [ModelReadOnly(typeof(ModelColumnEditModeDataSourceVisibilityCalculator))]
//        [DataSourceProperty("Views")]
//        IModelListView BatchEditModeLookupView{ get; set; }
//    }
//
//    class ModelColumnEditModeDataSourceLogic{
//        public static IModelList<IModelView> Get_Views(IModelMemberViewItem model) {
//            return new CalculatedModelNodeList<IModelView>(ViewNamesCalculator.GetViews(model, false));
//        }
//
//    }
//    class ModelColumnEditModeDataSourceVisibilityCalculator:IModelIsReadOnly{
//
//        public bool IsReadOnly(IModelNode node, string propertyName){
//            return !(propertyName == nameof(IModelColumnEditModeDataSource.BatchEditModeLookupView)
//                ? ((IModelColumn) node).ModelMember.MemberInfo.MemberTypeInfo.IsPersistent
//                : !string.IsNullOrWhiteSpace(((IModelColumn) node).PredefinedValues));
//        }
//
//        public bool IsReadOnly(IModelNode node, IModelNode childNode){
//            return false;
//        }
//    }
//
//    public class BatchLookupListViewController : ViewController<ListView>,IModelExtender {
//        //Handle the client-side event to set the grid's cell values to the editor.
//        public const string BatchEditStartEditing =
//            @"function(s,e) {     
//                var productNameColumn = s.GetColumnByField('LookupReferencedObject.Oid');
//                if (!e.rowValues.hasOwnProperty(productNameColumn.index))
//                    return;
//                var cellInfo = e.rowValues[productNameColumn.index];
//                ReferencedEdit.SetText(cellInfo.text);
//                ReferencedEdit.SetValue(cellInfo.value);
//                ReferencedEdit['grid'] = s;
//                if (e.focusedColumn === productNameColumn) {
//                    ReferencedEdit.SetFocus();
//                }
//            }";
//        //Handle the event to pass the value from the editor to the grid cell.
//        public const string BatchEditEndEditing =
//            @"function(s,e){ 
//                var productNameColumn = s.GetColumnByField('LookupReferencedObject.Oid');
//                if (!e.rowValues.hasOwnProperty(productNameColumn.index))
//                    return;
//                var cellInfo = e.rowValues[productNameColumn.index];
//                cellInfo.value = ReferencedEdit.GetValue();
//                cellInfo.text = ReferencedEdit.GetText();
//                ReferencedEdit.SetValue(null);
//            }";
//        protected override void OnActivated(){
//            base.OnActivated();
//            if (View.AllowEdit && ((IModelListViewWeb) View.Model).InlineEditMode == InlineEditMode.Batch &&View.Editor is ASPxGridListEditor listEditor){
//                listEditor.CreateCustomEditItemTemplate += listEditor_CreateCustomEditItemTemplate;
//                listEditor.CreateCustomGridViewDataColumn += listEditor_CreateCustomGridViewDataColumn;
//            }
//        }
//
//        protected override void OnDeactivated() {
//            var listEditor = (ASPxGridListEditor)View.Editor;
//            listEditor.CreateCustomEditItemTemplate -= listEditor_CreateCustomEditItemTemplate;
//            listEditor.CreateCustomGridViewDataColumn -= listEditor_CreateCustomGridViewDataColumn;
//            base.OnDeactivated();
//        }
//
//        private void listEditor_CreateCustomGridViewDataColumn(object sender, CreateCustomGridViewDataColumnEventArgs e) {
//            var column = ((IModelColumnEditModeDataSource) e.ModelColumn);
//            if (column.PredifinedValuesInBatchEditMode||column.BatchEditModeLookupView!=null){
//                var gridColumn = new GridViewDataComboBoxColumn{
//                    Name = e.ModelColumn.PropertyName,
//                    FieldName = e.ModelColumn.PropertyName 
//                };
//                var keyMember = column.ModelMember.MemberInfo.MemberTypeInfo.KeyMember;
//                
//                gridColumn.PropertiesComboBox.ValueType = keyMember.MemberType;
//                gridColumn.PropertiesComboBox.ValueField = column.PredifinedValuesInBatchEditMode? "Key"
//                    : $"{column.PropertyName}.{keyMember.Name}";
//                
//                gridColumn.PropertiesComboBox.TextField = column.PredifinedValuesInBatchEditMode?"Name":GetDefaultMemberName(column);
//                gridColumn.PropertiesComboBox.DataSource = GetDataSource(column);
//                AddScript(gridColumn);
//                e.Column = gridColumn;
//            }
//        }
//
//        private  string GetDefaultMemberName(IModelColumnEditModeDataSource column){
//            var defaultMember = column.ModelMember.MemberInfo.MemberTypeInfo.DefaultMember;
//            if (defaultMember != null) return defaultMember.Name;
//            throw new NullReferenceException($"DefaultMember of {column.ModelMember.MemberInfo.MemberTypeInfo}");
//        }
//
//        private void AddScript(GridViewDataComboBoxColumn gridColumn){
//            var script = $@"
//                    var {gridColumn.Name}Column = s.GetColumnByField('{gridColumn.PropertiesComboBox.ValueField}');
//                    if (!e.rowValues.hasOwnProperty({gridColumn.Name}Column.index))
//                        return;
//                    var cellInfo{gridColumn.Name} = e.rowValues[{gridColumn.Name}Column.index];
//                    cellInfo{gridColumn.Name}.value = {gridColumn.Name}.GetValue();
//                    cellInfo{gridColumn.Name}.text = {gridColumn.Name}.GetText();
//                    {gridColumn.Name}.SetValue(null);";
//
//            _batchEditEndEditngScript.Append(script);
//
//            script = $@"
//                    var {gridColumn.Name}Column = s.GetColumnByField('{gridColumn.PropertiesComboBox.ValueField}');
//                    if (!e.rowValues.hasOwnProperty({gridColumn.Name}Column.index))
//                        return;
//                    var cellInfo{gridColumn.Name} = e.rowValues[{gridColumn.Name}Column.index];
//                    {gridColumn.Name}.SetText(cellInfo{gridColumn.Name}.text);
//                    {gridColumn.Name}.SetValue(cellInfo{gridColumn.Name}.value);
//                    {gridColumn.Name}['grid'] = s;
//                    if (e.focusedColumn === {gridColumn.Name}Column) {{
//                        {gridColumn.Name}.SetFocus();
//                    }};";
//            _batchEditStartEditngScript.Append(script);
//        }
//
//        readonly StringBuilder _batchEditEndEditngScript=new StringBuilder();
//        readonly StringBuilder _batchEditStartEditngScript=new StringBuilder();
//
//        
//        private IEnumerable<object> GetDataSource(IModelColumnEditModeDataSource column){
//            return column.PredifinedValuesInBatchEditMode
//                ? column.PredefinedValues.Split(';').Select(s => (dynamic)new{Name=s,Key=s}): GetObjects(column.BatchEditModeLookupView);
//        }
//
//        private IEnumerable<object> GetObjects(IModelListView modelListView){
//            return ((IEnumerable) Application.CreateCollectionSource(ObjectSpace, modelListView.ModelClass.TypeInfo.Type,modelListView.Id).Collection).Cast<object>();
//        }
//
//        private void listEditor_CreateCustomEditItemTemplate(object sender, CreateCustomEditItemTemplateEventArgs e) {
//            var column = ((IModelColumnEditModeDataSource) e.ModelColumn);
//            if (column.PredifinedValuesInBatchEditMode || column.BatchEditModeLookupView != null){
//                e.Template=new LookupTemplate(GetDataSource(column),e.ModelColumn.PropertyName);
//                e.Handled = true;
//            }
//        }
//        protected void BatchValueIsUpdated(object sender, CustomUpdateBatchValueEventArgs e){
//            var modelColumn = GetColumn(e.PropertyName);
//            if (modelColumn.PredifinedValuesInBatchEditMode || modelColumn.BatchEditModeLookupView != null){
//                if (e.NewValue==null)
//                    modelColumn.ModelMember.MemberInfo.SetValue(e.Object,null);
//                else{
//                    var referenceObject = ObjectSpace.GetObjectByKey(modelColumn.ModelMember.MemberInfo.MemberType,e.NewValue);
//                    modelColumn.ModelMember.MemberInfo.SetValue(e.Object,referenceObject);
//                }
//
//                e.Handled = true;
//            }
//        }
//
//        private IModelColumnEditModeDataSource GetColumn(string propertyName){
//            if (!(View.Model.Columns[propertyName] is IModelColumnEditModeDataSource)){
//                var strings = propertyName.Split(';');
//                return (IModelColumnEditModeDataSource) View.Model.Columns[strings[0]];
//            }
//            return (IModelColumnEditModeDataSource) View.Model.Columns[propertyName];
//        }
//
//        protected override void OnViewControlsCreated() {
//            base.OnViewControlsCreated();
//            if (View.AllowEdit && ((IModelListViewWeb) View.Model).InlineEditMode == InlineEditMode.Batch &&
//                View.Editor is ASPxGridListEditor listEditor){
//                
//                listEditor.BatchEditModeHelper.CustomUpdateBatchValue += BatchValueIsUpdated;
//
//                string gridBatchEditStartEditingKey = $"BatchEditStartEditingKey{Guid.NewGuid()}";
//                ClientSideEventsHelper.AssignClientHandlerSafe(listEditor.Grid, "BatchEditStartEditing",
//                    "function(s,e){StartEdit(s,e);};", gridBatchEditStartEditingKey);
//                
////                ClientSideEventsHelper.AssignClientHandlerSafe(listEditor.Grid, "BatchEditStartEditing",
////                    $"function(s,e){{{_batchEditStartEditngScript}}};", gridBatchEditStartEditingKey);
//
//                string gridBatchEditEndEditingKey = $"BatchEditEndEditingKey{Guid.NewGuid()}";
//                ClientSideEventsHelper.AssignClientHandlerSafe(listEditor.Grid, "BatchEditEndEditing",
//                    "function(s,e){EndEdit(s,e);};", gridBatchEditEndEditingKey);
////                ClientSideEventsHelper.AssignClientHandlerSafe(listEditor.Grid, "BatchEditEndEditing",
////                    $"function(s,e){{{_batchEditEndEditngScript}}};", gridBatchEditEndEditingKey);
//            }
//            
//        }
//
//        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders){
//            extenders.Add<IModelColumn,IModelColumnEditModeDataSource>();
//        }
//    } 
//}
