using System;
using System.IO;
using System.Windows.Forms;
using Ionic.Zip;
using Xpand.ExpressApp.IO.Controllers;

namespace Xpand.ExpressApp.IO.Win.Controllers {
    public partial class IOViewController : IOViewControllerBase {
        public IOViewController() {
            InitializeComponent();
            RegisterActions(components);
        }

        protected string GetFilePath(bool isZipped) {
            var openFileDialog = new SaveFileDialog {
                CheckFileExists = false,
                AddExtension = true,
                Filter =isZipped?@"Zip files (*.zip)|*.zip": @"Xml files (*.xml)|*.xml"
            };
            var dialogResult = openFileDialog.ShowDialog();
            return dialogResult == DialogResult.OK ? openFileDialog.FileName : null;
        }

        protected override void Save(MemoryStream memoryStream) {
            var buffer = memoryStream.ToArray();
            Save(stream => stream.Write(buffer, 0, buffer.Length),false);
        }

        protected override void Save(ZipFile zipFile) {
            Save(zipFile.Save, true);
        }

        private void Save(Action<Stream> write, bool isZipped){
            var filePath = GetFilePath(isZipped);
            if (filePath != null){
                using (var fileStream = new FileStream(filePath, FileMode.Create)){
                    write(fileStream);
                }
            }
        }
    }
}
