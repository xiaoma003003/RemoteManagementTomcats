using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace deno.util
{
    class LogHelper
    {
        FileHelper fh = null;
        public void writeLog(string str) {
            string logLogName = DateTime.Now.ToString("yyyyMMdd");
            string path = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase.Replace("bin\\", "").Replace("Debug\\", "") + "logs\\" + logLogName + ".log";
            System.IO.Directory.CreateDirectory(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase.Replace("bin\\", "").Replace("Debug\\", "") + "logs\\");
            fh = new FileHelper();
            List<string> logList = new List<string>();
            logList.Add(str);
            fh.writeLog(logList, path);
            
        }
    }
}
