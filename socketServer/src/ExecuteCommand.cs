using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace socketServer.src
{
    class ExecuteCommand:IExecuteCommand
    {
        string IExecuteCommand.eExecuteCommand(string commandStr)
        {
            Process p = new Process();//创建进程对象 
            p.StartInfo.FileName = "cmd.exe";//设定需要执行的命令 
            p.StartInfo.UseShellExecute = false;//不使用系统外壳程序启动 
            p.StartInfo.RedirectStandardInput = true;//可以重定向输入  
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = false;//不创建窗口 
            p.Start();
            p.StandardInput.WriteLine(commandStr);
            p.StandardInput.WriteLine("exit");
            if (p != null)
            {
                p.Close();
            }
            return "ok";
        }
    }
}
