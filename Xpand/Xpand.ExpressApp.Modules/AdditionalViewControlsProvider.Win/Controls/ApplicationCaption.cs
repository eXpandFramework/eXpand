using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using DevExpress.Utils.Frames;
using Xpand.Persistent.Base;

namespace Xpand.ExpressApp.AdditionalViewControlsProvider.Win.Controls {
    [AdditionalViewControl]
    public class ApplicationCaption : ApplicationCaption8_1 {
        public ApplicationCaption() {

            Dock = DockStyle.Bottom;
            TabIndex = 0;
            TabStop = false;
            MinimumSize = new Size(350, 33);
            Visible = false;
        }

        protected override Image DXLogo {
            get {

                Stream manifestResourceStream = typeof(DifferenceType).Assembly.GetManifestResourceStream("Xpand.Persistent.Base.Resources.Logo.png");
                if (manifestResourceStream != null) return Image.FromStream(manifestResourceStream);
                throw new NullReferenceException();
            }
        }
    }
}