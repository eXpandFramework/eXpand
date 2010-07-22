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
            events.SolutionOpened += SetGridDataSource;
            events.ProjectAdded += project => addProject(project);
            events.ProjectRemoved += removeProject;
        }

        private int addProject(Project project)
        {
            return ((IList)gridControl1.DataSource).Add(project);
        }

        private void SetGridDataSource()
        {
            gridControl1.DataSource = GetProjectWrappers().ToList();
            var gridView = ((GridView)gridControl1.MainView);
            gridView.FocusedRowHandle = GridControl.AutoFilterRowHandle;
        }

        IEnumerable<ProjectWrapper> GetProjectWrappers() {
            IEnumerable<Project> projects = CodeRush.Solution.AllProjects.Select(project => CodeRush.Solution.FindEnvDTEProject(project.Name));


            projects =projects.Where(project =>project.ProjectItems != null &&
                                               project.ProjectItems.Cast<ProjectItem>().Any(item => item.Name.EndsWith(".xafml")));
            return projects.Select(project1 =>new ProjectWrapper {
                                                                     Name = project1.Name,
                                                                     OutputPath =project1.ConfigurationManager.ActiveConfiguration.FindProperty(ConfigurationProperty.OutputPath).Value.ToString(),
                                                                     OutPutFileName =project1.FindProperty(ProjectProperty.OutputFileName).Value.ToString(),
                                                                     FullPath = project1.FindProperty(ProjectProperty.FullPath).Value.ToString(),
                                                                     UniqueName = project1.UniqueName
                                                                 });
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
//        private ProjectItem getModelItem(Project project)
//        {
//            return project.ProjectItems.Cast<ProjectItem>().Where(item => item.Name.EndsWith(".xafml")).FirstOrDefault();
//        }

        public class ProjectWrapper {
            public string Name { get; set; }

            public string OutputPath { get; set; }
            public string OutPutFileName { get; set; }
            public string FullPath { get; set; }

            public string UniqueName { get; set; }
        }
        private void openModelEditor(ProjectWrapper projectWrapper)
        {

//            Configuration configuration = projectWrapper.ConfigurationManager.ActiveConfiguration;
//            Property outputPathProperty = configuration.FindProperty(ConfigurationProperty.OutputPath);
//            Property outputFileProperty = projectWrapper.FindProperty(ProjectProperty.OutputFileName);
//            Property outputDiffsProperty = projectWrapper.FindProperty(ProjectProperty.FullPath);

//            string outputFileName = outputFileProperty.Value.ToString();
            string outputFileName = projectWrapper.OutPutFileName;
            if (outputFileName.ToLower().EndsWith(".exe"))
                outputFileName += ".config";


            

            
                using (var storage = new DecoupledStorage(typeof(Options)))
                {
                    string path = storage.ReadString(Options.GetPageName(), "modelEditorPath");
                    if (!string.IsNullOrEmpty(path))
                    {
                        string assemblyName = Path.Combine(projectWrapper.FullPath,
                                                           Path.Combine(projectWrapper.OutputPath,
                                                                        outputFileName));
                        if (!File.Exists(assemblyName))
                        {
                            MessageBox.Show(string.Format(@"Assembly {0} not found", assemblyName), null, MessageBoxButtons.OK);
                            return;
                        }
                        string arguments = string.Format("\"{0}\" \"{1}\"",
                                                         assemblyName,
                                                         projectWrapper.FullPath);
                        if (File.Exists(path))
                            Process.Start(path, arguments);
                        else
                            MessageBox.Show(string.Format("Model editor not found at {0}", path));
                    }
                    else {
                        const string modeleditorpathPathIsEmpty = "ModelEditorPath path is empty";
                        MessageBox.Show(modeleditorpathPathIsEmpty);
                    }
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
                    var projectWrapper = (ProjectWrapper)gridView.GetRow(gridView.FocusedRowHandle);
                    if (e.Control)
                    {
                        Solution solution = CodeRush.Solution.Active;
                        string solutionConfigurationName = solution.SolutionBuild.ActiveConfiguration.Name;
                        solution.SolutionBuild.BuildProject(solutionConfigurationName, projectWrapper.UniqueName, true);
                    }
                    openModelEditor(projectWrapper);
                }
                else if (gridView.RowCount > 0)
                    openModelEditor((ProjectWrapper)gridView.GetRow(0));
            }
        }

        private void openModelEditorAction_Execute(ExecuteEventArgs ea)
        {
//            SetGridDataSource();
            Show();
        }


        private void gridView1_DoubleClick(object sender, EventArgs e)
        {
            var gridView = ((GridView)sender);
            openModelEditor((ProjectWrapper)gridView.GetRow(gridView.FocusedRowHandle));
        }

    }
}