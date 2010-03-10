using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace TestCompactPlugs1.Plugins
{
    public class Plugin1
    {
        public Form1 WinForm { get; set; }
        
        private System.Windows.Forms.Label label1;
        const string lbltext1 = "Hello from Plugin 1!";
        const string lbltext2 = "Bye from Plugin 1!";
        Timer myTimer;
        
        public void RunPlugin1() 
        {
            label1 = new System.Windows.Forms.Label();
            this.label1.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular);
            this.label1.Location = new System.Drawing.Point(46, 136);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(151, 20);
            this.label1.Text = lbltext1;
            WinForm.Controls.Add(label1);
            RunTimer();
        }

        private void RunTimer() 
        { 
            myTimer = new Timer();
            myTimer.Tick += new EventHandler(myTimer_Tick);
            myTimer.Interval = 1000;
            myTimer.Enabled = true;
        }

        void myTimer_Tick(object sender, EventArgs e)
        {
            switch (label1.Text)
            {
                case lbltext1:
                    label1.Text = lbltext2;
                    break;
                case lbltext2:
                    label1.Text = lbltext1;
                    break;
                default:
                    break;
            }
        }

    }
}
