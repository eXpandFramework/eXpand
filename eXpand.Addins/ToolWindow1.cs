using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using DevExpress.CodeRush.Core;
using DevExpress.CodeRush.PlugInCore;
using DevExpress.DXCore.Controls.XtraGrid;
using DevExpress.DXCore.Controls.XtraGrid.Views.Grid;
using EnvDTE;
using eXpandAddIns.Enums;
using Process=System.Diagnostics.Process;
using Project=EnvDTE.Project;
using eXpandAddIns.Extensioons;

namespace eXpandAddIns
{
    [Title("Xaf Models")]
    [ToolboxItem(false)]
    public partial class ToolWindow1 : ToolWindowPlugIn
    {




        // DXCore-generated code...
        #region InitializePlugIn
        // ReSharper disable RedundantOverridenMember
        public override void InitializePlugIn()
        {
            base.InitializePlugIn();
            SetGridDataSource();

            events.ProjectAdded += project => addProject(project);
            events.ProjectRemoved += project => removeProject(project);
        }

        private int addProject(Project project)
        {
            return ((IList)gridControl1.DataSource).Add(project);
        }

        private void SetGridDataSource()
        {
            IEnumerable<Project> projects = CodeRush.Solution.AllProjects.Select(project => CodeRush.Solution.FindEnvDTEProject(project.Name));
            IEnumerable<Project> enumerable =
                projects.Where(
                    project =>
                    project.ProjectItems != null &&
                    project.ProjectItems.Cast<ProjectItem>().Any(item => item.Name.EndsWith(".xafml")));
            gridControl1.DataSource = new List<Project>(enumerable);
            var gridView = ((GridView)gridControl1.MainView);
            gridView.FocusedRowHandle = GridControl.AutoFilterRowHandle;
        }

        private void removeProject(Project project)
        {

            var list = (List<Project>)gridControl1.DataSource;
            list.Remove(project);
            gridControl1.RefreshDataSource();

        }


        // ReSharper restore RedundantOverridenMember
        #endregion
        #region FinalizePlugIn
        // ReSharper disable RedundantOverridenMember
        public override void FinalizePlugIn()
        {
            //
            // TODO: Add your finalization code here.
            //

            base.FinalizePlugIn();
        }

        // ReSharper restore RedundantOverridenMember
        #endregion
        private ProjectItem getModelItem(Project project)
        {
            return project.ProjectItems.Cast<ProjectItem>().Where(item => item.Name.EndsWith(".xafml")).FirstOrDefault();
        }

        private void openModelEditor(Project project)
        {

            Configuration configuration = project.ConfigurationManager.ActiveConfiguration;
            Property outputPathProperty = configuration.FindProperty(ConfigurationProperty.OutputPath);
            Property outputFileProperty = project.FindProperty(ProjectProperty.OutputFileName);
            Property outputDiffsProperty = project.FindProperty(ProjectProperty.FullPath);

            string outputFileName = outputFileProperty.Value.ToString();
            if (outputFileName.ToLower().EndsWith(".exe"))
                outputFileName += ".config";


            ProjectItem modelItem = getModelItem(project);

            if (modelItem != null)
                using (var storage = new DecoupledStorage(typeof(Options)))
                {
                    string path = storage.ReadString(Options.GetPageName(), "modelEditorPath");
                    if (!string.IsNullOrEmpty(path))
                    {
                        string assemblyName = Path.Combine(outputDiffsProperty.Value.ToString(),
                                                           Path.Combine(outputPathProperty.Value.ToString(),
                                                                        outputFileName));
                        if (!File.Exists(assemblyName))
                        {
                            MessageBox.Show("Assembly " + assemblyName + " not found", null, MessageBoxButtons.OK);
                            return;
                        }
                        string arguments = string.Format("\"{0}\" \"{1}\"",
                                                         assemblyName,
                                                         outputDiffsProperty.Value);
                        if (File.Exists(path))
                            Process.Start(path, arguments);
                        else
                            MessageBox.Show("Model editor not found at " + path);
                    }
                    else
                        MessageBox.Show("ModelEditorPath path is empty");
                }
        }


        private void gridView1_KeyUp(object sender, KeyEventArgs e)
        {
            var gridView = ((GridView)gridControl1.MainView);
            if (gridView.FocusedRowHandle == 0 && e.KeyCode == Keys.Up)
                gridView.FocusedRowHandle = GridControl.AutoFilterRowHandle;
            if (e.KeyCode == Keys.Return)
            {
                if (GridControl.AutoFilterRowHandle != gridView.FocusedRowHandle)
                {
                    var project = (Project)gridView.GetRow(gridView.FocusedRowHandle);
                    if (e.Control)
                    {
                        Solution solution = CodeRush.Solution.Active;
                        string solutionConfigurationName = solution.SolutionBuild.ActiveConfiguration.Name;
                        solution.SolutionBuild.BuildProject(solutionConfigurationName, project.UniqueName, true);
                    }
                    openModelEditor(project);
                }
                else if (gridView.RowCount > 0)
                    openModelEditor((Project)gridView.GetRow(0));
            }
        }

        private void openModelEditorAction_Execute(ExecuteEventArgs ea)
        {
            SetGridDataSource();
            Show();
        }


        private void gridView1_DoubleClick(object sender, EventArgs e)
        {
            var gridView = ((GridView)sender);
            openModelEditor((Project)gridView.GetRow(gridView.FocusedRowHandle));
        }

    }
}