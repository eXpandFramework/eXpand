using System;
using System.Windows.Forms;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using EnvDTE;
using EnvDTE80;
using Xpand.VSIX.Extensions;

namespace Xpand.VSIX.ModelEditor {
    public partial class METoolWindowControl : UserControl {
        private readonly DTE2 _dte = DteExtensions.DTE;

        public METoolWindowControl() {
            InitializeComponent();
            GridHelper.Init(gridControl1);
            gridView1.KeyDown+=GridView1OnKeyDown;
            gridView1.KeyUp+=gridView1_KeyUp;
            gridView1.DoubleClick+=gridView1_DoubleClick;
        }


        private void GridView1OnKeyDown(object sender, KeyEventArgs e){
            var gridView = ((GridView)gridControl1.MainView);
            if (gridView.FocusedRowHandle == 0 && e.KeyCode == Keys.Up)
                gridView.FocusedRowHandle = GridControl.AutoFilterRowHandle;
        }

        private void OpenModelEditor(ProjectItemWrapper projectItemWrapper) {
            _dte.InitOutputCalls("OpenModelEditor");
            try{
                new ModelEditorRunner().Start(projectItemWrapper);
            }
            catch (Exception e){
                _dte.WriteToOutput(e.ToString());
            }
        }

        private void gridView1_KeyUp(object sender, KeyEventArgs e) {
            var gridView = ((GridView)gridControl1.MainView);
            if (e.KeyCode == Keys.Return) {
                if (GridControl.AutoFilterRowHandle != gridView.FocusedRowHandle&& GridControl.InvalidRowHandle != gridView.FocusedRowHandle) {
                    var projectWrapper = (ProjectItemWrapper)gridView.GetRow(gridView.FocusedRowHandle);
                    if (e.Control) {
                        Solution solution = _dte.Solution;
                        string solutionConfigurationName = solution.SolutionBuild.ActiveConfiguration.Name;
                        solution.SolutionBuild.BuildProject(solutionConfigurationName, projectWrapper.UniqueName);
                    } else
                        OpenModelEditor(projectWrapper);
                } else if (gridView.RowCount > 0)
                    OpenModelEditor((ProjectItemWrapper)gridView.GetRow(0));
            }
        }

        private void gridView1_DoubleClick(object sender, EventArgs e) {
            var gridView = ((GridView)sender);
            OpenModelEditor((ProjectItemWrapper)gridView.GetRow(gridView.FocusedRowHandle));
        }

    }
}
