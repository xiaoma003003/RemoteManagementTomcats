using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace socketServer.src
{
    interface IExecuteCommand
    {
       string eExecuteCommand(string commandStr);
    }
}
