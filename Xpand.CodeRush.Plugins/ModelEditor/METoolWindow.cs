using System;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.CodeRush.Core;
using DevExpress.CodeRush.PlugInCore;
using DevExpress.DXCore.Controls.XtraGrid;
using DevExpress.DXCore.Controls.XtraGrid.Views.Grid;
using DevExpress.DXCore.Interop;
using EnvDTE;
using Microsoft.VisualStudio.Shell.Interop;
using Xpand.CodeRush.Plugins.Extensions;

namespace Xpand.CodeRush.Plugins.ModelEditor {
    [Title("Xaf Models")]
    [ToolboxItem(false)]
    public partial class METoolWindow : ToolWindowPlugIn {
        private readonly DTE _dte = DevExpress.CodeRush.Core.CodeRush.ApplicationObject;
        string _buildingProject;
        bool? _atLeastOneFail;

        // DXCore-generated code...
        #region InitializePlugIn
        
        public override void InitializePlugIn() {
            base.InitializePlugIn();
            GridBinder.Init(gridControl1, events);
            events.ProjectBuildDone += EventsOnProjectBuildDone;
        }

        void EventsOnProjectBuildDone(string project, string projectConfiguration, string platform, string solutionConfiguration, bool succeeded) {
            if (!(_atLeastOneFail.HasValue) && !succeeded)
                _atLeastOneFail = true;
            if (_buildingProject == project) {
                _buildingProject = null;
                var dialogResult = DialogResult.Yes;
                if (_atLeastOneFail.HasValue && _atLeastOneFail.Value) {
                    _atLeastOneFail = null;
                    dialogResult = MessageBox.Show(@"Build fail!!! Continue opening the MEs?", null, MessageBoxButtons.YesNo);
                }
                if (dialogResult == DialogResult.Yes) {
                    var projectWrapper = (ProjectWrapper)gridView1.GetRow(gridView1.FocusedRowHandle);
                    OpenModelEditor(projectWrapper);
                }
            }

        }
        
        #endregion
        private void OpenModelEditor(ProjectWrapper projectWrapper) {
            _dte.InitOutputCalls("OpenModelEditor");
            try{
                new ModelEditorRunner().Start(projectWrapper);
            }
            catch (Exception e){
                _dte.WriteToOutput(e.ToString());
            }
        }

        private void gridView1_KeyUp(object sender, KeyEventArgs e) {
            var gridView = ((GridView)gridControl1.MainView);
            if (gridView.FocusedRowHandle == 0 && e.KeyCode == Keys.Up)
                gridView.FocusedRowHandle = GridControl.AutoFilterRowHandle;
            if (e.KeyCode == Keys.Return) {
                if (GridControl.AutoFilterRowHandle != gridView.FocusedRowHandle&& GridControl.InvalidRowHandle != gridView.FocusedRowHandle) {
                    var projectWrapper = (ProjectWrapper)gridView.GetRow(gridView.FocusedRowHandle);
                    if (e.Control) {
                        Solution solution = DevExpress.CodeRush.Core.CodeRush.Solution.Active;
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

        static IVsWindowFrame GetWindowFrameFromGuid(Guid guid) {
            Guid lSlot = guid;
            IVsWindowFrame lFrame;
            VisualStudioServices.VsUIShell.FindToolWindow((uint)__VSFINDTOOLWIN.FTW_fFrameOnly, ref lSlot, out lFrame);
            return lFrame;
        }
        static IVsWindowFrame GetWindowFrameFromWindow(Window window){
            return window == null
                ? null
                : (string.IsNullOrEmpty(window.ObjectKind) ? null : GetWindowFrameFromGuid(new Guid(window.ObjectKind)));
        }

        public static void ShowWindowDocked() {
            var window = DevExpress.CodeRush.Core.CodeRush.ToolWindows.Show<METoolWindow>();
            if (window != null) {
                IVsWindowFrame frame = GetWindowFrameFromWindow(window);
                if (frame != null) {
                    frame.SetProperty((int)__VSFPROPID.VSFPROPID_FrameMode, VSFRAMEMODE.VSFM_MdiChild);
                }
            }
        }


        private void gridView1_DoubleClick(object sender, EventArgs e) {
            var gridView = ((GridView)sender);
            OpenModelEditor((ProjectWrapper)gridView.GetRow(gridView.FocusedRowHandle));
        }

    }
}