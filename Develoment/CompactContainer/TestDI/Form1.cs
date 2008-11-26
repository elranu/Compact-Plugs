using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;

//Develop by Mariano Julio Vicario
//http://www.codeplex.com/compactcontainer/
//http://www.ranu.com.ar (Blog)
//Microsoft Public License (Ms-PL)

namespace TestDI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            string dir =Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName);
            string file = Path.Combine(dir, "Config.xml");
            CompactContainer.CompactConstructor.DefaultConstructor.AddDefinitions(file);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            DemoPresenter p = CompactContainer.CompactConstructor.DefaultConstructor.New<DemoPresenter>("Demo");
            textBox1.Text = p.Something.DoSomething();
            textBox1.Text += " - "+p.Inte.ToString();
            textBox1.Text += " - " + p.Str;
            textBox1.Text += " - " + p.Obj2.Inte2.ToString();

            string dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName);
            string file = Path.Combine(dir, "Config2.xml");
            CompactContainer.CompactConstructor.DefaultConstructor.AddDefinitions(file);
            DemoPresenter p2 = CompactContainer.CompactConstructor.DefaultConstructor.New<DemoPresenter>("Demo2");
            textBox1.Text += "  ----- " +p2.Something.DoSomething();
            textBox1.Text += " - " + p2.Inte.ToString();
            textBox1.Text += " - " + p2.Str;
            textBox1.Text += " - " + p2.Obj2.Inte2.ToString();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}