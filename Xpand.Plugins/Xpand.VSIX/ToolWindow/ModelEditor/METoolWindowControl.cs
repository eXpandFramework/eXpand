using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Forms;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using EnvDTE80;
using Xpand.VSIX.Extensions;

namespace Xpand.VSIX.ToolWindow.ModelEditor {
    public partial class METoolWindowControl : UserControl {
        private readonly DTE2 _dte = DteExtensions.DTE;

        public METoolWindowControl() {
            InitializeComponent();
            GridHelper.Init(gridControl1).Subscribe();
            gridView1.KeyDown+=GridView1OnKeyDown;
            Observable.FromEventPattern<KeyEventHandler, KeyEventArgs>(h => gridView1.KeyUp += h, h => gridView1.KeyUp -= h)
                .Where(p => p.EventArgs.KeyCode == Keys.Return&& gridView1.GetRow(gridView1.FocusedRowHandle) is ProjectItemWrapper)
                .SelectMany(pattern => OpenModelEditor())
                .Subscribe();
            Observable.FromEventPattern<EventHandler, EventArgs>(h => gridView1.DoubleClick += h, h => gridView1.DoubleClick -= h)
                .SelectMany(pattern => OpenModelEditor())
                .Subscribe();
        }

        private void GridView1OnKeyDown(object sender, KeyEventArgs e){
            var gridView = ((GridView)gridControl1.MainView);
            if (gridView.FocusedRowHandle == 0 && e.KeyCode == Keys.Up)
                gridView.FocusedRowHandle = GridControl.AutoFilterRowHandle;
        }

        private IObservable<Unit> OpenModelEditor() {
            var projectItemWrapper = (ProjectItemWrapper) gridView1.GetRow(gridView1.FocusedRowHandle);
            _dte.InitOutputCalls("OpenModelEditor");
            return new ModelEditorRunner().Start(projectItemWrapper)
                .Catch<Unit,Exception>(e => {
                    _dte.WriteToOutput(e.ToString());
                    return Observable.Empty<Unit>();
                });
        }



    }
}
