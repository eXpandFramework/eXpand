using System;

namespace Xpand.ExpressApp.ImportWizard.Win.Forms {
    public partial class ProgressForm : DevExpress.XtraEditors.XtraForm {
        public event EventHandler CancelClick;
        public ProgressForm() {
            InitializeComponent();
        }

        /// <summary>
        /// Constructor of the progress defining the initial parameters
        /// </summary>
        /// <param name="caption">Text that is displayed in the blue area on the top </param>
        /// <param name="recordCount">How many ? Mmm ?</param>
        /// <param name="message">For example "Precessing record {0} of {1}"</param>
        public ProgressForm(string caption, int recordCount, string message) {
            InitializeComponent();
            Text = caption;
            progress.Properties.Maximum = recordCount;
            _totalRecords = recordCount;
            _messageTemplate = message;

        }


        private readonly string _messageTemplate = string.Empty;
        private readonly int _totalRecords;
        private int _current;


        private void SetLabelText() {
            labelControl1.Text = string.IsNullOrEmpty(_messageTemplate) ?
                                    "" :
                                    string.Format(_messageTemplate, _current, _totalRecords);
        }

        public sealed override string Text {
            get { return base.Text; }
            set { base.Text = value; }
        }

        private void btnCancel_Click(object sender, EventArgs e) {
            if (CancelClick == null) return;

            Close();
            CancelClick?.Invoke(sender, e);
        }

        public void DoProgress() {
            DoProgress(1);
        }

        public void DoProgress(int i) {
            _current += i;
            progress.Increment(i);
            SetLabelText();
        }


    }
}