using System;
using System.Windows;
using System.Threading;

namespace loveServer
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static void ThreadMethod()
        {
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine("ThreadMethod - " + i);

                //Thread.Sleep(เวลาที่เราจะใส่[หน่วยเป็น millisecond]); คือการหยุดการทำงานใน Thread นั้นๆ
                Thread.Sleep(1);
            }
        }
        public MainWindow()
        {
            InitializeComponent();
            MessageBox.Show("Test2");
        }
    }
}