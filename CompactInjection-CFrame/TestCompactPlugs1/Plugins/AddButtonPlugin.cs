using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace TestCompactPlugs1.Plugins
{
    public class AddButtonPlugin
    {

        public Form1 WinForm { get; set; }
        public AddButtonPlugin()
        {

        }

        public void Run() 
        {
            System.Windows.Forms.Button button1 = new System.Windows.Forms.Button();
            // 
            // button1
            // 
            button1.Location = new System.Drawing.Point(75, 38);
            button1.Name = "button1";
            button1.Size = new System.Drawing.Size(72, 20);
            button1.TabIndex = 0;
            button1.Text = "Plugin Added!";
            WinForm.Controls.Add(button1);
        }

    }
}
