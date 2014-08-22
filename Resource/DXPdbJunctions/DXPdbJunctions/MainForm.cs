using System.Windows.Forms;

namespace DXPdbJunctions {
    public partial class MainForm : Form {
        private static MainForm _instance;

        public MainForm() {
            InitializeComponent();
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.DataSource = Logger.Instance.Text;
            comboBox1.DataSource = ApplicationSettings.Instance.DXperienceInstalled;
            comboBox1.DisplayMember = "Version";
            comboBox1.DataBindings.Add("SelectedItem", ApplicationSettings.Instance, "DXperience");
        }

        public static MainForm Instance {
            get { return _instance ?? (_instance = new MainForm()); }
        }

        public void UpdateLog(TextItem text) {
            dataGridView1.Refresh();
            dataGridView1.FirstDisplayedScrollingRowIndex = dataGridView1.RowCount - 1;
            dataGridView1.AutoResizeRows(DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders);
            Application.DoEvents();
        }


        internal void SetMaxProgressValue(int maxProgressValue) {
            progressBar1.Maximum = maxProgressValue;
            Application.DoEvents();
        }

        internal void SetProgressValue(int progressValue) {
            progressBar1.Value = progressValue;
            Application.DoEvents();
        }

        public void Run() {
            if (!ApplicationSettings.Instance.CommandPromptMode)
                Instance.ShowDialog();
        }

        private void button2_Click(object sender, System.EventArgs e) {
            ApplicationWorkflow.Instance.CreateProcess((DXperience) comboBox1.SelectedItem);
        }
    }
}
