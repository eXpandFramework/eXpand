using System.IO;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using Xpand.ExpressApp.IO.Controllers;
using Xpand.ExpressApp.IO.Core;

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
                Filter = @"Xml files (*.xml)|*.xml"
            };
            var dialogResult = openFileDialog.ShowDialog();
            return dialogResult == DialogResult.OK ? openFileDialog.FileName : null;
        }

        protected override void Save(XDocument document) {
            var filePath = GetFilePath();
            if (filePath != null) {
                var minifyOutput = document.IsMinified();
                var fileStream = new FileStream(filePath, FileMode.Create);
                using (var textWriter = XmlWriter.Create(fileStream, ExportEngine.GetXMLWriterSettings(minifyOutput))) {
                    document.Save(textWriter);
                    textWriter.Close();
                }
                
            }
        }
    }
}
