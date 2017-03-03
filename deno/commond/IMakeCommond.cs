using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace deno.commond
{
    interface IMakeCommond
    {
        string getStartTomcatCommond(string tomcatPath);
        string getStopTomcatCommond(string tomcatStopPath);
        string getTomcatServerXml(string tomcatPath);
        string getStopServer();
        string getRestartServer();
    }
}
