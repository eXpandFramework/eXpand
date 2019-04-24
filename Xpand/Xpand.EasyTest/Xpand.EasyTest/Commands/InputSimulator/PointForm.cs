using System;
using System.Drawing;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace Xpand.EasyTest.Commands.InputSimulator {
    public partial class PointForm : Form {
        PointForm() {
            InitializeComponent();
            Load+=OnLoad;
        }

        public static void Show(Point location){
            var pointForm = new PointForm{Location = location};
            pointForm.ShowDialog();
        }

        private void OnLoad(object sender, EventArgs eventArgs){
            Width = 10;
            Height = 10;
            var timer = new Timer{Interval = 250};
            timer.Tick += (o, args) =>{
                if (BackColor!=Color.Red){
                    BackColor=Color.Red;
                    return;
                }
                Close();
            };
            timer.Start();
        }

    }
}
