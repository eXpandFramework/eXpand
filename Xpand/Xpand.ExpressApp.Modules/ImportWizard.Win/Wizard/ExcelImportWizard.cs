using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraWizard;
using DocumentFormat.OpenXml.Packaging;
using Xpand.ExpressApp.ImportWizard.Core;
using Xpand.ExpressApp.ImportWizard.Settings;
using Xpand.ExpressApp.ImportWizard.Win.Forms;
using Xpand.ExpressApp.ImportWizard.Win.Properties;

namespace Xpand.ExpressApp.ImportWizard.Win.Wizard {
    public delegate void ExcelImportWizardStringToPropertyMap(XPObjectSpace objectSpace, XPMemberInfo prop, string value, ref IXPSimpleObject newObj);
    public partial class ExcelImportWizard : XtraForm {

        readonly XafApplication _application;
        readonly ExcelImportWizardStringToPropertyMap _propertyValueMapper;

        #region Initialization

        //Extra constructor to enable Value mapping customisation
        public ExcelImportWizard(XPObjectSpace objectSpace, ITypeInfo typeInfo,
            CollectionSourceBase collectionSourceBase, XafApplication application,
            StringValueMapper valueMapper)
            : this(objectSpace, typeInfo, collectionSourceBase, application, valueMapper.MapValueToObjectProperty) {
        }


        public ExcelImportWizard(XPObjectSpace objectSpace, ITypeInfo typeInfo, CollectionSourceBase collectionSourceBase, XafApplication application,
            ExcelImportWizardStringToPropertyMap propertyValueMapper = null) {
            _application = application;
            _propertyValueMapper = propertyValueMapper ?? new StringValueMapper().MapValueToObjectProperty;
            //set local variable values
            if (objectSpace == null)
                throw new ArgumentNullException("objectSpace", Resources.ExcelImportWizard_ExcelImportWizard_ObjectSpace_cannot_be_NULL);

            ObjectSpace = objectSpace;
            CurrentCollectionSource = collectionSourceBase;

            Type = typeInfo.Type;

            InitializeComponent();

            ImportMapLookUp.Properties.DataSource = ImportMapsCollection.ToList();

            //disable next, until file and other info is selected
            welcomeWizardPage1.AllowNext = false;
            wizardPage1.AllowNext = false;



            gridLookUpEdit1.Properties.View.OptionsBehavior.AutoPopulateColumns = true;
            gridLookUpEdit1.Properties.DataSource = MappableColumns;


            var mappablePropertyClassInfo
                = objectSpace.Session.GetClassInfo(typeof(MappableProperty));


            foreach (GridColumn column in gridLookUpEdit2View.Columns) {

                column.Caption = mappablePropertyClassInfo
                                    .GetMember(column.FieldName).DisplayName;
                if (column.FieldName == @"Mapped")
                    column.Visible = false;
            }
        }


        /// <summary>
        /// Fill MRU item list with values
        /// </summary>
        private void ExcelImportWizard_Load(object sender, EventArgs e) {
            _mus = new MyUserSettings();
            if (_mus.MRUItems != null)
                FileSelectEdit.Properties.Items.AddRange(_mus.MRUItems);
        }
        /// <summary>
        /// Save MRU item list with values
        /// </summary>
        private void ExcelImportWizard_FormClosing(object sender, FormClosingEventArgs e) {
            if (_mus.MRUItems == null)
                _mus.MRUItems = new List<string>();
            else
                _mus.MRUItems.Clear();
            foreach (var item in FileSelectEdit.Properties.Items) {
                _mus.MRUItems.Add(item.ToString());
            }

            _mus.Save();

            if (ObjectSpace.Session.InTransaction)
                ObjectSpace.Session.RollbackTransaction();

            if (ExcelDocument != null)
                ExcelDocument.Close();
        }

        #endregion

        private MyUserSettings _mus;
        public XPObjectSpace ObjectSpace { get; private set; }
        public CollectionSourceBase CurrentCollectionSource { get; private set; }

        private SpreadsheetDocument ExcelDocument { get; set; }
        private Sheet _sheet;
        public Sheet Sheet {
            get { return _sheet; }
            private set {
                _sheet = value;
                AssignDataSource();

            }
        }

        private ImportMap _importMap;
        public ImportMap ImportMap {
            get { return _importMap; }
            set {
                _importMap = value;
                AssingMapping(value);
            }
        }

        private Type _type;
        public Type Type {
            get { return _type; }
            private set {
                _type = value;

                if (_type == null) return;
                var props = ObjectSpace.Session.GetClassInfo(_type)
                                .PersistentProperties as IEnumerable<XPMemberInfo>;
                if (props != null)
                    //todo: allow to use only properties that user can modify
                    MappableColumns =
                        props.Select(p => new MappableProperty(ObjectSpace.Session) {
                            Name = p.Name,
                            DisplayName = p.DisplayName,
                            Mapped = false
                        }).ToList();
            }
        }



        private List<MappableProperty> MappableColumns { get; set; }
        private IEnumerable<ImportMap> ImportMapsCollection {
            get {
                return ObjectSpace.Session.
                        GetObjects(ObjectSpace.Session.GetClassInfo(typeof(ImportMap)),
                                   null, null, 0, false, true).OfType<ImportMap>();
            }
        }

        private void AssignDataSource() {
            ((GridView)ExcelSheetPreviewGrid.MainView).Columns.Clear();
            ExcelSheetPreviewGrid.DataSource = _sheet == null ? null : _sheet.DataPreviewTable();
        }

        #region Page commit



        private void WizardControl_SelectedPageChanged(object sender, WizardPageChangedEventArgs e) {
            if (e.Page == wizardPage1) {
                if (e.PrevPage == welcomeWizardPage1) InitWizPage1();
            }
            if (e.Page == wizardPage2 && e.PrevPage == wizardPage1 && MappingRadioGroup.SelectedIndex == 1) {

            }

        }



        private void WizardControl_SelectedPageChanging(object sender, WizardPageChangingEventArgs e) {
            if (e.PrevPage == wizardPage1 && e.Direction == Direction.Forward) {
                e.Page = wizardPage3;
            }


            if (e.PrevPage == wizardPage2 && e.Direction == Direction.Forward) {
                if ((bool)radioGroup2.EditValue) {
                    ImportMap.Description = ImportMapDescriptionEdit.Text;
                    ImportMap.Save();
                    //var uow = new UnitOfWork(ObjectSpace.Session.DataLayer);
                    //var a = uow.(ImportMap.Oid);
                    //a.Save();
                    //uow.CommitTransaction();

                    ObjectSpace.Session.Reload(ImportMap);
                }
            }

            if (e.PrevPage == wizardPage3 && e.Direction == Direction.Backward) {
                e.Page = wizardPage1;
            }
            if (e.PrevPage == welcomeWizardPage1 && e.Direction == Direction.Forward) {
                MappingRadioGroup.SelectedIndex = 1;
            }
        }

        #endregion

        private void InitWizPage1() {

            repositoryItemGridLookUpEdit.DataSource = MappableColumns;
            repositoryItemGridLookUpEdit.DisplayMember = @"Name";
            repositoryItemGridLookUpEdit.ValueMember = @"Name";

            ImportMapLookUp.Properties.DataSource = ImportMapsCollection.ToList();
            ImportMapLookUp.EditValue = null;

            //check for duplicate columns and throw error if found
            if (Sheet.Columns()
                .GroupBy(d => d.Name)
                .Select(g => new { g.Key, count = g.Count() }).Any(g => g.count > 1))
                throw new InvalidDataException(Resources.ExcelImportWizard_InitWizPage1_Duplicate_column_names_found__please_fix_excel_column_names);

            //Assign data for Mapping Grid
            var dt = _sheet.DataPreviewTable().Transpose();

            dt.RowChanged -= MappingGrid_RowChanged;
            var mappingGirdView = ((BandedGridView)MappingGrid.MainView);

            mappingGirdView.Columns.Clear();
            mappingGirdView.BestFitColumns();
            MappingGrid.DataSource = dt;

            Initialize_GridControl2_AfterDataSourceChanged();

            //Register to Mapping data table change events
            //this will be used to synch grid data with Import map object
            dt.RowChanged += MappingGrid_RowChanged;
            MappingGrid_RowChanged(dt, null);


        }

        /// <summary>
        /// Tracks changes in the mapping data table
        /// Synchronizes them to the Import map object
        /// Takes care of some interface actions
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MappingGrid_RowChanged(object sender, DataRowChangeEventArgs e) {
            //when saved map is selected and something changes in the grid
            //changes radio selection to a custom map
            if (!_ignoreDtChanges && MappingRadioGroup.SelectedIndex != 1)
                MappingRadioGroup.SelectedIndex = 1;

            var allowNext = false;
            var table = ((DataTable)sender);

            //Enables next button if at least one column is mapped
            if (table.Rows.OfType<DataRow>()
                .Where(p => {
                    var lastOrDefault = p.ItemArray.LastOrDefault();
                    return lastOrDefault != null && !string.IsNullOrEmpty(lastOrDefault.ToString());
                })
                .GroupBy(p => p.ItemArray.LastOrDefault())
                .Select(g => new { g.Key, count = g.Count() }).Any())
                allowNext = true;

            //select all rows that are mapped
            var gridMappings = table.Rows.OfType<DataRow>()
                .Where(p => {
                    var orDefault = p.ItemArray.LastOrDefault();
                    return orDefault != null && !string.IsNullOrEmpty(orDefault.ToString());
                });

            //synchronize columns mappings with Importmap object
            foreach (var gridMapping in gridMappings) {
                var mapping = gridMapping;
                var firstOrDefault = mapping.ItemArray.FirstOrDefault();
                var mpng = ImportMap.Mappings.FirstOrDefault(p => firstOrDefault != null && p.Column == firstOrDefault.ToString()) ??
                           new Mapping(ObjectSpace.Session) {
                               Map = ImportMap
                           };
                mpng.Column = gridMapping.ItemArray.First().ToString();
                var lastOrDefault = gridMapping.ItemArray.LastOrDefault();
                if (lastOrDefault != null) {
                    var mapedTo = lastOrDefault.ToString();
                    mpng.MapedTo = mapedTo;
                    var mappableProperty = MappableColumns.FirstOrDefault(p => p.Name == mapedTo);
                    if (mappableProperty != null)
                        mappableProperty.Mapped = true;
                }
            }

            gridLookUpEdit2View.RefreshData();


            wizardPage1.AllowNext = allowNext;

        }

        /// <summary>
        /// Takes care of how data is displayed in the Mapping grid
        /// Moves corresponding columns to corresponding bands
        /// </summary>
        private void Initialize_GridControl2_AfterDataSourceChanged() {
            //Banded Grid Control that prepared data for mapping

            var bands = ((BandedGridView)MappingGrid.MainView).Bands;
            var bandedGridView = MappingGrid.MainView as BandedGridView;

            if (bandedGridView == null) return;
            var cols = bandedGridView.Columns;

            foreach (BandedGridColumn col in cols) {
                if (col.AbsoluteIndex != 0 && col.AbsoluteIndex != cols.Count - 1) {
                    //move value columns to Values band
                    col.OwnerBand = bands[@"Values"];
                }
                if (col.AbsoluteIndex == cols.Count - 1) {
                    //Moves the LAST !!! column to Mapping band
                    col.OwnerBand = bands[@"MapTo"];
                    col.ColumnEdit = repositoryItemGridLookUpEdit;
                    col.Width = 100;
                }

            }
        }

        #region UI Event Handlers

        /// <summary>
        /// File select editor, drop down button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FileSelectEdit_ButtonClick(object sender, ButtonPressedEventArgs e) {
            var mruEdit = sender as MRUEdit;
            if (mruEdit != null)
                if (mruEdit.Properties.Buttons.IndexOf(e.Button)
                    == mruEdit.Properties.ActionButtonIndex) return;
            var dlg = new OpenFileDialog {
                Filter = Resources.ExcelImportWizard_FileSelectEdit_ButtonClick_Excel_Files___xlsx____xlsx
            };
            if (dlg.ShowDialog() != DialogResult.OK) return;
            if (mruEdit == null) return;
            mruEdit.EditValue = new FileInfo(dlg.FileName).FullName;
            mruEdit.Properties.Items.Add(mruEdit.Text);

        }

        //stuff that happens after file is selected
        private void FileSelectEdit_EditValueChanged(object sender, EventArgs e) {

            var edit = (sender as MRUEdit);
            if (edit == null) return;
            if (string.IsNullOrEmpty(edit.Text) || !new FileInfo(edit.Text).Exists) {
                SheetSelectEdit.Properties.DataSource = null;
                SheetSelectEdit.EditValue = null;
            }
            else {
                try {
                    ExcelDocument = SpreadsheetDocument.Open(edit.Text, false);

                    var sheets = ExcelDocument.Sheets()
                        .Select(p => p.OXmlSheet.Name)
                        .ToList();
                    SheetSelectEdit.Properties.DataSource = sheets;
                    if (sheets.Count == 1)
                        SheetSelectEdit.EditValue = sheets.FirstOrDefault();

                    ImportMapDescriptionEdit.Text = (new FileInfo(edit.Text)).Name;
                }
                catch {
                    if (ExcelDocument != null) ExcelDocument.Close();
                    throw;
                }

            }

            var b = SheetSelectEdit.Properties.DataSource != null;
            SheetSelectEdit.Enabled = b;
            welcomeWizardPage1.AllowNext = b;
        }

        /// <summary>
        /// header row CheckEdit, enables additional options for selecting a header row in excel file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HeaderRowCheck_CheckedChanged(object sender, EventArgs e) {
            HeaderRowSpinEdit.Enabled = ((CheckEdit)sender).Checked;
            PrieviewRowCountSpinEdit.Enabled = ((CheckEdit)sender).Checked;
            _sheet.ColumnHeaderRow = ((CheckEdit)sender).Checked
                                         ?
                                            decimal.ToInt32(HeaderRowSpinEdit.Value)
                                         : (int?)null;
            _sheet.PreviewRowCount = ((CheckEdit)sender).Checked
                                         ?
                                             decimal.ToInt32(PrieviewRowCountSpinEdit.Value)
                                         :
                                             (int?)null;
            AssignDataSource();

        }

        /// <summary>
        /// sets the row number of header row 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HeaderRowSpinEdit_EditValueChanged(object sender, EventArgs e) {
            _sheet.ColumnHeaderRow = decimal.ToInt32(((SpinEdit)sender).Value);
            AssignDataSource();
        }

        /// <summary>
        /// Clear excel sheet selector value if it gets disabled
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SheetSelectEdit_EnabledChanged(object sender, EventArgs e) {
            if (((LookUpEdit)sender).Enabled == false)
                ((LookUpEdit)sender).EditValue = null;
        }

        /// <summary>
        /// Clears the value of Excel sheet selector if Sheet list is changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SheetSelectEdit_ListChanged(object sender, ListChangedEventArgs e) {
            SheetSelectEdit.EditValue = null;
            SheetSelectEdit_EnabledChanged(sender, e);
        }

        /// <summary>
        /// Controls stuff that happens when excel sheet gets selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SheetSelectEdit_EditValueChanged(object sender, EventArgs e) {
            var sht = ((LookUpEdit)sender).EditValue;

            if (sht != null) {
                HeaderRowCheck.Checked = false;
                Sheet = ExcelDocument.Sheets().FirstOrDefault(p => p.OXmlSheet.Name == sht.ToString());
            }
            else {
                HeaderRowCheck.Checked = false;
                Sheet = null;
            }

            HeaderRowCheck.Enabled = Sheet != null;
            ExcelSheetPreviewGrid.Enabled = Sheet != null;

        }

        /// <summary>
        /// repository lookup handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void LookUpEdit_ButtonClick(object sender, ButtonPressedEventArgs e) {
        //    if (e.Button.Index != 1) return;

        //    ((LookUpEdit)sender).EditValue = null;
        //    ((GridControl)((LookUpEdit)sender).Parent).MainView.PostEditor();
        //    ((GridControl)((LookUpEdit)sender).Parent).MainView.UpdateCurrentRow();


        private void PrieviewRowCountSpinEdit_EditValueChanged(object sender, EventArgs e) {
            _sheet.PreviewRowCount = decimal.ToInt32(((SpinEdit)sender).Value);
            AssignDataSource();
        }

        private void MappingRadioGroup_SelectedIndexChanged_1(object sender, EventArgs e) {
            ImportMapLookUp.Enabled = MappingRadioGroup.SelectedIndex == 0;
            GuesMappingButton.Enabled = MappingRadioGroup.SelectedIndex == 1;


            switch (MappingRadioGroup.SelectedIndex) {
                case 0:
                    ImportMap = ObjectSpace.Session.GetObjectByKey<ImportMap>(ImportMapLookUp.EditValue, false);
                    break;
                case 1:
                    if (ImportMap != null) {
                        //var c = new Cloner();
                        //var cc = c.CloneTo(ImportMap, typeof(ImportMap));
                        //(cc as ImportMap).Description = string.Empty;
                        ////var im = Hellper.Clone(ImportMap, Session);
                        ////var im = ImportMap.Clone(typeof (ImportMap));
                        //ImportMap = (ImportMap)cc;
                    }
                    else ImportMap = new ImportMap(ObjectSpace.Session);

                    break;
                default:
                    throw new NotImplementedException();
            }


        }

        private void textEdit1_TextChanged(object sender, EventArgs e) {
            wizardPage2.AllowNext = ImportMapDescriptionEdit.Text != string.Empty;
        }

        private void GuesMappingButton_Click(object sender, EventArgs e) {
            GuesMappings();
        }

        private void ImportMapLookUp_EditValueChanged(object sender, EventArgs e) {
            ImportMap = ObjectSpace.Session.GetObjectByKey<ImportMap>(ImportMapLookUp.EditValue);
        }

        private void ResetButton_Click(object sender, EventArgs e) {
            InitWizPage1();
        }

        private void radioGroup2_EditValueChanged(object sender, EventArgs e) {
            ImportMapDescriptionEdit.Enabled = (bool)radioGroup2.EditValue;
        }

        /// <summary>
        /// Clear the MapsTo column Cell
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void repositoryItemGridLookUpEdit_ButtonClick(object sender, ButtonPressedEventArgs e) {
            if (e.Button.Index != 1) return;

            ((GridLookUpEdit)sender).EditValue = null;
            ((GridControl)((GridLookUpEdit)sender).Parent).MainView.PostEditor();
            ((GridControl)((GridLookUpEdit)sender).Parent).MainView.UpdateCurrentRow();
        }

        /// <summary>
        /// Performs post editor 
        /// Needed so datasource would be updated imediately 
        /// (by default it happens when user navigates to another record)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void repositoryItemGridLookUpEdit_Closed(object sender, ClosedEventArgs e) {
            BaseView mainView = ((GridControl)((GridLookUpEdit)sender).Parent).MainView;
            mainView.PostEditor();
            mainView.UpdateCurrentRow();
        }

        #endregion

        #region Mapping Procedures

        private bool _ignoreDtChanges;
        private void AssingMapping(ImportMap importMap) {
            _ignoreDtChanges = true;
            var dt = MappingGrid.DataSource as DataTable;
            if (dt == null || importMap == null) return;

            var rows = dt.Rows;

            for (var i = 0; i < rows.Count; i++) {
                if (!string.IsNullOrEmpty(rows[i][dt.Columns.Count - 1].ToString())) continue;

                var index = i;
                var mapping = importMap.Mappings.FirstOrDefault(p => p.Column == rows[index][0].ToString());

                if (mapping != null) {
                    var mapedTo = mapping.MapedTo;
                    dt.Rows[i][dt.Columns.Count - 1] = mapedTo;
                    var mappableCol = MappableColumns.FirstOrDefault(p => p.Name == mapedTo);
                    if (mappableCol != null) mappableCol.Mapped = true;
                }

                dt.Rows[i][dt.Columns.Count - 1] = null;
            }
            _ignoreDtChanges = false;
        }

        private void GuesMappings() {
            var dt = MappingGrid.DataSource as DataTable;
            if (dt == null || ImportMap == null) return;

            var rows = dt.Rows;

            for (var i = 0; i < rows.Count; i++) {
                var dataRow = rows[i];
                var mapsTo =
                    (object)MappableColumns
                                .Where(p => p.Name.ToUpper().Replace(@" ", "").Replace(@"_", "")
                                    == dataRow.ItemArray.First().ToString().ToUpper().Replace(@" ", "").Replace(@"_", "") ||
                                    p.DisplayName.ToUpper().Replace(@" ", "").Replace(@"_", "")
                                    == dataRow.ItemArray.First().ToString().ToUpper().Replace(@" ", "").Replace(@"_", ""))
                                .Select(p => p.Name)
                                .FirstOrDefault()
                    ;

                dt.Rows[i][dataRow.ItemArray.Length - 1] = mapsTo != null ? mapsTo.ToString() : null;
            }
        }


        #endregion

        #region Import Data

        private BackgroundWorker _bgWorker;
        private ProgressForm _frmProgress;


        private void ImportButton_Click(object sender, EventArgs e) {

            var rowCount = Sheet.Rows().Count();
            if (Sheet.ColumnHeaderRow != null)
                rowCount = (int)(rowCount - Sheet.ColumnHeaderRow);

            _frmProgress = new ProgressForm(Resources.ExcelImportWizard_ImportButton_Click_Import_excell_rows_progress___, rowCount, @"Processing record {0} of {1} ");
            _frmProgress.CancelClick += FrmProgressCancelClick;

            _bgWorker = new BackgroundWorker {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };

            _bgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
            _bgWorker.ProgressChanged += BgWorkerProgressChanged;
            _bgWorker.DoWork += BgWorkerDoWork;

            _bgWorker.RunWorkerAsync(new WorkerArgs(Sheet.Rows(),Sheet.ColumnHeaderRow));

            _frmProgress.ShowDialog();

        }

        private void AddNewObjectToCollectionSource(CollectionSourceBase currentCollectionSource, object newObject, XPObjectSpace objectSpace) {
            var newObjectTypeInfo = XafTypesInfo.Instance.FindTypeInfo(newObject.GetType());
            if ((currentCollectionSource == null) ||
                !currentCollectionSource.ObjectTypeInfo.IsAssignableFrom(newObjectTypeInfo)) return;

            if (objectSpace == currentCollectionSource.ObjectSpace)
                currentCollectionSource.Add(newObject);
            else {
                var propertyCollectionSource = (currentCollectionSource as PropertyCollectionSource);
                if ((propertyCollectionSource != null) && (propertyCollectionSource.MasterObject != null)) {
                    Object collectionOwner;
                    IMemberInfo memberInfo = null;
                    if (propertyCollectionSource.MemberInfo.GetPath().Count > 1) {
                        collectionOwner = ImportUtils.GetCollectionOwner(propertyCollectionSource.MasterObject,
                            propertyCollectionSource.MemberInfo);
                        if (collectionOwner != null)
                            memberInfo =
                                XafTypesInfo.Instance.FindTypeInfo(collectionOwner.GetType())
                                    .FindMember(propertyCollectionSource.MemberInfo.LastMember.Name);
                    }
                    else {
                        collectionOwner = propertyCollectionSource.MasterObject;
                        memberInfo = propertyCollectionSource.MemberInfo;
                    }
                    if ((collectionOwner != null) &&
                        XafTypesInfo.Instance.FindTypeInfo(collectionOwner.GetType()).IsPersistent) {
                        var collectionSource = _application.CreatePropertyCollectionSource(objectSpace, null,
                            objectSpace.GetObject(collectionOwner), memberInfo, "", CollectionSourceMode.Normal);
                        collectionSource.Add(newObject);
                    }
                }
            }
        }

        #endregion

    }



}