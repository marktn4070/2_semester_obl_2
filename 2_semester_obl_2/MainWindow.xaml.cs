using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Net.NetworkInformation;

namespace LinuxThreadAssignment
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        //IP, Name, Password, Port
        Server server1 = new Server("217.61.221.81", "edd101", "lmao1337", 7777);
        Server daniaServer = new Server("45.76.43.221", "root", "j}7E(Ma38tMg8_ux", 22);
        private DispatcherTimer Timer;

        public MainWindow()
        {
            InitializeComponent();
        }

        //bigboi addservermethod 
        private void ADDNEWSERVER(string hostnameIP, string username, string password, int port)
        {
            Server server = new Server(hostnameIP, username, password, port);

            Thread thread1 = new Thread(() => AddServerToList
            (
                server.ServerStatus()
            ));
            thread1.Start();

            void AddServerToList
                (
                string serverStatus
                )
            {
                this.Dispatcher.Invoke(() =>
                {

                    // Create Border for button
                    Border myBorder = new Border();
                    myBorder.BorderBrush = Brushes.Black;
                    myBorder.Margin = new Thickness(0, 5, 0, 0);
                    myBorder.BorderThickness = new Thickness(1);

                    //create main 'server' button
                    Button button = new Button();
                    button.Margin = new Thickness(5);
                    button.Width = 360;
                    button.FontSize = 12;




                    //////////////////////////////////////////////PING
                    Timer = new DispatcherTimer();
                    Timer.Interval = new TimeSpan(0, 0, 1);
                    Timer.Tick += Timer_Tick2;
                    Timer.Start();

                    void Timer_Tick2(object senderZ, EventArgs eZ)
                    {
                        Ping ping = new Ping();
                        try
                        {
                            PingReply reply = ping.Send(hostnameIP);
                            if (reply.Status == IPStatus.Success)
                            {
                                button.Content = "Status : Online" + serverStatus;
                            }
                            else
                            {
                                button.Content = "Status : Offline" + serverStatus;
                            }
                        }
                        catch (Exception ex)
                        {
                            button.Content = "Status : Offline" + serverStatus;
                        }
                    }

                    //button content;
                    //button.Content = serverStatus;
                    button.Content = serverStatus;


                    //t00ltipproc();
                    //async void t00ltipproc()
                    //{
                    //    while (true) button.tooltip = await rtu_processes();
                    //}

                    //Task<string> RTU_Processes() { return Task.Run(() => { Thread.Sleep(1000); return server.Get_Processes(); }); }

                    //EVENTHANDLER FOR BUTTON/SERVER CLICK

                    button.AddHandler(Button.ClickEvent, new RoutedEventHandler(Click_ServerInfo));




                    myBorder.Child = button;


                    //Add button to Server List StackPanel
                    ServerListStackPanel.Children.Add(myBorder);

                    void Click_ServerInfo(object sender, RoutedEventArgs e)
                    {

                        {
                            StatWindow statwindow = new StatWindow();
                            statwindow.Show();

                            bool isWindowOpen;
                            isWindowOpen = true;

                            statwindow.SERVERINFOTOP.Text = server.Get_Name();


                            //REALTIME UPDATE STATWINDOW                                                                               
                            RTU_UpTime();
                            async void RTU_UpTime()
                            {
                                while (isWindowOpen == true) statwindow.UpTimeDisplay.Text = await RTU_Get_UpTime();
                                Task<string> RTU_Get_UpTime() { return Task.Run(() => { Thread.Sleep(100); return server.Get_UpTime(); }); }
                            }

                            RTU_MemUse();
                            async void RTU_MemUse()
                            {
                                while (isWindowOpen == true) statwindow.MemUseDisplay.Text = await RTU_Get_MemUse();
                            }
                            Task<string> RTU_Get_MemUse() { return Task.Run(() => { Thread.Sleep(100); return server.Get_MemoryUse(); }); }

                            RTU_DiskUse();
                            async void RTU_DiskUse()
                            {
                                while (isWindowOpen == true) statwindow.DiskUseDisplay.Text = await RTU_Get_DiskUse();
                            }
                            Task<string> RTU_Get_DiskUse() { return Task.Run(() => { Thread.Sleep(100); return server.Get_DiskUse(); }); }

                            RTU_CpuUse();
                            async void RTU_CpuUse()
                            {
                                while (isWindowOpen == true) statwindow.CPUUseDisplay.Text = await RTU_Get_CpuUse();
                            }
                            Task<string> RTU_Get_CpuUse() { return Task.Run(() => { Thread.Sleep(100); return server.Get_CpuUse(); }); }


                            //////////////////////////////////////////////PING
                            Timer = new DispatcherTimer();
                            Timer.Interval = new TimeSpan(0, 0, 1);
                            Timer.Tick += Timer_Tick;
                            Timer.Start();

                            void Timer_Tick(object senderZ, EventArgs eZ)
                            {
                                Ping ping = new Ping();
                                try
                                {
                                    PingReply reply = ping.Send(server.Hostname);
                                    if (reply.Status == IPStatus.Success)
                                    {
                                        statwindow.PingDisplay.Text = "Online: " + reply.RoundtripTime;
                                    }
                                    else
                                    {
                                        statwindow.PingDisplay.Text = "Offline: " + reply.Status.ToString();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    statwindow.PingDisplay.Text = "Offline: " + ex.Message;
                                }
                            }
                        }
                    }
                });
            }
        }

        private void BUTTON_Add_server(object sender, RoutedEventArgs e)
        {
            ToggleViews();
        }

        private void Add(object sender, RoutedEventArgs e)
        {
            try
            {
                ToggleViews();

                ADDNEWSERVER(addnew_HostnameIP.Text, addnew_User.Text, addnew_Password.Password.ToString(), Convert.ToInt32(addnew_Port.Text));

            }
            catch (Exception ex)
            {
                MessageBox.Show("Can't connect to server. Please check input. Server has to be online when adding it to the list.\n\n" + ex.ToString());
            }
        }

        private void GoToMainPage(object sender, RoutedEventArgs e)
        {
            ToggleViews();
        }

        private void ToggleViews()
        {
            if (ServerListStackPanel.Visibility == Visibility.Visible)
            {
                ServerListStackPanel.Visibility = Visibility.Collapsed;
                Mainpagebutton_Add_New_Server.Visibility = Visibility.Collapsed;
                Mainpagebutton_Add_New_Server.Visibility = Visibility.Hidden;
                AddNewServerStackPanel.Visibility = Visibility.Visible;
            }
            else
            {
                ServerListStackPanel.Visibility = Visibility.Visible;
                Mainpagebutton_Add_New_Server.Visibility = Visibility.Visible;
                Mainpagebutton_Add_New_Server.Visibility = Visibility.Visible;
                AddNewServerStackPanel.Visibility = Visibility.Collapsed;
            }
        }

        private void aPicture_MouseDown(object sender, MouseEventArgs e)
        {
            ToggleViews();
        }
    }
}
