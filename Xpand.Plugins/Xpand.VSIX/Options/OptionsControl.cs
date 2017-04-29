using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using DevExpress.DXCore.Controls.XtraEditors.Controls;
using DevExpress.DXCore.Controls.XtraGrid.Views.Grid;

namespace Xpand.VSIX.Options {
    public partial class OptionsControl : UserControl {
        public OptionsControl(){
            InitializeComponent();
            gridView1.KeyDown += GridViewOnKeyDown;
            gridView2.KeyDown += GridViewOnKeyDown;
            gridView3.KeyDown += GridViewOnKeyDown;
            gridView4.KeyDown += GridViewOnKeyDown;
            gridView5.KeyDown += GridViewOnKeyDown;
            projectConverterPathButtonEdit.Text = OptionClass.Instance.ProjectConverterPath;
            testExecutorButtonEdit.Text = OptionClass.Instance.TestExecutorPath;
            publicTokenTextEdit.Text = OptionClass.Instance.Token;
            specificVersionCheckEdit.Checked = OptionClass.Instance.SpecificVersion;
            checkEditDebugME.Checked = OptionClass.Instance.DebugME;
            checkEditKillModelEditor.Checked = OptionClass.Instance.KillModelEditor;
            checkEditDisableExceptions.Checked = OptionClass.Instance.DisableExceptions;

            gridControlConnectionStrings.DataSource = OptionClass.Instance.ConnectionStrings;
            gridControlLoadProjectFromReferenceItem.DataSource = OptionClass.Instance.SourceCodeInfos;
            gridControlME.DataSource = OptionClass.Instance.MEs;
            gridControlAssemblyFolders.DataSource = OptionClass.Instance.ReferencedAssembliesFolders;
            gridControlExceptions.DataSource = OptionClass.Instance.Exceptions;

            Save();
        }

        public void Save(){
            var instance = OptionClass.Instance;
            instance.ProjectConverterPath = projectConverterPathButtonEdit.Text;
            instance.TestExecutorPath = testExecutorButtonEdit.Text;
            instance.Token = publicTokenTextEdit.Text;
            instance.SpecificVersion = specificVersionCheckEdit.Checked;
            instance.DebugME = checkEditDebugME.Checked;
            instance.KillModelEditor = checkEditKillModelEditor.Checked;
            instance.DisableExceptions = checkEditDisableExceptions.Checked;

            OptionClass.Instance.ConnectionStrings = (BindingList<ConnectionString>) gridControlConnectionStrings.DataSource;
            OptionClass.Instance.SourceCodeInfos = (BindingList<SourceCodeInfo>) gridControlLoadProjectFromReferenceItem.DataSource;
            OptionClass.Instance.MEs = (BindingList<ME>) gridControlME.DataSource;
            OptionClass.Instance.ReferencedAssembliesFolders = (BindingList<ReferencedAssembliesFolder>) gridControlAssemblyFolders.DataSource;
            OptionClass.Instance.Exceptions = (BindingList<ExceptionsBreak>) gridControlExceptions.DataSource;
            instance.Save();
        }


        private void GridViewOnKeyDown(object sender, KeyEventArgs args) {
            if (args.KeyCode == Keys.Delete) {
                var gridView = ((GridView)sender);
                ((IList)gridView.DataSource).Remove(gridView.GetRow(gridView.FocusedRowHandle));
            }
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e) {
            if (Equals(openFileDialog1.Tag, "TestExecutorPath")) {
                testExecutorButtonEdit.Text = openFileDialog1.FileName;
            }
            else if (Equals(openFileDialog1.Tag, "ProjectConverterPath")) {
                projectConverterPathButtonEdit.Text = openFileDialog1.FileName;
            }
        }



        private void button1_Click(object sender, EventArgs e) {
            button1.Enabled = false;
            var gridView = ((GridView)gridControlLoadProjectFromReferenceItem.MainView);
            for (int i = 0; i < gridView.RowCount; i++) {
                var codeInfo = (SourceCodeInfo)gridView.GetRow(i);
                StoreProjectPaths(codeInfo);
            }
            gridControlLoadProjectFromReferenceItem.RefreshDataSource();
            button1.Enabled = true;
        }

        void StoreProjectPaths(SourceCodeInfo sourceCodeInfo) {
            if (Directory.Exists(sourceCodeInfo.RootPath)){
                var projectPaths = Directory.GetFiles(sourceCodeInfo.RootPath, "*.csproj", SearchOption.AllDirectories)
                        .Where(s => Regex.IsMatch(Path.GetFileName(s) + "", sourceCodeInfo.ProjectRegex));
                var paths = projectPaths.Select(path => new ProjectInfo() {Path = path,OutputPath = GetOutPutPath(path)}).ToArray();
                sourceCodeInfo.ProjectPaths.Clear();
                sourceCodeInfo.ProjectPaths.AddRange(paths);
            }
        }

        string GetOutPutPath(string projectPath) {
            using (var fileStream = File.Open(projectPath, FileMode.Open)) {
                var streamReader = new StreamReader(fileStream);
                var readToEnd = streamReader.ReadToEnd();
                Environment.CurrentDirectory = Path.GetDirectoryName(projectPath) + "";
                var outPutPath = Path.GetFullPath(GetAttributeValue(readToEnd, "OutputPath"));
                var assemblyName = GetAttributeValue(readToEnd, "AssemblyName");
                return Path.Combine(outPutPath, assemblyName + ".dll");
            }
        }

        string GetAttributeValue(string readToEnd, string attributeName) {
            var regexObj = new Regex("<" + attributeName + ">([^<]*)</" + attributeName + ">");
            Match matchResults = regexObj.Match(readToEnd);
            return matchResults.Success ? matchResults.Groups[1].Value : null;
        }


        void ShowDialog(string modelEditorPath, string fileName) {
            openFileDialog1.FileName = fileName;
            openFileDialog1.Tag = modelEditorPath;
            openFileDialog1.ShowDialog();
        }

        private void projectConverterPathButtonEdit_ButtonClick(object sender, ButtonPressedEventArgs e) {
            ShowDialog("ProjectConverterPath", OptionClass.Instance.ProjectConverterPath);
        }


        private void testExecutorButtonEdit_ButtonClick_1(object sender, ButtonPressedEventArgs e) {
            ShowDialog("TestExecutorPath", OptionClass.Instance.TestExecutorPath);
        }

    }


}