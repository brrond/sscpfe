using System.IO;
using System.Configuration;
using System;
using System.Threading;
using System.Diagnostics;

namespace sscpfe
{
    class SSCPFEConfigurationApplication
    {
        public static int GetTabSize()
        {
            return int.Parse(ConfigurationManager.AppSettings["TabSize"]);
        }

    }
}
