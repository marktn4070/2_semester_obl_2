using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using Renci.SshNet;
using System.Threading;

namespace _2_semester_obl_2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            Thread serverOne = new Thread(RunServer);
            Thread serverTwo = new Thread(RunServer);
            Thread serverThree = new Thread(RunServer);

            ConnectionInfo connectionInfoOne = new ConnectionInfo("192.168.0.103", "ja", new PasswordAuthenticationMethod("ja", "cle555"));
            serverOne.Start(connectionInfoOne);

            InitializeComponent();

        }
        static void RunServer(object ci) //skal ikke være her lol
        {
            ConnectionInfo c = (ConnectionInfo)ci;
            using (SshClient client = new SshClient(c))
            {
                try
                {
                    client.Connect();

                    if (client.IsConnected)
                    {
                        //command to feed
                        var command = client.CreateCommand(Console.ReadLine());
                        var result = command.Execute();
                        command.Execute();

                        //write result to 'whatever'
                        Console.WriteLine(result);
                    }
                }
                catch (Exception)
                {
                }
            }
        }
    }


}
