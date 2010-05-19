using System.ComponentModel;
using System.IO;
using System.Text;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.Utils;
using DevExpress.XtraGrid.Views.Layout;
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
        [Category("eXpand")]
        ListViewStyle ListViewStyle { get; set; }
        [Category("eXpand")]
        string LayoutViewCustomization { get; set; }
    }

    public class ListViewViewStyleController : ViewController<ListView>, IModelExtender
    {
        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders)
        {
            extenders.Add<IModelListView, IModelListViewViewStyle>();
        }

        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            if (((IModelListViewViewStyle)View.Model).ListViewStyle == ListViewStyle.LayoutView && View.Editor is GridListEditor)
            {
                var gridView = ((GridListEditor)View.Editor).GridView;

                var layoutView = new LayoutView(gridView.GridControl);

                if (!string.IsNullOrEmpty(((IModelListViewViewStyle)View.Model).LayoutViewCustomization))
                {
                    using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(((IModelListViewViewStyle)View.Model).LayoutViewCustomization)))
                    {
                        gridView.GridControl.MainView.RestoreLayoutFromStream(stream);
                    }
                }

                gridView.GridControl.MainView = layoutView;
            }
        }

        protected override void OnDeactivating()
        {
            if (((IModelListViewViewStyle)View.Model).ListViewStyle == ListViewStyle.LayoutView && View.Editor is GridListEditor)
            {
                using (Stream layoutStream = new MemoryStream())
                {
                    ((GridListEditor)View.Editor).Grid.MainView.SaveLayoutToStream(layoutStream, OptionsLayoutBase.FullLayout);
                    var reader = new StreamReader(layoutStream);
                    string layoutString = reader.ReadToEnd();
                    ((IModelListViewViewStyle)View.Model).LayoutViewCustomization = layoutString;
                }
            }

            base.OnDeactivating();
        }
    }
}