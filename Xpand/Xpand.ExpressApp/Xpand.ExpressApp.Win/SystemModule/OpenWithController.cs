using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Base;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.Model;
using ListView = DevExpress.ExpressApp.ListView;

namespace Xpand.ExpressApp.Win.SystemModule {
    [ModelAbstractClass]
    public interface IModelColumnOpenWith {
        [Category("eXpand")]
        bool OpenWithAssociatedProgram { get; set; }
        [Category("eXpand")]
        bool OpenPathWithExplorer { get; set; }
    }

    public class OpenWithController : ViewController<ListView>, IModelExtender {
        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            var gridControl = View.Editor.Control as GridControl;
            if (gridControl != null) gridControl.KeyDown += GridControlOnKeyDown;
        }

        private void GridControlOnKeyDown(object sender, KeyEventArgs keyEventArgs) {
            if (keyEventArgs.KeyCode == Keys.Space) {
                var focusedView = ((GridControl)sender).FocusedView as ColumnView;
                if (focusedView != null) {
                    var modelColumnOpenInExplorer = ((IModelColumnOpenWith)focusedView.FocusedColumn.Model());
                    var path = focusedView.GetRowCellDisplayText(focusedView.FocusedRowHandle, focusedView.FocusedColumn);
                    if (modelColumnOpenInExplorer.OpenWithAssociatedProgram) {
                        Process.Start(path);
                    }
                    if (modelColumnOpenInExplorer.OpenPathWithExplorer) {
                        var directoryName = Path.GetDirectoryName(path) + "";
                        Process.Start(directoryName);
                    }
                }
            }
        }

        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelColumn, IModelColumnOpenWith>();
        }
    }
}
