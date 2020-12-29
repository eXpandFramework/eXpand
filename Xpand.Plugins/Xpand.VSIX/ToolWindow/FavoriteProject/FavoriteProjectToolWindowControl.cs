using System;
using System.IO;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using EnvDTE;
using EnvDTE80;
using Xpand.VSIX.Extensions;
using Xpand.VSIX.ToolWindow.ModelEditor;

namespace Xpand.VSIX.ToolWindow.FavoriteProject {
    public partial class FavoriteProjectToolWindowControl : UserControl {
        private readonly DTE2 _dte = DteExtensions.DTE;

        public FavoriteProjectToolWindowControl() {
            InitializeComponent();
            gridControl1.LookAndFeel.Assign(defaultLookAndFeel1.LookAndFeel);
            GridHelper.Init(gridControl1,false).Subscribe();
            gridView1.KeyDown+=GridView1OnKeyDown;
            gridView1.KeyUp+=gridView1_KeyUp;
            gridView1.DoubleClick+=gridView1_DoubleClick;
        }


        private void GridView1OnKeyDown(object sender, KeyEventArgs e){
            var gridView = ((GridView)gridControl1.MainView);
            if (gridView.FocusedRowHandle == 0 && e.KeyCode == Keys.Up)
                gridView.FocusedRowHandle = GridControl.AutoFilterRowHandle;
        }

        private void LoadProject(ProjectItemWrapper projectItemWrapper) {
            _dte.InitOutputCalls("LoadProject");
            try{
                if (!File.Exists(projectItemWrapper.FullPath)) {
                    _dte.WriteToOutput($"NOT FOUND {projectItemWrapper.FullPath}");
                }
                var project = DteExtensions.DTE.Solution.AddFromFile(projectItemWrapper.FullPath);
                project.SkipBuild();
                project.ChangeActiveConfiguration();
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
                        LoadProject(projectWrapper);
                } else if (gridView.RowCount > 0)
                    LoadProject((ProjectItemWrapper)gridView.GetRow(0));
            }
        }

        private void gridView1_DoubleClick(object sender, EventArgs e) {
            var gridView = ((GridView)sender);
            LoadProject((ProjectItemWrapper)gridView.GetRow(gridView.FocusedRowHandle));
        }

    }
}
