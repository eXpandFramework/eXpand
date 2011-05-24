using System;

namespace Xpand.ExpressApp.ImportWiz.Win.Forms
{
    public partial class ProgressForm : DevExpress.XtraEditors.XtraForm
    {
        public event EventHandler CancelClick = null;
        public ProgressForm()
        {
            InitializeComponent();
        }
        
        /// <summary>
        /// Constructor of the progress defining the initial parameters
        /// </summary>
        /// <param name="caption">Text that is displayed in the blue area on the top </param>
        /// <param name="recordCount">How many ? Mmm ?</param>
        /// <param name="message">For example "Precessing record {0} of {1}"</param>
        public ProgressForm(string caption, int recordCount, string message)
        {
            InitializeComponent();
            Text = caption;
            progress.Properties.Maximum = recordCount;
            _TotalRecords = recordCount;
            _MessageTemplate = message;

        }
        

        private string _MessageTemplate = string.Empty;
        private int _TotalRecords = 0;
        private int _Current = 0;


        private void SetLabelText()
        {
            labelControl1.Text = string.IsNullOrEmpty(_MessageTemplate) ?
                                    "" :
                                    string.Format(_MessageTemplate, _Current, _TotalRecords);
        }

        public override sealed string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (CancelClick == null) return;
            
            Close();
            CancelClick(sender, e);
        }

        public void DoProgress()
        {
            DoProgress(1);
        }

        public void DoProgress(int i)
        {
            _Current += i;
            progress.Increment(i);
            SetLabelText();
        }

        
    }
}