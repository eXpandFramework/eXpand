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
using DevExpress.ExpressApp.Model;

namespace eXpand.ExpressApp.Win.SystemModule
{
    public enum ListViewStyle
    {
        GridView,
        LayoutView
    }

    public interface IModelListViewViewStyle : IModelNode
    {
        ListViewStyle ListViewStyle { get; set; }
        string LayoutViewCustomization { get; set; }
    }

    public partial class ListViewViewStyleController : ViewController<ListView>, IModelExtender
    {
        public ListViewViewStyleController() {}

        private string LayoutFile
        {
            get { return string.Format("{0}.{1}", View.Id, "xml"); }
        }

        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders)
        {
            extenders.Add<IModelListView, IModelListViewViewStyle>();
        }

        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            if (((IModelListViewViewStyle)this.View.Model).ListViewStyle == ListViewStyle.LayoutView && View.Editor is GridListEditor)
            {
                var gridView = ((GridListEditor)this.View.Editor).GridView;

                LayoutView layoutView = new LayoutView(gridView.GridControl);
                
                if (!string.IsNullOrEmpty(((IModelListViewViewStyle)this.View.Model).LayoutViewCustomization))
                {
                    using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(((IModelListViewViewStyle)this.View.Model).LayoutViewCustomization)))
                    {
                        gridView.GridControl.MainView.RestoreLayoutFromStream(stream);
                    }
                }

                gridView.GridControl.MainView = layoutView;
            }
        }

        protected override void OnDeactivating() {
            if (((IModelListViewViewStyle)this.View.Model).ListViewStyle == ListViewStyle.LayoutView && View.Editor is GridListEditor)
            {
                using (Stream layoutStream = new MemoryStream()){
                    ((GridListEditor)View.Editor).Grid.MainView.SaveLayoutToStream(layoutStream, OptionsLayoutBase.FullLayout);
                    StreamReader reader = new StreamReader(layoutStream);
                    string layoutString = reader.ReadToEnd();
                    ((IModelListViewViewStyle)this.View.Model).LayoutViewCustomization = layoutString;
                }
            }

            base.OnDeactivating();
        }
    }
}