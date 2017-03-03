using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace deno
{
  public class UtilsHelper
    {
        public static bool IsIPSect(string ip)
        {
            return Regex.IsMatch(ip, @"^（（2[0-4]\d|25[0-5]|[01]?\d\d?）\.）{2}（（2[0-4]\d|25[0-5]|[01]?\d\d?|\*）\.）（2[0-4]\d|25[0-5]|[01]?\d\d?|\*）$");
        }
}
}
