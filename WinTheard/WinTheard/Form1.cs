using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;


namespace WinTheard
{
    public partial class Form1 : Form
    {
        delegate void StringParameterDelegate(int count);
        public Form1()
        {
            InitializeComponent();
        }

        private void btnStartThread_Click(object sender, EventArgs e)
        {
            Thread workThread = new Thread(new ThreadStart(Counting));
            workThread.IsBackground = true;
            workThread.Start();
        }

        private void Counting()
        {
            for (int i = 0; i < 10000000; i++)
            {
                UpdateLabelCount(i);
                Thread.Sleep(100);
            }
        }

        private void UpdateLabelCount(int count)
        {
            if (InvokeRequired)
            {
                // We're not in the UI thread, so we need to call BeginInvoke
                BeginInvoke(new StringParameterDelegate(UpdateLabelCount), new object[] { count });
                return;
            }
            label1.Text = count.ToString();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
