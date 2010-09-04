using System.Windows.Forms;
using Xpand.ExpressApp.IO.Controllers;

namespace Xpand.ExpressApp.IO.Win.Controllers
{
    public partial class IOViewController : IOViewControllerBase
    {
        public IOViewController()
        {
            InitializeComponent();
            RegisterActions(components);
        }

        protected override string GetFilePath() {
            var openFileDialog = new SaveFileDialog {
                                                        CheckFileExists = false,
                                                        AddExtension = true,
                                                        Filter = "Xml files (*.xml)|*.xml"
                                                    };
            var dialogResult = openFileDialog.ShowDialog();
            if (dialogResult==DialogResult.OK)
                return openFileDialog.FileName;
            return null;
        }
    }
}
