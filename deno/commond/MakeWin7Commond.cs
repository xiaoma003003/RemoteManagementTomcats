using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace deno.commond
{
    class MakeWin7Commond : IMakeCommond
    {
        //获取启动tomcat命令
        //tomcatPath tomcat路径E:\apache-tomcat-7.0.40-touch\bin\startup.bat
        string IMakeCommond.getStartTomcatCommond(string tomcatPath)
        {
            string sendStr = null;
            if (null!= tomcatPath && !"".Equals(tomcatPath)) {
                sendStr = "command" + tomcatPath.Substring(0, 2) + "& cd " + tomcatPath.Substring(0, tomcatPath.LastIndexOf("\\")) + " & " + tomcatPath.Substring(tomcatPath.LastIndexOf("\\") + 1);
            }
            return sendStr;
        }
        //获取关闭tomcat命令
        //tomcatPath tomcat路径E:\apache-tomcat-7.0.40-touch\bin\shutdown.bat
        string IMakeCommond.getStopTomcatCommond(string tomcatStopPath)
        {
            
            string sendStr = null;
            if (null != tomcatStopPath && !"".Equals(tomcatStopPath))
            {
                sendStr = "command" + tomcatStopPath.Substring(0, 2) + "& cd " + tomcatStopPath.Substring(0, tomcatStopPath.LastIndexOf("\\")) + " & " + tomcatStopPath.Substring(tomcatStopPath.LastIndexOf("\\") + 1);
            }
            return sendStr;
        }
        //获取tomcat配置文件server.xml
        string IMakeCommond.getTomcatServerXml(string tomcatPath)
        {
            string sendStr = null;
            if (null != tomcatPath && !"".Equals(tomcatPath))
            {
                tomcatPath = tomcatPath.Substring(0, tomcatPath.LastIndexOf("\\bin")) + "\\conf\\";
                //获取tocat配置文件
                sendStr = "command" + tomcatPath.Substring(0, 2) + "& cd " + tomcatPath.Substring(0, tomcatPath.LastIndexOf("\\")) + " & type server.xml";
            }
            return sendStr;
        }
        //开启服务器命令
        string IMakeCommond.getRestartServer() {
            string sendStr = "command" + "shutdown -r -t 1";//1秒后关机
            return sendStr;
        }
        //关闭服务器命令
        string IMakeCommond.getStopServer()
        {
            string sendStr = "command" + "shutdown -s -t 1";//1秒后关机
            return sendStr;
        }
    }
}
