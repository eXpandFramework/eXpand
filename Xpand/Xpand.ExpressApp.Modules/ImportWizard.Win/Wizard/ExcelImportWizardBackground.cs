using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Xpand.ExpressApp.ImportWizard.Core;
using Xpand.ExpressApp.ImportWizard.Win.Properties;

namespace Xpand.ExpressApp.ImportWizard.Win.Wizard{
    class WorkerArgs {
        private readonly IEnumerable<Row> _rows;
        private readonly int _headerRows;

        public WorkerArgs(IEnumerable<Row> rows, int? headerRows){
            _rows = rows;
            if (headerRows != null) _headerRows = headerRows.Value;
        }

        public IEnumerable<Row> Rows {
            get { return _rows; }
        }

        public int HeaderRows {
            get { return _headerRows; }
        }
    }

    public partial class ExcelImportWizard{
        private void BgWorkerDoWork(object sender, DoWorkEventArgs e){
            ProccesExcellRows( ObjectSpace, e, Type);
        }

        private void BgWorkerProgressChanged(object sender, ProgressChangedEventArgs e){
            Application.DoEvents();
            if (_frmProgress != null)
                _frmProgress.DoProgress(e.ProgressPercentage);

            SetMemoText(e.UserState.ToString());
            Application.DoEvents();
        }

        private void BgWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e){
            _frmProgress.Close();

            if (e.Cancelled){
                ObjectSpace.Rollback();
                XtraMessageBox.Show(Resources.ExcelImportWizard_BgWorkerRunWorkerCompleted_The_task_has_been_cancelled,
                    Resources.ExcelImportWizard_BgWorkerRunWorkerCompleted_Work_Canceled, MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
            }
            else if (e.Error != null){
                ObjectSpace.Rollback();
                XtraMessageBox.Show(Resources.ExcelImportWizard_BgWorkerRunWorkerCompleted_Error__Details__ + e.Error,
                    Resources.ExcelImportWizard_BgWorkerRunWorkerCompleted_Error, MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            else{
                ObjectSpace.CommitChanges();
                XtraMessageBox.Show(
                    Resources.ExcelImportWizard_BgWorkerRunWorkerCompleted_The_task_has_been_completed__Results__ +
                    e.Result);
            }

            WizardControl.SelectedPage = completionWizardPage1;
        }

        #region Progress Events

        public void SetMemoText(string text){
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (ResultsMemoEdit.InvokeRequired){
                var d = new SetMemoTextDelegate(SetMemoText);
                Invoke(d, new object[]{text});
            }
            else{
                ResultsMemoEdit.Text += Environment.NewLine + text;
                ResultsMemoEdit.Select(ResultsMemoEdit.Text.Length,
                    ResultsMemoEdit.Text.Length);
                ResultsMemoEdit.ScrollToCaret();
            }
            Application.DoEvents();
        }

        private void FrmProgressCancelClick(object sender, EventArgs e){
            _bgWorker.CancelAsync();
        }

        private delegate void SetMemoTextDelegate(string text);

        #endregion
    }
}