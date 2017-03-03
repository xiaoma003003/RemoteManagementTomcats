using socketServer.src;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace socketServer
{
    class ClientThread
    {
        IExecuteCommand executeCommand = null;
        private Socket client;
        byte[] recvBytes = null;
        Boolean isKeepAlive = true;
        //传递连接socket 
        public ClientThread(Socket ClientSocket)
        {
            this.client = ClientSocket;
        }
        //数据处理接口 
        public void ClientServer()
        {
            try
            {
                while (isKeepAlive)
                {
                    string recvStr = "";
                    recvBytes = new byte[1024];
                    int bytes;
                    //从客户端接受信息
                    bytes = client.Receive(recvBytes, recvBytes.Length, 0);
                    recvStr += Encoding.ASCII.GetString(recvBytes, 0, bytes);
                    //把客户端传来的信息显示出来
                    Console.WriteLine("server get message:{0} from client", recvStr);
                    //执行cmd命令
                    if (!recvStr.Equals("") && recvStr.StartsWith("command")) {
                        recvStr = recvStr.Substring(7);
                        string result = executeCommand.eExecuteCommand(recvStr);
                        responseClient(result);
                    } else if (!recvStr.Equals("") && recvStr.StartsWith("exit")) {
                        isKeepAlive = false;
                        responseClient("ok close connect");
                    }
                    else if (!recvStr.Equals("") && recvStr.StartsWith("hi")){
                        responseClient("hi");
                    } else if (!recvStr.Equals("") && recvStr.StartsWith("executeCommand")){
                        executeCommand = new ExecuteCommand();
                        responseClient("ok execute command");
                    } else if (!recvStr.Equals("") && recvStr.StartsWith("getresponse")) {
                        executeCommand = new ExecuteCommandGetFiles();
                        responseClient("ok get response");
                    }
                    else {
                        responseClient("hi");
                    }
                }
            } catch (Exception ex){
                Console.Write("出现异常：");                 
                Console.WriteLine(ex.ToString());                 
                Console.ReadLine();
            }
            client.Close();
        }
        private void responseClient(string sendStr) {
            if (sendStr == null)
            {
                sendStr = "ok!";
            }
            byte[] bs = Encoding.ASCII.GetBytes(sendStr);
            client.Send(bs, bs.Length, 0);//返回信息给客户端
        }  
    }
}

