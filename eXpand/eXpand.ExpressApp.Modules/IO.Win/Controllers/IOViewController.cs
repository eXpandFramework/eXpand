using System.Windows.Forms;
using eXpand.ExpressApp.IO.Controllers;

namespace eXpand.ExpressApp.IO.Win.Controllers
{
    public partial class IOViewController : IOViewControllerBase
    {
        public IOViewController()
        {
            InitializeComponent();
            RegisterActions(components);
        }

        protected override string GetFilePath() {
            var openFileDialog = new SaveFileDialog {CheckFileExists = false, AddExtension = true};
            var dialogResult = openFileDialog.ShowDialog();
            if (dialogResult==DialogResult.OK)
                return openFileDialog.FileName;
            return null;
        }
    }
}
