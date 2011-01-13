using System;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.CodeRush.Core;
using DevExpress.CodeRush.PlugInCore;
using DevExpress.DXCore.Controls.XtraGrid;
using DevExpress.DXCore.Controls.XtraGrid.Views.Grid;
using EnvDTE;

namespace XpandAddIns.ModelEditor {
    [Title("Xaf Models")]
    [ToolboxItem(false)]
    public partial class METoolWindow : ToolWindowPlugIn {
        string _buildingProject;
        bool? atLeastOneFail;

        // DXCore-generated code...
        #region InitializePlugIn
        // ReSharper disable RedundantOverridenMember
        public override void InitializePlugIn() {
            base.InitializePlugIn();
            GridBinder.Init(gridControl1, events);
            events.ProjectBuildDone += EventsOnProjectBuildDone;
        }

        void EventsOnProjectBuildDone(string project, string projectConfiguration, string platform, string solutionConfiguration, bool succeeded) {
            if (!(atLeastOneFail.HasValue) && !succeeded)
                atLeastOneFail = true;
            if (_buildingProject == project) {
                _buildingProject = null;
                DialogResult dialogResult = DialogResult.Yes;
                if (atLeastOneFail.HasValue && atLeastOneFail.Value) {
                    atLeastOneFail = null;
                    dialogResult = MessageBox.Show(@"Build fail!!! Continue opening the ME?", null, MessageBoxButtons.YesNo);
                }
                if (dialogResult == DialogResult.Yes) {
                    var projectWrapper = (ProjectWrapper)gridView1.GetRow(gridView1.FocusedRowHandle);
                    OpenModelEditor(projectWrapper);
                }
            }

        }


        // ReSharper restore RedundantOverridenMember
        #endregion
        #region FinalizePlugIn
        // ReSharper disable RedundantOverridenMember
        public override void FinalizePlugIn() {
            //
            // TODO: Add your finalization code here.
            //

            base.FinalizePlugIn();
        }

        // ReSharper restore RedundantOverridenMember
        #endregion
        private void OpenModelEditor(ProjectWrapper projectWrapper) {
            new ModelEditorRunner().Start(projectWrapper);
        }


        private void gridView1_KeyUp(object sender, KeyEventArgs e) {
            var gridView = ((GridView)gridControl1.MainView);
            if (gridView.FocusedRowHandle == 0 && e.KeyCode == Keys.Up)
                gridView.FocusedRowHandle = GridControl.AutoFilterRowHandle;
            if (e.KeyCode == Keys.Return) {
                if (GridControl.AutoFilterRowHandle != gridView.FocusedRowHandle) {
                    var projectWrapper = (ProjectWrapper)gridView.GetRow(gridView.FocusedRowHandle);
                    if (e.Control) {
                        Solution solution = CodeRush.Solution.Active;
                        string solutionConfigurationName = solution.SolutionBuild.ActiveConfiguration.Name;
                        _buildingProject = projectWrapper.UniqueName;
                        solution.SolutionBuild.BuildProject(solutionConfigurationName, projectWrapper.UniqueName);
                    } else
                        OpenModelEditor(projectWrapper);
                } else if (gridView.RowCount > 0)
                    OpenModelEditor((ProjectWrapper)gridView.GetRow(0));
            }
        }

        private void openModelEditorAction_Execute(ExecuteEventArgs ea) {
            Show();
        }


        private void gridView1_DoubleClick(object sender, EventArgs e) {
            var gridView = ((GridView)sender);
            OpenModelEditor((ProjectWrapper)gridView.GetRow(gridView.FocusedRowHandle));
        }

    }
}