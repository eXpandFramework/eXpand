using System;
using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Design;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Design;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.Design {
    public interface IColumnViewEditor : ISupportFooter {
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        bool OverrideViewDesignMode { get; set; }
        GridControl Grid { get; }
        object DataSource { get; set; }
        IModelListView Model { get; }
        DevExpress.XtraGrid.Views.Base.ColumnView ColumnView { get; }
        CollectionSourceBase CollectionSource { get; }
        RepositoryEditorsFactory RepositoryFactory { get; set; }
        void SaveModel();
        void Setup(CollectionSourceBase collectionSource, XafApplication xafApplication);
        event EventHandler<CreateCustomModelSynchronizerEventArgs> CreateCustomModelSynchronizer;
        event EventHandler<ColumnCreatedEventArgs> ColumnCreated;
        event EventHandler<CustomGridViewCreateEventArgs> CustomGridViewCreate;
        object CreateControls();
        bool IsAsyncServerMode();
        ColumnWrapper AddColumn(IModelColumn columnInfo);
        void RemoveColumn(ColumnWrapper xafGridColumnWrapper);
        event EventHandler GridDataSourceChanging;
    }
    public class CustomGridViewCreateEventArgs : HandledEventArgs {
        public CustomGridViewCreateEventArgs(GridControl gridControl) {
            GridControl = gridControl;
        }

        public DevExpress.XtraGrid.Views.Base.ColumnView GridView { get; set; }
        public GridControl GridControl { get; private set; }
    }

    public abstract class ColumnViewDesigner : BaseDesigner {
        [ThreadStatic]
        static ImageCollection largeImages;
        [ThreadStatic]
        static ImageCollection smallImages;
        static ImageCollection LargeImages {
            get {
                return largeImages ?? (largeImages = ImageHelper.CreateImageCollectionFromResources("DevExpress.XtraGrid.Images.icons32x32.png", typeof(BaseGridDesigner).Assembly, new Size(32, 32)));
            }
        }
        static ImageCollection SmallImages {
            get {
                return smallImages ?? (smallImages = ImageHelper.CreateImageCollectionFromResources("DevExpress.XtraGrid.Images.icons16x16.png", typeof(BaseGridDesigner).Assembly, new Size(16, 16)));
            }
        }
        protected override object LargeImageList { get { return LargeImages; } }
        protected override object SmallImageList { get { return SmallImages; } }

        protected override void CreateGroups() {
            Groups.Clear();
            //            DesignerGroup group = Groups.Add(DesignerGroupType.Main.ToString(), "Main Grid settings (adjust the view, columns, bands, and specify in-place editors and summaries).", null, true);
            //            var designerType = Type.GetType("DevExpress.XtraGrid.FeatureBrowser.FeatureBrowserGridMainFrame, DevExpress.XtraGrid" + XafApplication.CurrentVersion + ".Design");
            //            if (designerType != null)
            //                group.Add("Feature Browser", "Locate relevant options by features.", designerType, GetDefaultLargeImage(3), GetDefaultSmallImage(3), null);

            //            designerType = Type.GetType("DevExpress.XtraGrid.Frames.ViewsEditor, DevExpress.XtraGrid" + XafApplication.CurrentVersion + ".Design");
            //            if (designerType != null)
            //                group.Add("View Repository", "Manage views.", designerType, GetDefaultLargeImage(6), GetDefaultSmallImage(6), true);

            //            designerType = Type.GetType("DevExpress.XtraGrid.Frames.ColumnDesigner, DevExpress.XtraGrid" + XafApplication.CurrentVersion + ".Design");
            //            if (designerType != null)
            //                group.Add("Columns", "Adjust the Column collection of the current view, assign in-place editors to columns and specify total summaries.", designerType, GetDefaultLargeImage(1), GetDefaultSmallImage(1), null);

            //            designerType = Type.GetType("DevExpress.XtraGrid.Frames.PersistentRepositoryGridEditor, DevExpress.XtraGrid" + XafApplication.CurrentVersion + ".Design");
            //            if (designerType != null)
            //                group.Add("In-place Editor Repository", "Adjust the editors used for in-place editing.", designerType, GetDefaultLargeImage(7), GetDefaultSmallImage(7), true);


        }

    }
}