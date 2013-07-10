using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using Xpand.ExpressApp.IO.Controllers;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.PersistentMetaData;

namespace Xpand.ExpressApp.IO.Win.Controllers {
    public partial class IOViewController : IOViewControllerBase {
        public IOViewController() {
            InitializeComponent();
            RegisterActions(components);
        }

        protected string GetFilePath() {
            var openFileDialog = new SaveFileDialog {
                CheckFileExists = false,
                AddExtension = true,
                Filter = "Xml files (*.xml)|*.xml"
            };
            var dialogResult = openFileDialog.ShowDialog();
            return dialogResult == DialogResult.OK ? openFileDialog.FileName : null;
        }

        protected override void Import(DevExpress.ExpressApp.Actions.SingleChoiceActionExecuteEventArgs singleChoiceActionExecuteEventArgs) {
            base.Import(singleChoiceActionExecuteEventArgs);
            var confirmationRequired = ((IConfirmationRequired)Application);
            singleChoiceActionExecuteEventArgs.ShowViewParameters.CreatedView.Closed += (sender, eventArgs) => confirmationRequired.ConfirmationRequired -= OnConfirmationRequired;
            confirmationRequired.ConfirmationRequired += OnConfirmationRequired;
        }
        void OnConfirmationRequired(object sender, CancelEventArgs cancelEventArgs) {
            cancelEventArgs.Cancel = true;
        }

        protected override void Save(XDocument document) {
            var filePath = GetFilePath();
            if (filePath != null) {
                var xmlWriterSettings = new XmlWriterSettings {
                    OmitXmlDeclaration = true, Indent = true, NewLineChars = "\r\n", CloseOutput = true,
                };
                using (XmlWriter textWriter = XmlWriter.Create(new FileStream(filePath, FileMode.Create), xmlWriterSettings)) {
                    document.Save(textWriter);
                    textWriter.Close();
                }
            }
        }
    }
}
