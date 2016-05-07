using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Threading;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Media;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;


namespace loveServer
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    /*public class dsp
    {
        void disp()
        {
            Dispatcher.CheckAccess();
        }
    }*/
    public class Email
    {
        public string id;
        public string header;
        public string from;
        public string to;
        public string message;
        public string date;

        public Email(string id, string header, string from, string to, string message, string date)
        {
            this.id = id;
            this.header = header;
            this.from = from;
            this.to = to;
            this.message = message;
            this.date = date;
        }
    }

    public partial class MainWindow : Window
    {
        //for first thread
        private static bool database = false;
        private static MySqlConnection con;
        //for second thread
        private static bool database2 = false;
        private static MySqlConnection con2;

        //private string st;
        delegate void UpSt(string st);

        delegate void UpSt2(string st);

        private string mikeyServerConnectionString =
            "server=176.32.230.43;uid=cl58-mikeyvv;pwd=JtXzgj4sz;database=cl58-mikeyvv;";

        private string naxServerConnectionString =
            "server=172.20.10.2;uid=bank;pwd=p@ssw0rd;database=mailbox;";

        public MainWindow()
        {
            InitializeComponent();
            GetToWork(); //Checking to keep database(first) alive.
            GetToWork2(); //Checking to keep database(second) alive.
            CallPush();
        }

        private void CallPush()
        {
            Thread sendWorkerThread = new Thread(new ThreadStart(PushEmail));
            sendWorkerThread.Start();
        }

        private void PushEmail()
        {
            while (true)
            {
                Queue colMailList = new Queue();
                MySqlDataReader reader;
                if (database && database2)
                {
                    if (con.State != ConnectionState.Open)
                    {
                        con.Open();
                        //continue;
                    }
                    string queryString = "SELECT * FROM `mailbox2`";
                    MySqlCommand cmd;
                    MySqlCommand cmd2;
                    MySqlCommand cmd3;
                    //Wait until the database is ready.
                    try
                    {
                        cmd = new MySqlCommand(queryString, con);
                        //cmd2 = new MySqlCommand(queryString, con);
                        reader = cmd.ExecuteReader(); //MikeyServer
                    }
                    catch (Exception)
                    {
                        continue; //skip this loop if database is not ready.
                    }

                    int num_rows = 0;
                    while (reader.Read())
                    {
                        num_rows++;
                        colMailList.Enqueue(new Email(reader.GetString("id").ToString(), reader.GetString("header"),
                            reader.GetString("from"), reader.GetString("to"), reader.GetString("message"),
                            reader.GetString("date")));
                    }
                    while (colMailList.Count > 0)
                    {
                        Email email = (Email) colMailList.Dequeue();
                        queryString = "INSERT INTO `mailbox`( `header`, `from`, `to`, `message`) VALUES ('" +
                                      email.header + "','" +
                                      email.from + "','" + email.to + "','" + email.message + "')";
                        
                        cmd2 = new MySqlCommand(queryString,con2);
                        if (con2.State == ConnectionState.Open) con2.Close();
                        if (con2.State != ConnectionState.Open) con2.Open();
                        cmd2.ExecuteNonQuery();
                        con2.Close();
  
                        queryString = "DELETE FROM `mailbox2` WHERE `id`=" + email.id;
                        
                        //if (con.State == ConnectionState.Open) con.Close();
                        cmd3 = new MySqlCommand(queryString, con);
                        if (con.State == ConnectionState.Open) con.Close();
                        if (con.State != ConnectionState.Open) con.Open();
                        cmd3.ExecuteNonQuery();
                        con.Close();
                    }
                    con.Close();
                }
                Thread.Sleep(2000);
            }
        }

        private void GetToWork()
        {
            Thread workThread = new Thread(new ParameterizedThreadStart(ThreadMethod_ConnectDB));
            workThread.IsBackground = true;
            workThread.Start(mikeyServerConnectionString);
        }

        private void GetToWork2()
        {
            Thread workThread2 = new Thread(new ParameterizedThreadStart(ThreadMethod_ConnectDB2));
            workThread2.IsBackground = true;
            workThread2.Start(naxServerConnectionString);
        }

        private void ThreadMethod_ConnectDB(object mikeyServerConnectionString)
        {
            con = new MySqlConnection((string) mikeyServerConnectionString);
            bool lost = false;
            while (true)
            {
                //   int i = 0;
                try
                {
                    //if (con.State != ConnectionState.Open) con.Open();
                    con.Open();
                    UpdateStatus(". . . Working . . .");
                    lost = false;
                    database = true;
                    while (!lost)
                    {
                        MySqlConnection isAliveConnection = new MySqlConnection();
                        isAliveConnection.Close();
                        isAliveConnection = new MySqlConnection((string) mikeyServerConnectionString);
                        try
                        {
                            isAliveConnection.Open();
                            lost = false;
                            isAliveConnection.Close();
                            //Thread.Sleep(1000);
                        }
                        catch (Exception)
                        {
                            lost = true;
                        }

                        /*UpdateStatus(con.State.ToString()+" "+i++);
                        Thread.Sleep(2000);
                        con.Open();*/
                    }
                    //con.Close();
                }
                catch (Exception)
                {
                    UpdateStatus("Connection Lost.");
                    //Thread.Sleep(1000);
                    UpdateStatus(". . . Connecting . . .");
                    con.Close();
                }
            }
        }

        private void ThreadMethod_ConnectDB2(object naxConnectionString)
        {
            con2 = new MySqlConnection((string) naxConnectionString);
            bool lost = false;
            while (true)
            {
                //   int i = 0;
                try
                {
                    //if (con2.State != ConnectionState.Open) con2.Open();
                    con2.Open();
                    UpdateStatus2(". . . Working . . .");
                    lost = false;
                    database2 = true;
                    while (!lost)
                    {
                        MySqlConnection isAliveConnection = new MySqlConnection();
                        isAliveConnection.Close();
                        isAliveConnection = new MySqlConnection((string) naxConnectionString);
                        try
                        {
                            isAliveConnection.Open();
                            lost = false;
                            isAliveConnection.Close();
                            //Thread.Sleep(1000);
                        }
                        catch (Exception)
                        {
                            lost = true;
                        }

                        /*UpdateStatus(con.State.ToString()+" "+i++);
                        Thread.Sleep(2000);
                        con.Open();*/
                    }
                    //con.Close();
                }
                catch (Exception)
                {
                    UpdateStatus2("Connection Lost.");
                    //Thread.Sleep(1000);
                    UpdateStatus2(". . . Connecting . . .");
                    con2.Close();
                }
            }
        }

        private void UpdateStatus(string st)
        {
            Color color = new Color();
            Color color2 = new Color();

            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.BeginInvoke(new UpSt(UpdateStatus), new object[] {st});
                return;
            }
            status.Content = st;
            /*if (st == ". . . Working . . .")
            {
                color = (Color) ColorConverter.ConvertFromString("#FF639FBB");
                color2 = (Color) ColorConverter.ConvertFromString("#FF00D8BB");
            }
            else if (st == "Connection Lost.")
            {
                color = (Color) ColorConverter.ConvertFromString("Black");
                color2 = (Color) ColorConverter.ConvertFromString("#FF767676");
            }
            else
            {
                color = (Color) ColorConverter.ConvertFromString("Black");
                color2 = (Color) ColorConverter.ConvertFromString("#FF3653A2");
            }
            G_1.Color = color;
            G_2.Color = color2;*/
        }

        private void UpdateStatus2(string st)
        {
            Color color = new Color();
            Color color2 = new Color();
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.BeginInvoke(new UpSt2(UpdateStatus2), new object[] {st});
                return;
            }
            status2.Content = st;
            //Dispatcher.BeginInvokeShutdown(DispatcherPriority.Normal);
            /*if (st == ". . . Working . . .")
            {
                color = (Color) ColorConverter.ConvertFromString("#FF639FBB");
                color2 = (Color) ColorConverter.ConvertFromString("#FF00D8BB");
            }
            else if (st == "Connection Lost.")
            {
                color = (Color) ColorConverter.ConvertFromString("Black");
                color2 = (Color) ColorConverter.ConvertFromString("#FF767676");
            }
            else
            {
                color = (Color) ColorConverter.ConvertFromString("Black");
                color2 = (Color) ColorConverter.ConvertFromString("#FF3653A2");
            }
            G_1.Color = color;
            G_2.Color = color2;*/
        }
    }
}