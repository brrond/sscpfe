using System.IO;
using System.Configuration;
using System;
using System.Threading;
using System.Diagnostics;
using System.Globalization;

namespace sscpfe
{
    class SSCPFEConfigurationApplication : IApp
    {

        public SSCPFEConfigurationApplication()
        {

        }

        public static int GetTabSize()
        {
            Configuration cfg = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            return int.Parse(cfg.AppSettings.Settings["TabSize"].Value);
        }

        public void Mainloop()
        {
            // ask user and set value for every possible cfg
            // Possible settings: 
            // TabSize
            // ForegounrdColor//
            // BackgroundColor//

            Configuration cfg = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            int tabSize = int.Parse(cfg.AppSettings.Settings["TabSize"].Value);
            string inp = "";
            do {
                Console.Write("TabSize : ");
                System.Windows.Forms.SendKeys.SendWait(tabSize.ToString());
                inp = Console.ReadLine();
                if (int.TryParse(inp, out tabSize) && tabSize > 0)
                    break;

            } while (true);
            cfg.AppSettings.Settings["TabSize"].Value = tabSize.ToString();
            cfg.Save();
        }
    }
}
