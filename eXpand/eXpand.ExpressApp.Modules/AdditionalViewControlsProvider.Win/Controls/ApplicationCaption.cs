using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using DevExpress.Utils.Frames;

namespace eXpand.ExpressApp.AdditionalViewControlsProvider.Win.Controls {
    [AdditionalViewControl]
    public class ApplicationCaption : ApplicationCaption8_1
    {
        public ApplicationCaption() {
            
            Dock = DockStyle.Bottom;
            TabIndex = 0;
            TabStop = false;
            MinimumSize = new Size(350, 33);
            Visible = false;
        }

        protected override Image DXLogo {
            get {

                Stream manifestResourceStream = typeof(eXpand.Persistent.Base.DifferenceType).Assembly.GetManifestResourceStream("eXpand.Persistent.Base.Resources.Logo.png");
                if (manifestResourceStream != null) return Image.FromStream(manifestResourceStream);
                throw new NullReferenceException();
            }
        }
    }
}