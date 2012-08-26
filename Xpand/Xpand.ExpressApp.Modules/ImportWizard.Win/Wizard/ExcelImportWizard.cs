using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using DevExpress.Data.Filtering;
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

    public partial class ExcelImportWizard : XtraForm {
        #region Initialization

        public ExcelImportWizard(XPObjectSpace objectSpace, ITypeInfo typeInfo, CollectionSourceBase collectionSourceBase) {
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

            //var col = gridLookUpEdit2View.Columns.ColumnByFieldName("Mapped");
            //if (col != null) {
            //    col.Visible = false;
            //    col.FilterInfo =
            //        new ColumnFilterInfo(new BinaryOperator("Mapped", false));

            //}

            var mappablePropertyClassInfo
                = objectSpace.Session.GetClassInfo(typeof(MappableProperty));


            foreach (GridColumn column in gridLookUpEdit2View.Columns) {

                column.Caption = mappablePropertyClassInfo
                                    .GetMember(column.FieldName).DisplayName;
                if (column.FieldName == "Mapped") {
                    column.Visible = false;
                }
            }

            //col = gridLookUpEdit2View.Columns.ColumnByFieldName("Oid");
            //if (col != null)
            //    col.Visible = false;

        }


        /// <summary>
        /// Fill MRU item list with values
        /// </summary>
        private void ExcelImportWizard_Load(object sender, EventArgs e) {
            _Mus = new MyUserSettings();
            if (_Mus.MRUItems != null)
                FileSelectEdit.Properties.Items.AddRange(_Mus.MRUItems);
        }
        /// <summary>
        /// Save MRU item list with values
        /// </summary>
        private void ExcelImportWizard_FormClosing(object sender, FormClosingEventArgs e) {
            if (_Mus.MRUItems == null)
                _Mus.MRUItems = new List<string>();
            else
                _Mus.MRUItems.Clear();
            foreach (var item in FileSelectEdit.Properties.Items) {
                _Mus.MRUItems.Add(item.ToString());
            }

            _Mus.Save();

            if (ObjectSpace.Session.InTransaction)
                ObjectSpace.Session.RollbackTransaction();

            if (ExcelDocument != null)
                ExcelDocument.Close();
        }

        #endregion

        private MyUserSettings _Mus;
        public XPObjectSpace ObjectSpace { get; private set; }
        public CollectionSourceBase CurrentCollectionSource { get; private set; }

        private SpreadsheetDocument ExcelDocument { get; set; }
        private Sheet _Sheet;
        public Sheet Sheet {
            get { return _Sheet; }
            private set {
                _Sheet = value;
                AssignDataSource();

            }
        }

        private ImportMap _ImportMap;
        public ImportMap ImportMap {
            get { return _ImportMap; }
            set {
                _ImportMap = value;
                AssingMapping(value);
            }
        }

        private Type _Type;
        public Type Type {
            get { return _Type; }
            set {
                _Type = value;

                if (_Type == null) return;
                var props = ObjectSpace.Session.GetClassInfo(_Type)
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
            ExcelSheetPreviewGrid.DataSource = _Sheet == null ? null : _Sheet.DataPreviewTable();
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
            //MappeablePropertiesCollection.AddRange(MappableColumns.ToList());


            repositoryItemGridLookUpEdit.DataSource = MappableColumns;//.ToList();
            repositoryItemGridLookUpEdit.DisplayMember = "Name";
            repositoryItemGridLookUpEdit.ValueMember = "Name";
            // repositoryItemGridLookUpEdit1View. = MappableColumns;
            //repositoryItemGridLookUpEdit1View.DisplayMember = "Name";
            //repositoryItemGridLookUpEdit1View.ValueMember = "Name";



            ImportMapLookUp.Properties.DataSource = ImportMapsCollection.ToList();
            ImportMapLookUp.EditValue = null;

            //check for duplicate columns and throw error if found
            if (Sheet.Columns()
                .GroupBy(d => d.Name)
                .Select(g => new { g.Key, count = g.Count() }).Any(g => g.count > 1))
                throw new InvalidDataException(Resources.ExcelImportWizard_InitWizPage1_Duplicate_column_names_found__please_fix_excel_column_names);

            //Assign data for Mapping Grid
            var dt = _Sheet.DataPreviewTable().Transpose();

            dt.RowChanged -= dt_RowChanged;
            var mappingGirdView = ((BandedGridView)MappingGrid.MainView);

            mappingGirdView.Columns.Clear();
            mappingGirdView.BestFitColumns();
            MappingGrid.DataSource = dt;

            gridControl2_DataSourceChanged();
            //Register to Mapping data table change events
            //this will be used to synch grid data with Import map object
            dt.RowChanged += dt_RowChanged;
            dt_RowChanged(dt, null);


        }

        /// <summary>
        /// Tracks changes in the mapping data table
        /// Synchronizes them to the Import map object
        /// Takes care of some interface actions
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void dt_RowChanged(object sender, DataRowChangeEventArgs e) {
            //when saved map is selected and something changes in the grid
            //changes radio selection to a custom map
            if (!_IgnoreDtChanges && MappingRadioGroup.SelectedIndex != 1)
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
        private void gridControl2_DataSourceChanged() {
            //Banded Grid Control that prepared data for mapping

            var bands = ((BandedGridView)MappingGrid.MainView).Bands;
            var bandedGridView = MappingGrid.MainView as BandedGridView;

            if (bandedGridView == null) return;
            var cols = bandedGridView.Columns;

            foreach (BandedGridColumn col in cols) {
                if (col.AbsoluteIndex != 0 && col.AbsoluteIndex != cols.Count - 1) {
                    //move value columns to Values band
                    col.OwnerBand = bands["Values"];
                }
                if (col.AbsoluteIndex == cols.Count - 1) {
                    //Moves the LAST !!! column to Mapping band
                    col.OwnerBand = bands["MapTo"];
                    col.ColumnEdit = repositoryItemGridLookUpEdit;
                    col.Width = 100;
                }

            }
        }

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
            } else {
                try {
                    ExcelDocument = SpreadsheetDocument.Open(edit.Text, false);

                    var sheets = ExcelDocument.Sheets()
                        .Select(p => p.OXmlSheet.Name)
                        .ToList();
                    SheetSelectEdit.Properties.DataSource = sheets;
                    if (sheets.Count == 1)
                        SheetSelectEdit.EditValue = sheets.FirstOrDefault();

                    ImportMapDescriptionEdit.Text = (new FileInfo(edit.Text)).Name;
                } catch {
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
            _Sheet.ColumnHeaderRow = ((CheckEdit)sender).Checked
                                         ?
                                            decimal.ToInt32(HeaderRowSpinEdit.Value)
                                         : (int?)null;
            _Sheet.PreviewRowCount = ((CheckEdit)sender).Checked
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
            _Sheet.ColumnHeaderRow = decimal.ToInt32(((SpinEdit)sender).Value);
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
            } else {
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
            _Sheet.PreviewRowCount = decimal.ToInt32(((SpinEdit)sender).Value);
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
                    } else ImportMap = new ImportMap(ObjectSpace.Session);

                    break;
                default:
                    throw new NotImplementedException();
            }


        }

        #region Mapping Procedures

        private bool _IgnoreDtChanges;
        private void AssingMapping(ImportMap importMap) {
            _IgnoreDtChanges = true;
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
            _IgnoreDtChanges = false;
        }

        private void GuesMappings() {
            var dt = MappingGrid.DataSource as DataTable;
            if (dt == null || ImportMap == null) return;

            var rows = dt.Rows;

            //ImportMap.Changed += ImportMap_Changed;


            for (var i = 0; i < rows.Count; i++) {
                var dataRow = rows[i];
                var mapsTo =
                    (object)MappableColumns
                                .Where(p => p.Name.ToUpper().Replace(" ", "").Replace("_", "")
                                    == dataRow.ItemArray.First().ToString().ToUpper().Replace(" ", "").Replace("_", "") ||
                                    p.DisplayName.ToUpper().Replace(" ", "").Replace("_", "")
                                    == dataRow.ItemArray.First().ToString().ToUpper().Replace(" ", "").Replace("_", ""))
                                .Select(p => p.Name)
                                .FirstOrDefault()
                    ;

                dt.Rows[i][dataRow.ItemArray.Length - 1] = mapsTo != null ? mapsTo.ToString() : null;

            }

        }


        #endregion

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


        #region Import Data

        private BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;


        private void ImportButton_Click(object sender, EventArgs e) {

            var rowCount = Sheet.Rows().Count();
            if (Sheet.ColumnHeaderRow != null)
                rowCount = (int)(rowCount - Sheet.ColumnHeaderRow);

            _FrmProgress = new ProgressForm(Resources.ExcelImportWizard_ImportButton_Click_Import_excell_rows_progress___, rowCount, "Processing record {0} of {1} ");
            _FrmProgress.CancelClick += FrmProgressCancelClick;

            _BgWorker = new BackgroundWorker {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };

            _BgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
            _BgWorker.ProgressChanged += BgWorkerProgressChanged;
            _BgWorker.DoWork += BgWorkerDoWork;

            _BgWorker.RunWorkerAsync(Sheet.Rows());

            _FrmProgress.ShowDialog();


        }

        #region Progress Events
        private void BgWorkerProgressChanged(object sender, ProgressChangedEventArgs e) {


            if (_FrmProgress != null)
                _FrmProgress.DoProgress(e.ProgressPercentage);

            SetMemoText(e.UserState.ToString());

        }

        private delegate void SetMemoTextDelegate(string text);

        public void SetMemoText(string text) {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (ResultsMemoEdit.InvokeRequired) {
                var d = new SetMemoTextDelegate(SetMemoText);
                Invoke(d, new object[] { text });
            } else {
                ResultsMemoEdit.Text += Environment.NewLine + text;
                ResultsMemoEdit.Select(ResultsMemoEdit.Text.Length,
                                       ResultsMemoEdit.Text.Length);
                ResultsMemoEdit.ScrollToCaret();
            }
        }

        private void FrmProgressCancelClick(object sender, EventArgs e) {
            _BgWorker.CancelAsync();
        }

        private void BgWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {

            _FrmProgress.Close();

            if (e.Cancelled) {
                ObjectSpace.Rollback();
                XtraMessageBox.Show(Resources.ExcelImportWizard_BgWorkerRunWorkerCompleted_The_task_has_been_cancelled, Resources.ExcelImportWizard_BgWorkerRunWorkerCompleted_Work_Canceled, MessageBoxButtons.OK,
                                    MessageBoxIcon.Exclamation);
            } else if (e.Error != null) {
                ObjectSpace.Rollback();
                XtraMessageBox.Show(Resources.ExcelImportWizard_BgWorkerRunWorkerCompleted_Error__Details__ + e.Error, Resources.ExcelImportWizard_BgWorkerRunWorkerCompleted_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            } else {
                ObjectSpace.CommitChanges();
                XtraMessageBox.Show(Resources.ExcelImportWizard_BgWorkerRunWorkerCompleted_The_task_has_been_completed__Results__ + e.Result);
            }

            WizardControl.SelectedPage = completionWizardPage1;
        }

        #endregion

        private void BgWorkerDoWork(object sender, DoWorkEventArgs e) {
            ProccesExcellRows((IEnumerable<Row>)e.Argument, ObjectSpace, e);
        }

        private static Object GetCollectionOwner(Object masterObject, IMemberInfo memberInfo) {
            return memberInfo.GetOwnerInstance(masterObject);
        }


        private static void AddNewObjectToCollectionSource(CollectionSourceBase currentCollectionSource, object newObject, XPObjectSpace objectSpace) {
            var newObjectTypeInfo = XafTypesInfo.Instance.FindTypeInfo(newObject.GetType());
            if ((currentCollectionSource != null) && currentCollectionSource.ObjectTypeInfo.IsAssignableFrom(newObjectTypeInfo)) {
                if (objectSpace == currentCollectionSource.ObjectSpace) {
                    currentCollectionSource.Add(newObject);
                } else {
                    var propertyCollectionSource = (currentCollectionSource as PropertyCollectionSource);
                    if ((propertyCollectionSource != null) && (propertyCollectionSource.MasterObject != null)) {
                        Object collectionOwner;
                        if (propertyCollectionSource.MemberInfo.GetPath().Count > 1) {
                            collectionOwner = GetCollectionOwner(propertyCollectionSource.MasterObject, propertyCollectionSource.MemberInfo);
                            if (collectionOwner != null) {
                                XafTypesInfo.Instance.FindTypeInfo(collectionOwner.GetType()).FindMember(propertyCollectionSource.MemberInfo.LastMember.Name);
                            }
                        } else {
                            collectionOwner = propertyCollectionSource.MasterObject;
                        }
                        if ((collectionOwner != null)
                                && XafTypesInfo.Instance.FindTypeInfo(collectionOwner.GetType()).IsPersistent) {
                            //TODO: Fix this
                            throw new NotImplementedException("Feature not completely implemented. Import win Create Collection Source");
                            //var collectionSource = Application.CreatePropertyCollectionSource(
                            //    objectSpace, null, objectSpace.GetObject(collectionOwner), memberInfo, "", CollectionSourceMode.Normal);
                            //collectionSource.Add(newObject);
                        }
                    }
                }
            }
        }

        public void ProccesExcellRows(IEnumerable records, XPObjectSpace objectSpace, DoWorkEventArgs e) {

            var i = 0;

            //for every row in excel sheet
            foreach (Row record in records) {
                ++i;
                if (i == 1) continue;
                if (_BgWorker.CancellationPending) { e.Cancel = true; break; }

                //var os = new ObjectSpace(objectSpace, XafTypesInfo.Instance);
                object newObj = null;

                //check if row contains Oid

                //get key property name of the object type being imported
                var kp = objectSpace.GetKeyPropertyName(Type);
                //check if it exists in excel and is mapped ?
                var idMapping = ImportMap.Mappings.FirstOrDefault(p => p.MapedTo == kp);
                if (idMapping != null && GetQString(record[idMapping.Column].Value) != string.Empty) {
                    try {
                        //find existing object
                        var val = record[idMapping.Column];
                        var gwid = new Guid(GetQString(val.Value));
                        newObj = objectSpace.FindObject(Type, new BinaryOperator(kp, gwid), true);
                    } catch {

                    }
                }
                if (newObj == null) //create a new instance
                    newObj = objectSpace.CreateObject(Type) as IXPSimpleObject;

                string message;
                if (newObj != null) {
                    var props = ((IXPSimpleObject)newObj).ClassInfo.PersistentProperties
                        .OfType<XPMemberInfo>().ToList();


                    foreach (var mapping in ImportMap.Mappings) {
                        if (_BgWorker.CancellationPending) { e.Cancel = true; break; }
                        Application.DoEvents();

                        var mapping1 = mapping;
                        var prop = props.FirstOrDefault(p => p.Name == mapping1.MapedTo);

                        try {
                            var val = record[mapping.Column];
                            // continue;

                            if (val != null) {
                                //if simple property
                                if (prop.ReferenceType == null && !prop.MemberType.IsEnum) {
                                    var isNullable = prop.MemberType.IsGenericType && prop.MemberType.GetGenericTypeDefinition() == typeof(Nullable<>);
                                    object convertedValue = null;

                                    if (prop.MemberType == null) return;

                                    if (isNullable) {
                                        if (prop.StorageType == typeof(int)) {
                                            int number;
                                            if (val.Value != String.Empty && Int32.TryParse(val.Value, out number))
                                                convertedValue = number;

                                        } else if (prop.StorageType == typeof(DateTime)) {
                                            if (val.Value != string.Empty) {
                                                //Include validate
                                                var dt = DateTime.FromOADate(Convert.ToDouble(val.Value));
                                                convertedValue = dt;
                                            }
                                        } else if (prop.StorageType == typeof(double)) {
                                            double number;
                                            var rez = Double.TryParse(val.Value, out number);
                                            if (rez) convertedValue = number;
                                        }
                                    } else {
                                        if (prop.MemberType.IsEnum) {
                                            prop.SetValue(newObj, Enum.Parse(prop.MemberType, val.Value));
                                        } else if (prop.MemberType == typeof(char))
                                            convertedValue = Convert.ChangeType(GetQString(val.Value), prop.MemberType);
                                        else if (prop.StorageType == typeof(int)) {
                                            int number;
                                            if (val.Value != String.Empty && Int32.TryParse(val.Value, out number))
                                                convertedValue = number;
                                            else
                                                convertedValue = 0;
                                        } else if (prop.MemberType == typeof(Guid))
                                            convertedValue = new Guid(GetQString(val.Value));
                                        else if (prop.StorageType == typeof(DateTime)) {
                                            if (val.Value != string.Empty) {
                                                //Include validate
                                                var dt = DateTime.FromOADate(Convert.ToDouble(val.Value));
                                                convertedValue = dt;
                                            }
                                        } else if (prop.MemberType == typeof(double)) {
                                            double number;
                                            //  Application.CurrentCulture.NumberFormat.NumberDecimalSeparator = ".";
                                            var rez = Double.TryParse(val.Value, NumberStyles.Number, new NumberFormatInfo { NumberDecimalSeparator = "." }, out number);
                                            if (rez) convertedValue = number;
                                        } else if (prop.MemberType == typeof(bool)) {
                                            if (val.Value != string.Empty && (val.Value.Length == 1 || val.Value.ToLower() == "true" || val.Value.ToLower() == "false")) {
                                                bool truefalse;
                                                if (val.Value.ToLower() == "true" || val.Value.ToLower() == "false")
                                                    truefalse = Convert.ToBoolean(val.Value);
                                                else
                                                    truefalse = Convert.ToBoolean(Convert.ToInt32(val.Value));
                                                convertedValue = truefalse;
                                            }
                                        } else
                                            convertedValue = Convert.ChangeType(val.Value, prop.MemberType);
                                    }

                                    if (convertedValue != null) {
                                        if (convertedValue is double)
                                            convertedValue = Math.Round((double)convertedValue, 2, MidpointRounding.ToEven);

                                        prop.SetValue(newObj, convertedValue);
                                    }
                                }
                                //if referenced property
                                if (prop.ReferenceType != null) {

                                    //if other referenced type
                                    if (prop.MemberType.IsSubclassOf(typeof(XPBaseObject))) {
                                        var text = val.Value;
                                        var typ = prop.MemberType;
                                        var mval = ImportWizard.Helper.GetXpObjectByKeyValue(objectSpace, text, typ);
                                        prop.SetValue(newObj, objectSpace.GetObject(mval));
                                    }
                                }

                            }

                        } catch (Exception ee) {
                            message = string.Format(Resources.ExcelImportWizard_ProccesExcellRows_Error_processing_record__0____1_, i - 1, ee);
                            _BgWorker.ReportProgress(0, message);
                        }

                        if (CurrentCollectionSource != null)
                            AddNewObjectToCollectionSource(CurrentCollectionSource, newObj, ObjectSpace);
                        ObjectSpace.Session.Save(newObj);
                    }
                }

                objectSpace.CommitChanges();
                message = string.Format(Resources.ExcelImportWizard_ProccesExcellRows_Importing_record__0__succesfull_, i - 1);
                _BgWorker.ReportProgress(1, message);
                Application.DoEvents();
            }
        }

        #endregion

        private void radioGroup2_EditValueChanged(object sender, EventArgs e) {
            ImportMapDescriptionEdit.Enabled = (bool)radioGroup2.EditValue;
        }

        public string GetQString(string oid) {
            try {
                var s1 = oid.IndexOf("'", StringComparison.Ordinal);
                var s2 = oid.IndexOf("'", s1 + 1, StringComparison.Ordinal);
                var s3 = oid.Substring(s1 + 1, s2 - 1);
                return s3;
            } catch (Exception) {

                return string.Empty;
            }

        }

        private void mappablePropertyBindingSource_CurrentChanged(object sender, EventArgs e) {

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




    }



}