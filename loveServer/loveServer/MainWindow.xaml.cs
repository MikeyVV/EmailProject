using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MySql.Data.MySqlClient;

namespace loveServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    
    

    public partial class MainWindow : Window
    {
        public delegate void delUpdateUITextBox(string text);

        ThreadStart threadStart;
        Thread myUpdateThread;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, EventArgs e)
        {
            threadStart = new ThreadStart(GetTheThreadStarted);
            myUpdateThread = new Thread(threadStart);
            myUpdateThread.Name = "Seconed Thread";
            myUpdateThread.Start();
        }

        private void GetTheThreadStarted()
        {
            delUpdateUITextBox DelUpdateUITextBox = new delUpdateUITextBox(UpdateUITeaxtBox);
            this.BeginInvoke(DelUpdateUITextBox, "I was update" + myUpdateThread.Name);
        }

        public void UpdateUITeaxtBox(string textBoxString)
        {
            this.status.Content = textBoxString;
        }
    }
}