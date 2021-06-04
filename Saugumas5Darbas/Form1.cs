using SimpleTCP;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Saugumas5Darbas
{
    
    public static class Verify
    {
        
        public static void VerifyMessage(byte[] digitalSignature, string secureMessage)
        {
           
            // Recipient
            //get bytes of secureMessage
            byte[] messageHash = secureMessage.ComputeMessageHash();

            if (DigitalSignature.VerifySignedMessage(messageHash, digitalSignature))
            {
                Console.WriteLine($"Message '{secureMessage}' is valid and can be trusted.");

                //clear all data from file if it is any data there
                FileStream fileStream = File.Open(@"D:\Documents\Desktop\Saugumas5Darbas-master\Client\Text\lukas.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                fileStream.SetLength(0);
                fileStream.Close(); // This flushes the content, too.

                File.WriteAllText(@"D:\Documents\Desktop\Saugumas5Darbas-master\Client\Text\lukas.txt", secureMessage);

            }
            else
            {
                Console.WriteLine($"The following message: '{secureMessage}' is not valid. DO NOT TRUST THIS MESSAGE!");
            }
        }
    }

    public partial class Form1 : Form
    {
        public static bool verified;
        public static string verifiedMessage;
        SimpleTcpServer server;
        public Form1()
        {
            InitializeComponent();
}

        
        private void Form1_Load(object sender, EventArgs e)
        {
            //creating new Simple server
            server = new SimpleTcpServer();
            server.Delimiter = 0x13;//enter
            server.StringEncoder = Encoding.UTF8;
            server.DataReceived += Server_DataReceived;

        }

        private void Server_DataReceived(object sender, SimpleTCP.Message e)
        {
            txtStatus.Invoke((MethodInvoker)delegate ()
            {

                string clientMes = e.MessageString;
                if(clientMes != String.Empty)
                {
                    string line;
                    using (StreamReader reader = new StreamReader(@"D:\Documents\Desktop\Saugumas5Darbas-master\Client\Text\lukas.txt"))
                    {
                        line = reader.ReadLine();
                    }
                    txtStatus.Text = line;

                }
            });
        }

        //START SERVER
        private void buttonbtnStart_Click(object sender, EventArgs e)
        {
            txtStatus.Text += "Server starting ...";
            System.Net.IPAddress ip = System.Net.IPAddress.Parse(txtHost.Text);
            //start server at decrared IP and PORT 
            server.Start(ip, Convert.ToInt32(txtPort.Text));

        }

        //STOP SERVER
        private void btnStop_Click(object sender, EventArgs e)
        {
            //to stop server if its started
            if (server.IsStarted)
            {
                server.Stop();
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            

            //DigitalSignature.ContainerName = "KeyContainer";

            // Sender

            string secureMessage = $"Transfer $500 into account number 029192819283 on {DateTime.Now}";

            byte[] digitalSignature = DigitalSignature.SignMessage(secureMessage);

            // Message intercepted

            //secureMessage = $"Transfer $5000 into account number 849351278435 on {DateTime.Now}";

            // Recipient
            //get bytes of secureMessage
            byte[] messageHash = secureMessage.ComputeMessageHash();

            if (DigitalSignature.VerifySignedMessage(messageHash, digitalSignature))
            {
                Console.WriteLine($"Message '{secureMessage}' is valid and can be trusted.");
            }
            else
            {
                Console.WriteLine($"The following message: '{secureMessage}' is not valid. DO NOT TRUST THIS MESSAGE!");
            }
                


        }
    }
}
