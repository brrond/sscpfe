using System;
using System.Text;

namespace sscpfe
{
    class SSCPFERunner
    {
        readonly string[] args;        

        public SSCPFERunner(string[] args)
        {
            this.args = args;
        }


        public void Run()
        {
            IApp app = null; // create app
            switch (handleARGS())
            {
                case ARGSHandlerCommand.DEFAULT:
                    app = new SSCPFEApplication();
                    break;
                case ARGSHandlerCommand.HELP:
                    ShowHelp();
                    break;
                case ARGSHandlerCommand.FILE:
                    app = new SSCPFEApplication(args[0]);
                    break;
                case ARGSHandlerCommand.ENCODING:
                    app = new SSCPFEApplication(args[2], ParseEncoding(args[1]));
                    break;
                case ARGSHandlerCommand.BINARY:
                    app = new SSCPFEBinaryReaderApplication(args[1]);
                    break;
                case ARGSHandlerCommand.CFG:
                    app = new SSCPFEConfigurationApplication();
                    break;
                default:
                    break;
            }
            
            if(app != null)
                app.Mainloop(); // start app
        }

        private Encoding ParseEncoding(string str)
        {
            str = str.Trim().ToLower();
            if (str == "ascii")
                return Encoding.ASCII;
            else if (str == "utf16" || str == "utf-16")
                return Encoding.Unicode;
            else if (str == "utf7" || str == "utf-7")
                return Encoding.UTF7;
            else if (str == "utf32" || str == "utf-32")
                return Encoding.UTF32;

            return Encoding.UTF8;
        }

        ARGSHandlerCommand handleARGS()
        {
            //sscpfe                            // DONE
            //sscpfe file_name                  // DONE
            //sscpfe /help                      // DONE
            //sscpfe /b file_name               // DONE
            //sscpfe /e encoding file_name      // DONE
            //sscpfe jkfajfkejfeifjeiffjeifj    // DONE
            //sscpfe /cfg
            if (args.Length != 0)
            {
                if((args[0].ToLower() == "/b" || args[0].ToLower() == "-b") && args.Length == 2)
                {
                    return ARGSHandlerCommand.BINARY; 
                }
                else if((args[0].ToLower() == "/e" || args[0].ToLower() == "-e") && args.Length == 3)
                {
                    return ARGSHandlerCommand.ENCODING;
                }
                else if((args[0].ToLower() == "/cfg"))
                {
                    return ARGSHandlerCommand.CFG;
                }
                
                if (args[0].ToLower()[0] == '/')
                    return ARGSHandlerCommand.HELP;
                return ARGSHandlerCommand.FILE;
            }
            return ARGSHandlerCommand.DEFAULT;
        }

        public static void ShowHelp()
        {
            Console.WriteLine("Super simple command promt file editor (sscpfe)");
            Console.WriteLine("There are no commands actually. Just type 'sscpfe' as command and start entering your text.");
            Console.WriteLine("Also you can edit file by typing its name after 'sscpfe' command (sscpfe some_text.txt)");
        }
    }
}
