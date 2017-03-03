using deno.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace deno
{
    class SCHelper
    {
        LogHelper log = new LogHelper();
        Socket c = null;
        public SCHelper(int port,string host) {
            try { 
                //创建终结点EndPoint
                IPAddress ip = IPAddress.Parse(host);
                //把ip和端口转化为IPEndpoint实例
                IPEndPoint ipe = new IPEndPoint(ip, port);
                //创建socket并连接到服务器
                c = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//创建Socket
                Console.WriteLine("Conneting Server");
                c.Connect(ipe);//连接到服务器
            }catch (Exception ex)
            {
                Console.Write("连接服务出现异常：");
                Console.WriteLine(ex.ToString());
                Console.ReadLine();
            }
        }
        public string sendCommond(string commondStr) {
            try
            {
                string recvStr = "";
                //向服务器发送信息
                //把字符串编码为字节
                byte[] bs = Encoding.ASCII.GetBytes(commondStr);
                Console.WriteLine("Send Message:  "+ commondStr);
                //发送信息    
                if (c != null)
                {
                    c.Send(bs, bs.Length, 0);
                    //接受从服务器返回的信息           
                    byte[] recvBytes = new byte[1024 * 1024];
                    int bytes;
                    //从服务器端接受返回信息
                    bytes = c.Receive(recvBytes, recvBytes.Length, 0);
                    recvStr += Encoding.ASCII.GetString(recvBytes, 0, bytes);
                    //显示服务器返回信息  
                    //Console.WriteLine(System.DateTime.Now + "- get message from server:{0}", recvStr);   
                }
                else {
                    recvStr = "连接异常！";
                }
                return recvStr;
            }
            catch (Exception ex)
            {
                Console.Write("连接服务出现异常：");
                Console.WriteLine(ex.ToString());
                Console.ReadLine();
            }
            return null;
        }
        public void socketClose() {
            log.writeLog(System.DateTime.Now + "  关闭连接" );
            c.Close();
        }
        
    }
}
