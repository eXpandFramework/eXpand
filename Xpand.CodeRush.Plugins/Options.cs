using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using DevExpress.CodeRush.Core;
using DevExpress.DXCore.Controls.XtraEditors.Controls;
using DevExpress.DXCore.Controls.XtraGrid.Views.Grid;
using EnvDTE;
using Xpand.CodeRush.Plugins.Extensions;

namespace Xpand.CodeRush.Plugins {
    [UserLevel(UserLevel.NewUser)]
    public partial class Options : OptionsPage {
        

        // DXCore-generated code...

        #region Initialize
        protected override void Initialize() {
            base.Initialize();
            
            gridView1.KeyDown+=GridViewOnKeyDown;
            gridView2.KeyDown+=GridViewOnKeyDown;
            gridView3.KeyDown+=GridViewOnKeyDown;
            gridView4.KeyDown+=GridViewOnKeyDown;
            
            PreparePage+=OnPreparePage;
        }


        private void OnPreparePage(object sender, OptionsPageStorageEventArgs ea){
            projectConverterPathButtonEdit.Text = OptionClass.Instance.ProjectConverterPath;
            testExecutorButtonEdit.Text = OptionClass.Instance.TestExecutorPath;
            publicTokenTextEdit.Text = OptionClass.Instance.Token;
            specificVersionCheckEdit.Checked = OptionClass.Instance.SpecificVersion;
            checkEditDebugME.Checked = OptionClass.Instance.DebugME;
            checkEditKillModelEditor.Checked = OptionClass.Instance.KillModelEditor;

            gridControlConnectionStrings.DataSource = OptionClass.Instance.ConnectionStrings;
            gridControlLoadProjectFromReferenceItem.DataSource = OptionClass.Instance.SourceCodeInfos;
            gridControlME.DataSource = OptionClass.Instance.MEs;
            gridControlAssemblyFolders.DataSource = OptionClass.Instance.ReferencedAssembliesFolders;
        }



        private void GridViewOnKeyDown(object sender, KeyEventArgs args) {
            if (args.KeyCode == Keys.Delete) {
                var gridView = ((GridView)sender);
                ((IList)gridView.DataSource).Remove(gridView.GetRow(gridView.FocusedRowHandle));
            }
        }

        #endregion

        #region GetCategory
        public static string GetCategory() {
            return @"XAF";
        }
        #endregion
        #region GetPageName
        public static string GetPageName() {
            return @"XafAddins";
        }
        #endregion


        private void openFileDialog1_FileOk(object sender, CancelEventArgs e) {
            if (Equals(openFileDialog1.Tag, "TestExecutorPath")) {
                testExecutorButtonEdit.Text = openFileDialog1.FileName;
            }
            else if (Equals(openFileDialog1.Tag, "ProjectConverterPath")) {
                projectConverterPathButtonEdit.Text = openFileDialog1.FileName;
            }
        }

        private void Options_CommitChanges(object sender, CommitChangesEventArgs ea){
            var instance = OptionClass.Instance;
            instance.ProjectConverterPath = projectConverterPathButtonEdit.Text;
            instance.TestExecutorPath = testExecutorButtonEdit.Text;
            instance.Token = publicTokenTextEdit.Text;
            instance.SpecificVersion = specificVersionCheckEdit.Checked;
            instance.DebugME = checkEditDebugME.Checked;
            instance.KillModelEditor = checkEditKillModelEditor.Checked;
            instance.Save();
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

        public static bool ReadBool(string key){
            return Storage.ReadBoolean(GetPageName(), key);
        }
        public static string ReadString(string key) {
            return Storage.ReadString(GetPageName(), key);
        }

        private void testExecutorButtonEdit_ButtonClick_1(object sender, ButtonPressedEventArgs e) {
            ShowDialog("TestExecutorPath", OptionClass.Instance.TestExecutorPath);
        }

    }

    public class OptionClass{
        private static readonly DTE _dte = DevExpress.CodeRush.Core.CodeRush.ApplicationObject;
        private readonly BindingList<ConnectionString> _connectionStrings=new BindingList<ConnectionString>();
        private readonly BindingList<ReferencedAssembliesFolder> _referencedAssembliesFolders=new BindingList<ReferencedAssembliesFolder>();
        private readonly BindingList<ME> _mEs=new BindingList<ME>();
        private readonly BindingList<SourceCodeInfo> _sourceCodeInfos=new BindingList<SourceCodeInfo>();
        private static readonly OptionClass _instance;
        private static readonly string _path;

        static OptionClass(){
            var storage = DevExpress.CodeRush.Core.CodeRush.Options.GetStorage(typeof(Options));
            _path = Path.Combine(Path.GetDirectoryName(storage.FileName) + "", Path.GetFileNameWithoutExtension(storage.FileName) + "-Options.xml");
            _instance = GetOptionClass();
        }

        public static OptionClass Instance{
            get { return _instance; }
        }

        public bool KillModelEditor { get; set; }


        public bool DebugME { get; set; }


        public bool SpecificVersion { get; set; }


        public string Token { get; set; }

        public string TestExecutorPath { get; set; }


        public string ProjectConverterPath { get; set; }

        private static OptionClass GetOptionClass() {
            var optionClass = new OptionClass();
            try {
                var xmlSerializer = new XmlSerializer(typeof(OptionClass));

                var allText = File.ReadAllText(_path);
                if (!string.IsNullOrWhiteSpace(allText)){
                    var xmlReader = XmlReader.Create(new StringReader(allText));
                    if (xmlSerializer.CanDeserialize(xmlReader)){
                        optionClass = (OptionClass) xmlSerializer.Deserialize(xmlReader);
                    }
                }
            }
            catch (Exception e) {
                _dte.WriteToOutput(e.ToString());
            }
            return optionClass;

        }

        [XmlArray]
        public BindingList<ConnectionString> ConnectionStrings{
            get { return _connectionStrings; }
        }
        [XmlArray]
        public BindingList<ReferencedAssembliesFolder> ReferencedAssembliesFolders{
            get { return _referencedAssembliesFolders; }
        }

        [XmlArray]
        public BindingList<ME> MEs{
            get { return _mEs; }
        }
        [XmlArray]
        public BindingList<SourceCodeInfo> SourceCodeInfos{
            get { return _sourceCodeInfos; }
        }

        public void Save(){
            var stringBuilder = new StringBuilder();
            new XmlSerializer(typeof(OptionClass)).Serialize(XmlWriter.Create(stringBuilder),Instance );
            File.WriteAllText(_path,stringBuilder.ToString());
        }
    }
    public class ConnectionString :OptionClassBase{
        public string Name { get; set; }
    }

    public class ReferencedAssembliesFolder:OptionClassBase{

        public string Folder { get; set; }
    }

    public class ME:OptionClassBase{
        public string Path { get; set; }
    }


    public class SourceCodeInfo:OptionClassBase {
        private readonly List<ProjectInfo> _projectPaths=new List<ProjectInfo>();
        public string RootPath { get; set; }
        public string ProjectRegex { get; set; }

        public int Count{
            get { return ProjectPaths.Count; }
        }

        [XmlArray]
        public List<ProjectInfo> ProjectPaths{
            get { return _projectPaths; }
        }

        public override string ToString() {
            return "RootPath:" + RootPath + " ProjectRegex=" + ProjectRegex + " Count=" + Count;
        }
    }

    public class ProjectInfo{
        public string Path { get; set; }
        public string OutputPath { get; set; }
    }

    public class OptionClassBase{
    }
}