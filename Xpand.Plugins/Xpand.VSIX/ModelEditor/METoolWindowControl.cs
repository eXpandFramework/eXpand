using System;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.DXCore.Controls.XtraGrid;
using DevExpress.DXCore.Controls.XtraGrid.Views.Grid;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell.Interop;
using Xpand.VSIX.Extensions;

namespace Xpand.VSIX.ModelEditor {
    
    public partial class METoolWindowControl : UserControl {
        private readonly DTE2 _dte = DteExtensions.DTE;
        string _buildingProject;
        bool? _atLeastOneFail;

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
                    var projectWrapper = (ProjectItemWrapper)gridView1.GetRow(gridView1.FocusedRowHandle);
                    OpenModelEditor(projectWrapper);
                }
            }

        }

        public METoolWindowControl(){
            InitializeComponent();
            GridHelper.Init(gridControl1);
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
            if (gridView.FocusedRowHandle == 0 && e.KeyCode == Keys.Up)
                gridView.FocusedRowHandle = GridControl.AutoFilterRowHandle;
            if (e.KeyCode == Keys.Return) {
                if (GridControl.AutoFilterRowHandle != gridView.FocusedRowHandle&& GridControl.InvalidRowHandle != gridView.FocusedRowHandle) {
                    var projectWrapper = (ProjectItemWrapper)gridView.GetRow(gridView.FocusedRowHandle);
                    if (e.Control) {
                        Solution solution = _dte.Solution;
                        string solutionConfigurationName = solution.SolutionBuild.ActiveConfiguration.Name;
                        _buildingProject = projectWrapper.UniqueName;
                        solution.SolutionBuild.BuildProject(solutionConfigurationName, projectWrapper.UniqueName);
                    } else
                        OpenModelEditor(projectWrapper);
                } else if (gridView.RowCount > 0)
                    OpenModelEditor((ProjectItemWrapper)gridView.GetRow(0));
            }
        }


        static IVsWindowFrame GetWindowFrameFromGuid(Guid guid) {
            Guid lSlot = guid;
            IVsWindowFrame lFrame;
//            VisualStudioServices.VsUIShell.FindToolWindow((uint)__VSFINDTOOLWIN.FTW_fFrameOnly, ref lSlot, out lFrame);
            return null;
        }
        static IVsWindowFrame GetWindowFrameFromWindow(Window window){
            return window == null
                ? null
                : (string.IsNullOrEmpty(window.ObjectKind) ? null : GetWindowFrameFromGuid(new Guid(window.ObjectKind)));
        }

        public static void ShowWindowDocked() {
//            var window = DevExpress.CodeRush.Core.CodeRush.ToolWindows.Show<METoolWindow>();
//            if (window != null) {
//                IVsWindowFrame frame = GetWindowFrameFromWindow(window);
//                if (frame != null) {
//                    frame.SetProperty((int)__VSFPROPID.VSFPROPID_FrameMode, VSFRAMEMODE.VSFM_MdiChild);
//                }
//            }
        }


        private void gridView1_DoubleClick(object sender, EventArgs e) {
            var gridView = ((GridView)sender);
            OpenModelEditor((ProjectItemWrapper)gridView.GetRow(gridView.FocusedRowHandle));
        }

    }
}