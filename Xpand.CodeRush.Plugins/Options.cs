using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using DevExpress.CodeRush.Core;
using DevExpress.DXCore.Controls.XtraEditors.Controls;
using DevExpress.DXCore.Controls.XtraGrid.Views.Grid;

namespace Xpand.CodeRush.Plugins {
    [UserLevel(UserLevel.NewUser)]
    public partial class Options : OptionsPage {
        public const string ProjectPaths = "ProjectPaths";
        public const string FormatOnSave = "FormatOnSave";
        public const string SpecificVersion = "SpecificVersion";
        public const string DebugME = "DebugME";
        public const string KillModelEditor = "KillModelEditor";
        public const string ProjectConverterPath = "projectConverterPath";
        public const string TestExecutorPath = "testExecutorPath";
        public const string Token = "token";
        private const string ConnectionStringsKey = "ConnectionStrings";
        private const string SourceCodeInfosKey = "SourceCodeInfos";
        private const string MEKey = "ME";

        // DXCore-generated code...

        #region Initialize
        protected override void Initialize() {
            base.Initialize();

            
            PreparePage+=OnPreparePage;
            gridView1.KeyDown += GridView1OnKeyDown;
            gridView2.KeyDown += GridView1OnKeyDown;

        }

        private void OnPreparePage(object sender, OptionsPageStorageEventArgs ea){
            DecoupledStorage storage = GetStorage();
            projectConverterPathButtonEdit.Text = storage.ReadString(PageName, ProjectConverterPath, projectConverterPathButtonEdit.Text);
            testExecutorButtonEdit.Text = storage.ReadString(PageName, TestExecutorPath, testExecutorButtonEdit.Text);
            publicTokenTextEdit.Text = storage.ReadString(PageName, Token, publicTokenTextEdit.Text);
            formatOnSaveCheckEdit.Checked = storage.ReadBoolean(PageName, FormatOnSave, formatOnSaveCheckEdit.Checked);
            specificVersionCheckEdit.Checked = storage.ReadBoolean(PageName, SpecificVersion, true);
            checkEditDebugME.Checked = storage.ReadBoolean(PageName, DebugME, checkEditDebugME.Checked);
            checkEditKillModelEditor.Checked = storage.ReadBoolean(PageName, KillModelEditor, true);

            gridControlConnectionStrings.DataSource = GetConnectionStrings();
            gridControlReferencedAssemblies.DataSource = GetSourceCodeInfos();
            gridControlME.DataSource = GetMEPaths();
        }


        public static BindingList<SourceCodeInfo> GetSourceCodeInfos() {
            return GetDataSource(CreateSourceCodeInfo(), SourceCodeInfosKey);
        }

        public static BindingList<ConnectionString> GetConnectionStrings() {
            return GetDataSource(CreateConnectionString(), ConnectionStringsKey);
        }
        public static BindingList<ME> GetMEPaths() {
            return GetDataSource(CreateME(), MEKey);
        }

        private static Func<string, ME> CreateME(){
            return s => new ME() {Path = s};
        }

        static Func<string, SourceCodeInfo> CreateSourceCodeInfo() {
            return s => {
                string[] strings = s.Split('|');
                var strings1 = strings[1].Split('+');
                return new SourceCodeInfo { RootPath = strings[0], ProjectRegex = strings1[0], Count = Convert.ToInt32(strings1[1]) };
            };
        }

        static Func<string, ConnectionString> CreateConnectionString() {
            return s => new ConnectionString { Name = s };
        }

        static BindingList<T> GetDataSource<T>(Func<string, T> selector, string key) {
            string readString = Storage.ReadString(GetPageName(), key, "");
            return new BindingList<T>(readString.Split(';').Where(s => !(string.IsNullOrEmpty(s))).Select(selector).ToList());
        }

        private void GridView1OnKeyDown(object sender, KeyEventArgs args) {
            if (args.KeyCode == Keys.Delete) {
                var gridView = ((GridView)sender);
                ((IList)gridView.DataSource).Remove(gridView.GetRow(gridView.FocusedRowHandle));
            }
        }

        public class SourceCodeInfo {
            public string RootPath { get; set; }
            public string ProjectRegex { get; set; }
            public int Count { get; set; }
            public override string ToString() {
                return "RootPath:" + RootPath + " ProjectRegex=" + ProjectRegex + " Count=" + Count;
            }
        }

        public class ME{
            public string Path { get; set; }
        }

        public class ConnectionString {
            public string Name { get; set; }
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
            if (Equals(openFileDialog1.Tag, TestExecutorPath)) {
                testExecutorButtonEdit.Text = openFileDialog1.FileName;
            }
            else if (Equals(openFileDialog1.Tag, ProjectConverterPath)) {
                projectConverterPathButtonEdit.Text = openFileDialog1.FileName;
            }
        }

        private void Options_CommitChanges(object sender, CommitChangesEventArgs ea) {
            var decoupledStorage = ea.Storage;
            decoupledStorage.WriteString(PageName, Token, publicTokenTextEdit.Text);
            decoupledStorage.WriteString(PageName, ProjectConverterPath, projectConverterPathButtonEdit.Text);
            decoupledStorage.WriteString(PageName, TestExecutorPath, testExecutorButtonEdit.Text);
            decoupledStorage.WriteBoolean(PageName, FormatOnSave, formatOnSaveCheckEdit.Checked);
            decoupledStorage.WriteBoolean(PageName, SpecificVersion, specificVersionCheckEdit.Checked);
            decoupledStorage.WriteBoolean(PageName, DebugME, checkEditDebugME.Checked);
            decoupledStorage.WriteBoolean(PageName, KillModelEditor, checkEditKillModelEditor.Checked);
            decoupledStorage.WriteString(PageName, SourceCodeInfosKey, "");
            SaveDataSource(SerializeConnectionString, ConnectionStringsKey, decoupledStorage, (BindingList<ConnectionString>)gridControlConnectionStrings.DataSource);
            SaveDataSource(SerializeSourceCodeInfo, SourceCodeInfosKey, decoupledStorage, (BindingList<SourceCodeInfo>)gridControlReferencedAssemblies.DataSource);
            SaveDataSource(SerializeMe, MEKey, decoupledStorage, (BindingList<ME>)gridControlME.DataSource);
        }

        private string SerializeMe(ME me){
            return me.Path + ";";
        }

        string SerializeSourceCodeInfo(SourceCodeInfo sourceCodeInfo) {
            return sourceCodeInfo.RootPath + "|" + sourceCodeInfo.ProjectRegex + "+" + sourceCodeInfo.Count + ";";
        }

        string SerializeConnectionString(ConnectionString arg) {
            return arg.Name + ";";
        }


        void SaveDataSource<T>(Func<T, string> func, string key, DecoupledStorage storage, IEnumerable<T> dataSource) {
            string connectionStrings = dataSource.Aggregate<T, string>(null, (current, connectionString) => current + func.Invoke(connectionString));
            storage.WriteString(PageName, key, connectionStrings);
        }


        private void button1_Click(object sender, EventArgs e) {
            button1.Enabled = false;
            var gridView = ((GridView)gridControlReferencedAssemblies.MainView);
            for (int i = 0; i < gridView.RowCount; i++) {
                var codeInfo = (SourceCodeInfo)gridView.GetRow(i);
                StoreProjectPaths(codeInfo, i);
            }
            gridControlReferencedAssemblies.RefreshDataSource();
            button1.Enabled = true;
        }

        void StoreProjectPaths(SourceCodeInfo sourceCodeInfo, int index) {
            var enumerable=new string[0];
            if (Directory.Exists(sourceCodeInfo.RootPath)){
                var projectPaths =
                    Directory.GetFiles(sourceCodeInfo.RootPath, "*.csproj", SearchOption.AllDirectories)
                        .Where(s => Regex.IsMatch(Path.GetFileName(s) + "", sourceCodeInfo.ProjectRegex));
                IEnumerable<string> paths = projectPaths.Select(s1 => s1 + "|" + GetOutPutPath(s1));
                enumerable = paths as string[] ?? paths.ToArray();
                sourceCodeInfo.Count = enumerable.Length;
            }
            Storage.WriteStrings(ProjectPaths, index + "_" + sourceCodeInfo.ProjectRegex, enumerable.ToArray());
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
            ShowDialog(ProjectConverterPath, Storage.ReadString(PageName, ProjectConverterPath, projectConverterPathButtonEdit.Text));
        }

        public static bool ReadBool(string key){
            return Storage.ReadBoolean(GetPageName(), key);
        }
        public static string ReadString(string key) {
            return Storage.ReadString(GetPageName(), key);
        }

        private void testExecutorButtonEdit_ButtonClick_1(object sender, ButtonPressedEventArgs e) {
            ShowDialog(TestExecutorPath, Storage.ReadString(PageName, TestExecutorPath, testExecutorButtonEdit.Text));
        }

    }
}