using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Forms;
using MSTSCLib;

namespace ProcessAsUser{
    public partial class Form1 : Form{

        public EventHandler<EventArgs> Connected; 

        public Form1(){


            InitializeComponent();
        }

        public bool Connect(string userName, string password){
            rdp.Server = Environment.MachineName;
            rdp.UserName = userName;
            var secured = (IMsTscNonScriptable)rdp.GetOcx();
            secured.ClearTextPassword = password;
            bool done = false;
            rdp.OnLoginComplete += (o, args) => {done = true;};
            rdp.Connect();
            var maxDuration = TimeSpan.FromSeconds(10);
            var stopwatch = Stopwatch.StartNew();
            while (!done&&stopwatch.Elapsed < maxDuration) {
                Application.DoEvents();
            }
            return done;
        }
        private void button1_Click(object sender, EventArgs e){
            try{
                rdp.Server = txtServer.Text;
                rdp.UserName = txtUserName.Text;

                var secured = (IMsTscNonScriptable) rdp.GetOcx();
                secured.ClearTextPassword = txtPassword.Text;
                rdp.Connect();
//                rdp.
            }
            catch (Exception ex){
                MessageBox.Show("Error Connecting",
                    "Error connecting to remote desktop " + txtServer.Text + " Error:  " + ex.Message,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e){
            try{
                // Check if connected before disconnecting
                if (rdp.Connected.ToString(CultureInfo.InvariantCulture) == "1")
                    rdp.Disconnect();
            }
            catch (Exception ex){
                MessageBox.Show("Error Disconnecting",
                    "Error disconnecting from remote desktop " + txtServer.Text + " Error:  " + ex.Message,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}