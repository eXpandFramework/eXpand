using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.CodeRush.Core;
using DevExpress.DXCore.Controls.XtraEditors.Controls;
using Microsoft.Win32;
using System.Linq;

namespace eXpandAddIns
{
    [UserLevel(UserLevel.NewUser)]
    public partial class Options : OptionsPage {
        // DXCore-generated code...
        #region Initialize
        protected override void Initialize() {
            base.Initialize();

            DecoupledStorage storage = GetStorage();
            connectionStringName.Text = storage.ReadString(PageName, connectionStringName.Name, connectionStringName.Text);

            buttonEdit2.Text = storage.ReadString(PageName, "modelEditorPath", buttonEdit2.Text);
            buttonEdit1.Text = storage.ReadString(PageName, "projectConverterPath", buttonEdit1.Text);
            textEdit1.Text = storage.ReadString(PageName, "token", textEdit1.Text);
            openFileDialog1.FileName = storage.ReadString(PageName, "modelEditorPath", buttonEdit2.Text);
            openFileDialog2.FileName = storage.ReadString(PageName, "projectConverterPath", buttonEdit1.Text);
            string readString = storage.ReadString(PageName, "VsAssemblyPaths", "");
            gridControl1.DataSource =
                new BindingList<VsAssemblyPath>(
                    readString.Split(';').Where(s => !(string.IsNullOrEmpty(s))).Select(
                        s => new VsAssemblyPath {Name = s}).ToList());
            gridView1.KeyDown+=GridView1OnKeyDown;
            
        }

        private void GridView1OnKeyDown(object sender, KeyEventArgs args) {
            if (args.KeyCode==Keys.Delete) {
                ((IList)gridView1.DataSource).Remove(gridView1.GetRow(gridView1.FocusedRowHandle));
            }
        }


        public class VsAssemblyPath {
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

        private void buttonEdit1_ButtonClick(object sender, ButtonPressedEventArgs e) {
            openFileDialog1.ShowDialog();
        }


        private void openFileDialog1_FileOk(object sender, CancelEventArgs e) {
            buttonEdit2.Text = openFileDialog1.FileName;
        }

        private void Options_CommitChanges(object sender, CommitChangesEventArgs ea) {
            ea.Storage.WriteString(PageName, "token", textEdit1.Text);
            ea.Storage.WriteString(PageName, "modelEditorPath", buttonEdit2.Text);
            ea.Storage.WriteString(PageName, "projectConverterPath", buttonEdit1.Text);
            ea.Storage.WriteString(PageName, connectionStringName.Name, connectionStringName.Text);
            string paths = getPaths();
            ea.Storage.WriteString(PageName,"VsAssemblyPaths",paths);
            createPaths(paths);
        }

        private void createPaths(string paths) {
            RegistryKey localMachine = Registry.LocalMachine;
            RegistryKey registryKey = localMachine.OpenSubKey("SOFTWARE\\Microsoft\\.NETFramework\\AssemblyFolders",true);
            if (registryKey != null) {
                foreach (var value in getExpandKeys(registryKey)) {
                    registryKey.DeleteSubKey(value);    
                }
                string[] strings = paths.Split(';');
                for (int i = 0; i < strings.Length; i++) {
                    RegistryKey key = registryKey.CreateSubKey("eXpand" + i,RegistryKeyPermissionCheck.ReadWriteSubTree);
                    if (key != null) key.SetValue("", strings[i]);
                }
            }
        }

        private IEnumerable<string> getExpandKeys(RegistryKey registryKey) {
            return registryKey.GetSubKeyNames().Where(s => s.StartsWith("eXpand"));
        }

        private string getPaths() {
            string ret = "";
            for (int i = 0; i < gridControl1.MainView.RowCount; i++) {
                ret+= ((VsAssemblyPath) gridControl1.MainView.GetRow(i)).Name+";" ;
            }
            return ret.TrimEnd(';');
        }

        private void openFileDialog2_FileOk(object sender, CancelEventArgs e)
        {
            buttonEdit1.Text = openFileDialog2.FileName;
        }

        private void buttonEdit1_ButtonClick_1(object sender, ButtonPressedEventArgs e)
        {
            openFileDialog2.ShowDialog();
        }
    }
}