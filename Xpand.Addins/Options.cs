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

namespace XpandAddIns {
    [UserLevel(UserLevel.NewUser)]
    public partial class Options : OptionsPage {
        public const string ProjectPaths = "ProjectPaths";
        public const string FormatOnSave = "FormatOnSave";
        public const string ModelEditorPath = "modelEditorPath";
        public const string ProjectConverterPath = "projectConverterPath";
        public const string Token = "token";
        public const string GacUtilPath = "GacUtilPath";
        public const string GacUtilRegex = "GacUtilRegex";
        // DXCore-generated code...
        #region Initialize
        protected override void Initialize() {
            base.Initialize();

            DecoupledStorage storage = GetStorage();
            modelEditorPathButtonEdit.Text = storage.ReadString(PageName, ModelEditorPath, modelEditorPathButtonEdit.Text);
            projectConverterPathButtonEdit.Text = storage.ReadString(PageName, ProjectConverterPath, projectConverterPathButtonEdit.Text);
            gacUtilPathButtonEdit.Text = storage.ReadString(PageName, GacUtilPath, gacUtilPathButtonEdit.Text);
            publicTokenTextEdit.Text = storage.ReadString(PageName, Token, publicTokenTextEdit.Text);
            gacUtilRegexButtonEdit.Text = storage.ReadString(PageName, GacUtilRegex, gacUtilRegexButtonEdit.Text);
            formatOnSaveCheckEdit.Checked = storage.ReadBoolean(PageName, FormatOnSave, formatOnSaveCheckEdit.Checked);

            gridControl1.DataSource = GetConnectionStrings();
            gridControl2.DataSource = GetSourceCodeInfos();
            gridView1.KeyDown += GridView1OnKeyDown;
            gridView2.KeyDown += GridView1OnKeyDown;

        }

        public static BindingList<SourceCodeInfo> GetSourceCodeInfos() {
            return GetDataSource(CreateSourceCodeInfo(), "SourceCodeInfos");
        }

        public static BindingList<ConnectionString> GetConnectionStrings() {
            return GetDataSource(CreateConnectionString(), "ConnectionStrings");
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
            if (Equals(openFileDialog1.Tag, ProjectConverterPath)) {
                projectConverterPathButtonEdit.Text = openFileDialog1.FileName;
            }
            else if (Equals(openFileDialog1.Tag, ModelEditorPath))
                modelEditorPathButtonEdit.Text = openFileDialog1.FileName;
            else if (Equals(openFileDialog1.Tag, GacUtilPath))
                gacUtilPathButtonEdit.Text = openFileDialog1.FileName;
        }

        private void Options_CommitChanges(object sender, CommitChangesEventArgs ea) {
            var decoupledStorage = ea.Storage;
            decoupledStorage.WriteString(PageName, Token, publicTokenTextEdit.Text);
            decoupledStorage.WriteString(PageName, ModelEditorPath, modelEditorPathButtonEdit.Text);
            decoupledStorage.WriteString(PageName, ProjectConverterPath, projectConverterPathButtonEdit.Text);
            decoupledStorage.WriteString(PageName, GacUtilPath, gacUtilPathButtonEdit.Text);
            decoupledStorage.WriteString(PageName, GacUtilRegex, gacUtilRegexButtonEdit.Text);
            decoupledStorage.WriteBoolean(PageName, FormatOnSave, formatOnSaveCheckEdit.Checked);
            decoupledStorage.WriteString(PageName, "SourceCodeInfos", "");
            SaveDataSource(SerializeConnectionString, "ConnectionStrings", decoupledStorage, (BindingList<ConnectionString>)gridControl1.DataSource);
            SaveDataSource(SerializeSourceCodeInfo, "SourceCodeInfos", decoupledStorage, (BindingList<SourceCodeInfo>)gridControl2.DataSource);
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
            var gridView = ((GridView)gridControl2.MainView);
            for (int i = 0; i < gridView.RowCount; i++) {
                var codeInfo = (SourceCodeInfo)gridView.GetRow(i);
                StoreProjectPaths(codeInfo, i);
            }
            gridControl2.RefreshDataSource();
            button1.Enabled = true;
        }

        void StoreProjectPaths(SourceCodeInfo sourceCodeInfo, int index) {
            var projectPaths = Directory.GetFiles(sourceCodeInfo.RootPath, "*.csproj", SearchOption.AllDirectories).Where(s => Regex.IsMatch(Path.GetFileName(s) + "", sourceCodeInfo.ProjectRegex));
            IEnumerable<string> paths = projectPaths.Select(s1 => s1 + "|" + GetOutPutPath(s1));
            var enumerable = paths as string[] ?? paths.ToArray();
            sourceCodeInfo.Count = enumerable.Count();
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

        private void gacUtilPathButtonEdit_ButtonClick(object sender, ButtonPressedEventArgs e) {
            ShowDialog(GacUtilPath);
        }

        private void modelEditorPathButtonEdit_ButtonClick(object sender, ButtonPressedEventArgs e) {
            ShowDialog(ModelEditorPath);
        }

        void ShowDialog(string modelEditorPath) {
            openFileDialog1.FileName = Storage.ReadString(PageName, modelEditorPath, modelEditorPathButtonEdit.Text);
            openFileDialog1.Tag = modelEditorPath;
            openFileDialog1.ShowDialog();
        }

        private void projectConverterPathButtonEdit_ButtonClick(object sender, ButtonPressedEventArgs e) {
            ShowDialog(ProjectConverterPath);
        }

        public static string ReadString(string key) {
            return Storage.ReadString(GetPageName(), key);
        }
    }
}