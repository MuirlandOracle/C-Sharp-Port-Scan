using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace Port_Scanner
{
    class Program
    {
        //Check for an open port
        static void openPort(IPAddress ipAddr, int port)
        {
            IPEndPoint remoteEP = new IPEndPoint(ipAddr, port);
            Socket sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                sender.Connect(remoteEP);
                sender.Shutdown(SocketShutdown.Both);
                sender.Close();
                Console.WriteLine($"[+] Port {port} is open");
            }
            catch (SocketException e)
            {
                return;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Unexpected Exception: {e}");
            }
        }

        static void Main(string[] args)
        {
            //Check for help request
            if(args.Contains("-h") || args.Contains("--help"))
            {
                Console.WriteLine($"Syntax: {AppDomain.CurrentDomain.FriendlyName} IP PORT1 PORT2,PORT3 PORT-PORT");
                return;
            }

            //Variable initialisation
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            int threads = 0;
            int maxThreads = 2000;
            List<Task> tasks = new List<Task>();
            var ports = new List<int>();

            //Simple Argparsing -- needs an upgarde at some point
            foreach (string i in args.Skip(1))
            {
                //Check for comma separated ports
                if(i.Contains(",")){
                    foreach(string j in i.Split(','))
                    {
                        try
                        {
                            ports.Add(Convert.ToInt32(j));
                        } catch (FormatException e)
                        {
                            Console.WriteLine($"Invalid Port Range: {j}!");
                            return;
                        }
                    }
                }
                //Check for hyphen separated port ranges
                else if (i.Contains("-")){
                    try {
                        var bounds = new List<int>();
                        foreach(string j in i.Split('-'))
                        {
                            bounds.Add(Convert.ToInt32(j));
                        }
                        for(int j = bounds[0]; j<=bounds[1]; j++)
                        {
                            ports.Add(j);
                        }
                    }
                    catch (FormatException e)
                    {
                        Console.WriteLine($"Invalid Port Range: {i}!");
                        return;
                    }
                }
                //Add single port to the list
                else
                {
                    try
                    {
                        ports.Add(Convert.ToInt32(i));
                    }
                    catch (FormatException e)
                    {
                        Console.WriteLine($"Invalid Port Range: {i}!");
                        return;
                    }
                }
            }
            
            //Try to parse the IP
            try
            {
                 ipAddress = IPAddress.Parse(args[0]);
            } catch (IndexOutOfRangeException e)
            {
                Console.WriteLine($"No IP Specified!\nSyntax: {AppDomain.CurrentDomain.FriendlyName} IP PORT1 PORT2,PORT3 PORT-PORT");
                return;
            }
            catch (FormatException e)
            {
                Console.WriteLine($"Invalid IP: {args[0]}\nSyntax: {AppDomain.CurrentDomain.FriendlyName} IP PORT1 PORT2,PORT3 PORT-PORT");
            }

            //Confirm port range
            if (ports.Count == 0 || (ports.Max() > 65535 || ports.Min() < 0))
            {
                Console.WriteLine($"Invalid Port Range!");
            }

            //Start the multithreading
            foreach(int port in ports)
            {
                Thread t = new Thread(() => openPort(ipAddress, port));
                t.Start();
                while(Process.GetCurrentProcess().Threads.Count > maxThreads)
                {
                    continue;
                }

            }

        }
    }
}
