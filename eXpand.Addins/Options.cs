using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.CodeRush.Core;
using DevExpress.DXCore.Controls.XtraEditors.Controls;
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
            

            buttonEdit2.Text = storage.ReadString(PageName, "modelEditorPath", buttonEdit2.Text);
            buttonEdit1.Text = storage.ReadString(PageName, "projectConverterPath", buttonEdit1.Text);
            textEdit1.Text = storage.ReadString(PageName, "token", textEdit1.Text);
            openFileDialog1.FileName = storage.ReadString(PageName, "modelEditorPath", buttonEdit2.Text);
            openFileDialog2.FileName = storage.ReadString(PageName, "projectConverterPath", buttonEdit1.Text);
            
            gridControl1.DataSource =GetConnectionStrings(storage);
            gridView1.KeyDown+=GridView1OnKeyDown;
            
        }

        public static BindingList<ConnectionString> GetConnectionStrings(DecoupledStorage storage) {
            string readString = storage.ReadString(GetPageName(), "ConnectionStrings", "");
            return new BindingList<ConnectionString>(
                readString.Split(';').Where(s => !(string.IsNullOrEmpty(s))).Select(
                    s => new ConnectionString {Name = s}).ToList());
        }

        private void GridView1OnKeyDown(object sender, KeyEventArgs args) {
            if (args.KeyCode==Keys.Delete) {
                ((IList)gridView1.DataSource).Remove(gridView1.GetRow(gridView1.FocusedRowHandle));
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
            string connectionStrings = ((BindingList<ConnectionString>) gridControl1.DataSource).Aggregate<ConnectionString, string>(null, (current, connectionString) => current + (connectionString.Name + ";"));
            ea.Storage.WriteString(PageName, "ConnectionStrings", connectionStrings);
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