using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Xml.Linq;

namespace Client_lab4
{
    public partial class Form1 : Form
    {
        protected string Answer;
        string A;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string abc = textBox1.Text;
            A = Speaking(abc);
            XDocument xrequest = new XDocument();
            XElement Search_pattern = new XElement("Search_pattern");
            XAttribute patternAttr = new XAttribute("Pattern", abc);
            Search_pattern.Add(patternAttr);
            xrequest.Add(Search_pattern);
            xrequest.Save("Request-1.XML");
            label2.Text = A;
            WriteLog(A);
        }

        public string Speaking(string theMessage)
        {
            string F = File.ReadAllText(@"C:\Mine\Labs\Lab-4\Client_lab4\Client_lab4\bin\Debug\Request-1.xml");
            byte[] M = Encoding.UTF8.GetBytes(F);
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 49000);
            byte[] bytes = new byte[1000000];
            using (Socket S = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
            {
                S.Connect(ipEndPoint);
                S.Send(M);
                int bytesRec = S.Receive(bytes);
                Answer = Encoding.GetEncoding(1251).GetString(bytes, 0, bytesRec);
                S.Shutdown(SocketShutdown.Both);              
                return Answer;
            }
        }

        private static void WriteLog(string z)
        {
            using (StreamWriter F = new StreamWriter(@"C:\Mine\Labs\Lab-4\logs\log.txt", true))
            {
                F.WriteLine(DateTime.Now + z);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
