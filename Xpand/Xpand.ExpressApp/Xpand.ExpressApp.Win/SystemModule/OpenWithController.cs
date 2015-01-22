using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Base;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.Model;
using ListView = DevExpress.ExpressApp.ListView;

namespace Xpand.ExpressApp.Win.SystemModule {
    [ModelAbstractClass]
    public interface IModelCommonMemberViewItemOpenWith {
        [Category("eXpand")]
        bool OpenWithAssociatedProgram { get; set; }
        [Category("eXpand")]
        bool OpenPathWithExplorer { get; set; }
    }

    public class OpenWithController : ViewController<ObjectView>, IModelExtender {
        protected override void OnActivated() {
            base.OnActivated();
            var detailView = View as DetailView;
            if (detailView != null) {
                OpenWith(detailView);
            }

        }

        private void OpenWith(DetailView detailView) {
            var propertyEditors = detailView.GetItems<PropertyEditor>();
            foreach (var propertyEditor in propertyEditors) {
                propertyEditor.ControlCreated += PropertyEditorOnControlCreated;
            }
        }

        private void PropertyEditorOnControlCreated(object sender, EventArgs eventArgs) {
            var editor = (PropertyEditor)sender;
            ((Control)editor.Control).KeyUp += (ender, args) => {
                if (args.KeyCode == Keys.Enter && !string.IsNullOrWhiteSpace(editor.PropertyValue+"")) {
                    var modelCommonMemberViewItemOpenWith = ((IModelCommonMemberViewItemOpenWith)editor.Model);
                    OpenWith(modelCommonMemberViewItemOpenWith, editor.PropertyValue.ToString());
                }
            };
        }
        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            var listView = View as ListView;
            if (listView != null) {
                OpenWith(listView);
            }
        }

        private void OpenWith(ListView listView) {
            var gridControl = listView.Editor.Control as GridControl;
            if (gridControl != null) {
                gridControl.KeyUp += GridControlOnKeyUp;
                gridControl.MouseDoubleClick += GridControlOnMouseDoubleClick;
            }
        }

        private void GridControlOnMouseDoubleClick(object sender, MouseEventArgs mouseEventArgs) {
            OpenWith((GridControl)(sender));
        }

        private void GridControlOnKeyUp(object sender, KeyEventArgs keyEventArgs) {
            if (keyEventArgs.KeyCode == Keys.Enter) {
                var gridControl = ((GridControl)sender);
                OpenWith(gridControl);
            }
        }

        private void OpenWith(GridControl gridControl){
            var focusedView = gridControl.FocusedView as ColumnView;
            if (focusedView != null && focusedView.FocusedColumn != null){
                var path = focusedView.GetRowCellDisplayText(focusedView.FocusedRowHandle, focusedView.FocusedColumn);
                var modelCommonMemberViewItemOpenWith =
                    ((IModelCommonMemberViewItemOpenWith) focusedView.FocusedColumn.Model());
                OpenWith(modelCommonMemberViewItemOpenWith, path);
            }
        }

        private void OpenWith(IModelCommonMemberViewItemOpenWith modelCommonMemberViewItemOpenWith, string path) {
            if (modelCommonMemberViewItemOpenWith.OpenWithAssociatedProgram) {
                Process.Start(path);
            }
            if (modelCommonMemberViewItemOpenWith.OpenPathWithExplorer) {
                var directoryName = Path.GetDirectoryName(path) + "";
                Process.Start(directoryName);
            }
        }

        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelCommonMemberViewItem, IModelCommonMemberViewItemOpenWith>();
        }
    }
}
