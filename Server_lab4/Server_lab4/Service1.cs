using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Xml.Linq;
using System.Management;
using Microsoft.Win32;
using System.Text.RegularExpressions;

namespace Server_lab4
{
    public partial class Service1 : ServiceBase
    {
        RegistryHive h = RegistryHive.LocalMachine;
        int port = 49000;
        Thread Thread1;
        bool mustStop;
        public static string answer;
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Thread1 = new Thread(WorkerThread);
            Thread1.Start();
        }

        protected override void OnStop()
        {
            if ((Thread1 != null) && (Thread1.IsAlive))
            {
                mustStop = true;
            }
        }

        void WorkerThread()
        {
            while (!mustStop)
            {
                IPAddress IP = IPAddress.Parse("192.168.1.106");
                IPEndPoint ipEndPoint = new IPEndPoint(IP, port);
                Socket S = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                try
                {
                    S.Bind(ipEndPoint);
                    while (true)
                    {
                        IPEndPoint L = new IPEndPoint(IP, 0);
                        EndPoint R = (EndPoint)(L);
                        byte[] D = new byte[1000000];
                        int Receive = S.ReceiveFrom(D, ref R);
                        string Request = Unpack(Encoding.GetEncoding(1251).GetString(D, 0, Receive));
                        XDocument request_1 = new XDocument();
                        request_1 = MakeXML(Request);
                        if (!File.Exists(@"C:\Mine\Labs\laba_4\logs_server\Request-1.xml"))
                        {
                            request_1.Save(@"C:\Mine\Labs\laba_4\logs_server\Request-1.xml");
                        }
                        else
                        {
                            request_1.Save(@"C:\Mine\Labs\laba_4\logs_server\Request-2.xml");
                        }
                        XDocument response;
                        response = Make_List(GetSurnamesByFilter(Request));
                        answer = response.ToString();
                        if (!File.Exists(@"C:\Mine\Labs\laba_4\logs_server\Response-1.xml"))
                        {
                            response.Save(@"C:\Mine\Labs\laba_4\logs_server\Response-1.xml");
                        }
                        else
                        {
                            response.Save(@"C:\Mine\Labs\laba_4\logs_server\Response-2.xml");
                        }

                        if (Request.Length > 4)
                        {
                            byte[] M = Encoding.GetEncoding(1251).GetBytes(answer);
                            S.SendTo(M, R);
                        }
                        else
                        {
                            byte[] M = Encoding.GetEncoding(1251).GetBytes(answer);
                            S.SendTo(M, R);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        static string[] GetSurnamesByFilter(string d)
        {
            var keyvalues = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).
                           OpenSubKey(@"Software\udpparameters").GetValueNames();
            var keyvalues1 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).
                            OpenSubKey(@"Software\udpparameters").GetValueNames();

            for (int i = 0; i < keyvalues.Length; i++)
            {
                keyvalues[i] = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).
                            OpenSubKey(@"Software\udpparameters").GetValue(keyvalues[i]).
                            ToString().Substring(0, RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).
                            OpenSubKey(@"Software\udpparameters").GetValue(keyvalues[i]).
                            ToString().IndexOf(' '));
                keyvalues1[i] = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).
                           OpenSubKey(@"Software\udpparameters").GetValue(keyvalues1[i]).ToString();
            }
            if (d.StartsWith("*"))
            {
                if (d == "*")
                {
                    return keyvalues;
                }
                else
                {
                    d = d.Substring(1);
                    var result = new List<string>();
                    int k = 0;
                    for (int i = 0; i < keyvalues.Length; ++i)
                    {
                        if (keyvalues[i].EndsWith(d)) { result.Add((keyvalues[i])); ++k; }
                    }
                    return result.ToArray();

                }
            }
            else if (d.EndsWith("*"))
            {
                string g = "";
                foreach (char ch in d)
                {
                    string str = ch.ToString();
                    if (Regex.IsMatch(str, @"[а-я]$", RegexOptions.IgnoreCase))
                    {
                        g += str;
                    }
                }
                string[] result = new string[1000];
                int k = 0;
                for (int i = 0; i < keyvalues.Length; ++i)
                {
                    if (keyvalues[i].StartsWith(g)) { result[k] = (keyvalues[i]); k++; }
                }
                return result;
            }
            else
            {
                string[] result = new string[1000];
                int k = 0;
                for (int i = 0; i < keyvalues.Length; ++i)
                {
                    if (keyvalues[i] == d) result[k++] = (keyvalues1[i]);
                }
                return result;
            }
        }
        XDocument Make_List(string[] a)
        {
            var doc = new XDocument();
            var el = new XElement("list");
            for (int i = 0; i < a.Length; i++)
            {
                el.Add(new XElement("students", a[i]));
            }
            doc.Add(el);
            return doc;
        }
        string Unpack(string g)
        {
            var t = XDocument.Parse(g);
            return t.Element("filt").Value;
        }
        XDocument MakeXML(string name)
        {
            XDocument doc = new XDocument();

            XElement f = new XElement("students", name);
            doc.Add(f);
            return doc;
        }
    }
}
