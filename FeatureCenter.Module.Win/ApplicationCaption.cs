using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using DevExpress.Utils.Frames;
using FeatureCenter.Module.Welcome;

namespace FeatureCenter.Module.Win {
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
                Stream manifestResourceStream = typeof(WelcomeObject).Assembly.GetManifestResourceStream("FeatureCenter.Module.Images.logo.png");
                if (manifestResourceStream != null) return Image.FromStream(manifestResourceStream);
                throw new NullReferenceException();
            }
        }
    }
}