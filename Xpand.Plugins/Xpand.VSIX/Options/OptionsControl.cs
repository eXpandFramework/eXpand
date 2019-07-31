using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Grid;


namespace Xpand.VSIX.Options {
    public partial class OptionsControl : UserControl {
        public OptionsControl(){
            InitializeComponent();
            gridView1.KeyDown += GridViewOnKeyDown;
            gridView2.KeyDown += GridViewOnKeyDown;
            gridView4.KeyDown += GridViewOnKeyDown;
            gridView5.KeyDown += GridViewOnKeyDown;
            gridView6.KeyDown+=GridViewOnKeyDown;
            projectConverterPathButtonEdit.Text = OptionClass.Instance.ProjectConverterPath;
            testExecutorButtonEdit.Text = OptionClass.Instance.TestExecutorPath;
            publicTokenTextEdit.Text = OptionClass.Instance.Token;
            specificVersionCheckEdit.Checked = OptionClass.Instance.SpecificVersion;
            checkEditDebugME.Checked = OptionClass.Instance.DebugME;
            checkEditKillModelEditor.Checked = OptionClass.Instance.KillModelEditor;
            checkEditDisableExceptions.Checked = OptionClass.Instance.DisableExceptions;

            gridControlConnectionStrings.DataSource = OptionClass.Instance.ConnectionStrings;
            gridControlLoadProjectFromReferenceItem.DataSource = OptionClass.Instance.SourceCodeInfos;
            gridControlAssemblyFolders.DataSource = OptionClass.Instance.ReferencedAssembliesFolders;
            gridControlExceptions.DataSource = OptionClass.Instance.Exceptions;
            gridControlExternal.DataSource = OptionClass.Instance.ExternalTools;
            gridControlCmdBindings.DataSource = OptionClass.Instance.DteCommands;
            var collection = Enum.GetNames(typeof(DTEEvent)).Cast<object>().ToArray();
            repositoryItemComboBox1.Items.AddRange(collection);
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

            OptionClass.Instance.DteCommands = (BindingList<DteCommand>) gridControlCmdBindings.DataSource;
            OptionClass.Instance.ConnectionStrings = (BindingList<ConnectionString>) gridControlConnectionStrings.DataSource;
            OptionClass.Instance.SourceCodeInfos = (BindingList<SourceCodeInfo>) gridControlLoadProjectFromReferenceItem.DataSource;
            OptionClass.Instance.ReferencedAssembliesFolders = (BindingList<ReferencedAssembliesFolder>) gridControlAssemblyFolders.DataSource;
            OptionClass.Instance.Exceptions = (BindingList<ExceptionsBreak>) gridControlExceptions.DataSource;
            OptionClass.Instance.ExternalTools = (BindingList<ExternalTools>) gridControlExternal.DataSource;
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
            buttonSearchProjects.Enabled = false;
            var gridView = ((GridView)gridControlLoadProjectFromReferenceItem.MainView);
            for (int i = 0; i < gridView.RowCount; i++) {
                var codeInfo = (SourceCodeInfo)gridView.GetRow(i);
                codeInfo.AddProjectPaths();
            }
            gridControlLoadProjectFromReferenceItem.RefreshDataSource();
            buttonSearchProjects.Enabled = true;
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