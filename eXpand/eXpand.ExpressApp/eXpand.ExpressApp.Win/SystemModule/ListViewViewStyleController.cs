using System;
using System.IO;
using System.Text;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Layout;
using DevExpress.XtraGrid.Views.Layout.Events;

namespace eXpand.ExpressApp.Win.SystemModule
{
    public partial class ListViewViewStyleController : ViewController
    {
        private const string gridLayoutViewLayoutAttribute = @"GridLayoutViewLayout";
        private const string viewStyleAttribute = @"ViewStyle";
        private const string LayoutViewCustomizationAttributeName = "LayoutViewCustomization";
        private GridView gridView;

        public ListViewViewStyleController()
        {
            InitializeComponent();
            RegisterActions(components);
        }

        public string SelectedViewStyle { get; private set; }

        private string LayoutFile
        {
            get { return string.Format("{0}.{1}", View.Id, "xml"); }
        }

        public override Schema GetSchema()
        {
            const string s =
                @"<Element Name=""Application"">;
                            <Element Name=""Views"">
                                <Element Name=""ListView"">;
                                    <Attribute Name=""" +
                gridLayoutViewLayoutAttribute +
                @""" IsInvisible=""True""/>
                <Attribute Name=""" +
                    viewStyleAttribute +
                @""" Choice=""GridView,LayoutView""/>
                                </Element>
                            </Element>
                    </Element>";

            return new Schema(new DictionaryXmlReader().ReadFromString(s));
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            SelectedViewStyle = View.Info.GetAttributeValue(viewStyleAttribute);
            if (string.IsNullOrEmpty(SelectedViewStyle))
            {
                SelectedViewStyle = "GridView";
            }
            if (SelectedViewStyle == "GridView")
            {
                return;
            }
            View.ControlsCreated += OnGridViewControlsCreated;
        }

        private void OnGridViewControlsCreated(object sender, EventArgs e){
            var listview = ((ListView) View);
            var gridListEditor = ((GridListEditor) listview.Editor);
            gridView = gridListEditor.GridView;

            var riPictureEdit = ((RepositoryItemPictureEdit) gridView.GridControl.RepositoryItems.Add("PictureEdit"));
            riPictureEdit.SizeMode = PictureSizeMode.Squeeze;

            LayoutView layoutView = new LayoutView(gridView.GridControl);
            string layoutString = Info.GetAttributeValue(LayoutViewCustomizationAttributeName, string.Empty);
            if (!string.IsNullOrEmpty(layoutString)) {
                using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(layoutString)))
                {
                    gridView.GridControl.MainView.RestoreLayoutFromStream(stream);	
                }
            }
            gridView.GridControl.MainView = layoutView;

            ((LayoutView) gridView.GridControl.MainView).CustomRowCellEdit += CustomRowCellEdit;
            gridView.GridControl.MainView.DataSourceChanged += MainView_DataSourceChanged;

        }

        private void CustomRowCellEdit(object sender, LayoutViewCustomRowCellEditEventArgs e) {
            if (e.Column.FieldName == "Photo") {
                RepositoryItem editor = e.RepositoryItem = new RepositoryItemPictureEdit();
                editor.ReadOnly = true;
                e.RepositoryItem = editor;
            }
        }


        private void MainView_DataSourceChanged(object sender, EventArgs e) {
            //if (File.Exists(LayoutFile)) {
            //    //gridView.GridControl.MainView.RestoreLayoutFromXml(LayoutFile);

            //    //((LayoutView) gridView.GridControl.MainView).Columns.ColumnByFieldName("Photo").ColumnEdit =
            //    //    gridView.GridControl.RepositoryItems["PictureEdit"];
            //}
        }

        protected override void OnDeactivating() {
            if (SelectedViewStyle == "LayoutView") {
                using (Stream layoutStream = new MemoryStream()){
                    gridView.GridControl.MainView.SaveLayoutToStream(layoutStream, OptionsLayoutBase.FullLayout);
                    StreamReader reader = new StreamReader(layoutStream);
                    string layoutString = reader.ReadToEnd();
                    Info.SetAttribute(LayoutViewCustomizationAttributeName, layoutString);
                }
            }
            base.OnDeactivating();
        }
    }
}