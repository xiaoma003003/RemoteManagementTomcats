using deno.commond;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace deno.util
{
   //命令帮助类
    class CDHelper
    {
        string operatingsystem;
        public  CDHelper(string operatingsystem) {
            this.operatingsystem = operatingsystem;
        }
        public IMakeCommond getMakeCommond() {
            IMakeCommond iMakeCommond = null;
            switch (operatingsystem) {
                case "win7":
                    iMakeCommond = new MakeWin7Commond();
                    break;
                default:
                    break;
            }
            return iMakeCommond;
        }
    }
}
