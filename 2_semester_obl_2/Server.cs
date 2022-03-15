using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace LinuxThreadAssignment
{
    class Server
    {
        public string Hostname;
        private string UserName;
        private string Password;
        public int PortNumber;

        public DispatcherTimer Timer { get; private set; }

        public Server(string hostname, string username, string password, int portnumber)
        {
            this.Hostname = hostname;
            this.UserName = username;
            this.Password = password;
            this.PortNumber = portnumber;
        }

        //MotherMethod for inputting LinuxCommand and returning Value
        public string LinuxCommand(string linuxCommand)
        {
            //Bypass Keyboard authentication
            KeyboardInteractiveAuthenticationMethod kauth = new KeyboardInteractiveAuthenticationMethod(UserName);
            PasswordAuthenticationMethod pauth = new PasswordAuthenticationMethod(UserName, Password);
            kauth.AuthenticationPrompt += new EventHandler<Renci.SshNet.Common.AuthenticationPromptEventArgs>(HandleKeyEvent);

            //Grab info for connections
            ConnectionInfo connectionInfo = new ConnectionInfo(Hostname, PortNumber, UserName, pauth, kauth);

            //Connect
            using (SshClient client = new SshClient(connectionInfo))
            {
                //check if sshclient is connected to server
                try
                {
                    //Connect to server
                    client.Connect();

                    if (client.IsConnected)
                    {
                        //command to feed
                        var command = client.CreateCommand(linuxCommand);
                        var result = command.Execute();
                        command.Execute();

                        //write result to 'whatever'
                        return result.ToString();
                    }
                    else
                    {
                        return "N/A";
                    }
                }
                catch (Exception)
                {
                    return "N/A";
                }
            }
        }

        public string Get_OnlineStatus()
        {
            //Grab info for connections
            //Bypass Keyboard authentication
            KeyboardInteractiveAuthenticationMethod kauth = new KeyboardInteractiveAuthenticationMethod(UserName);
            PasswordAuthenticationMethod pauth = new PasswordAuthenticationMethod(UserName, Password);
            kauth.AuthenticationPrompt += new EventHandler<Renci.SshNet.Common.AuthenticationPromptEventArgs>(HandleKeyEvent);

            //Grab info for connections
            ConnectionInfo connectionInfo = new ConnectionInfo(Hostname, PortNumber, UserName, pauth, kauth);

            using (SshClient client = new SshClient(connectionInfo))
            {
                try
                {
                    //Connect to server
                    client.Connect();

                    //check if sshclient is connected to server
                    if (client.IsConnected)
                    {
                        return "Online";
                    }
                    else
                    {
                        return "Offline";
                    }
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }

        public string Get_Name()
        {
            string serverName = LinuxCommand("uname --nodename");
            return serverName;
        }

        public string Get_IP()
        {
            string serverIP = LinuxCommand("curl https://ipinfo.io/ip").ToString();
            return serverIP;
        }

        public string Get_OSinfo()
        {
            string OSinfo = LinuxCommand("lsb_release -a");
            return OSinfo;
        }

        public string Get_MemoryUse()
        {
            string memoryUse = LinuxCommand("free -m | grep \"Mem:\" | awk '{print $3 \" Mb used of \" $2 \"Mb\" }' ");
            return memoryUse;
        }

        public string Get_DiskUse()
        {
            string diskUse = LinuxCommand("df -hl | grep \"/dev/sd\\|/dev/vd\" | awk '{print $1 \"   \" $3 \" used of \" $2}' ");
            return diskUse;
        }

        public string Get_CpuUse()
        {
            string cpuUse = LinuxCommand("top - bn1 | grep \"Cpu(s)\" | sed \"s/.*, *\\([0-9.]*\\)%* id.*/\\1/\" | awk '{print 100 - $1\"%\"}'");
            return cpuUse;
        }

        public string Get_UpTime()
        {
            string upTime = LinuxCommand("uptime | awk '{print $3 \" min\"}'");
            return upTime;
        }

        public string Get_Processes()
        {
            string procceses = LinuxCommand("ps -aux");
            return procceses;
        }

        public string ServerStatus()
        {
            string serverStatus =
                 "\nName : \t" + Get_Name()
                + "IP : \t" + Get_IP();

            return serverStatus;
        }

        //EventHandler
        //If prompted for password, input password and press enter.
        public void HandleKeyEvent(object sender, Renci.SshNet.Common.AuthenticationPromptEventArgs e)
        {
            foreach (Renci.SshNet.Common.AuthenticationPrompt prompt in e.Prompts)
            {
                if (prompt.Request.IndexOf("Password:", StringComparison.InvariantCultureIgnoreCase) != -1)
                {
                    prompt.Response = Password;
                }
            }
        }
    }

}
