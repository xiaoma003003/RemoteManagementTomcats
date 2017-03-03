using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace socketServer
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            //string HostName = Dns.GetHostName(); //得到主机名 
            //IPHostEntry IpEntry = Dns.GetHostEntry(HostName); //得到主机IP             
           // string strIPAddr = IpEntry.AddressList[0].ToString();
            IPAddress ip = IPAddress.Parse("192.168.0.8"); //把ip地址字符串转换为IPAddress              
            IPEndPoint ipep = new IPEndPoint(ip, 20000);  //用指定的端口和ip   
            Socket newsock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            newsock.Bind(ipep);//绑定              
            newsock.Listen(19999);//监听              
            while (true)
            {
                try
                {
                    //当有可用的客户端连接尝试时执行，并返回一个新的socket                  
                    Socket client = newsock.Accept();
                    //创建消息服务线程对象，并把连接socket赋于ClientThread                     
                    ClientThread newclient = new ClientThread(client);
                    //把ClientThread 类的ClientService方法委托给线程 
                    Thread newthread = new Thread(new ThreadStart(newclient.ClientServer));
                    // 启动消息服务线程                     
                    newthread.Start();
                }
                catch
                {
                    //连接中断或者连接失败                
                }
            }
        }
    }   
}
