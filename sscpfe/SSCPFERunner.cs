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
                case ARGSHandlerCommand.ENCODINGS:
                    break;
                case ARGSHandlerCommand.BINARY:
                    app = new SSCPFEBinaryReaderApplication(args[1]);
                    break;
                case ARGSHandlerCommand.CFG:
                    app = new SSCPFEConfigurationApplication();
                    break;
                case ARGSHandlerCommand.MONKEYTYPE:
                    app = new SSCPFETypingTestApplication();
                    break;
                default:
                    break;
            }

            if (app != null)
                app.Mainloop(); // start app
        }

        private Encoding ParseEncoding(string str)
        {
            // this method gets encoding from user input
            // the problem is we have two possible input types
            // 1. Code of the encoding
            // 2. Name of the encoding
            try
            {
                str = str.Trim().ToLower(); // preprocess string
                int code;
                if (int.TryParse(str, out code)) // if we have code
                    return Encoding.GetEncoding(code); // try to return encoding by code
                return Encoding.GetEncoding(str); // try to return encoding by name
            } catch(Exception e) // If user doesn't pay attention
            {
                Console.WriteLine("Incorrect encoding: {0}", str);
                Console.WriteLine(e.Message);
                Console.WriteLine("Use sscpfe /e to see all available encodings");
                return Encoding.ASCII;
            }
        }

        ARGSHandlerCommand handleARGS()
        {
            //sscpfe                            // DONE
            //sscpfe file_name                  // DONE
            //sscpfe /help                      // DONE
            //sscpfe /b file_name               // DONE
            //sscpfe /e encoding file_name      // DONE
            //sscpfe /e                         // DONE
            //sscpfe jkfajfkejfeifjeiffjeifj    // DONE
            //sscpfe /cfg                       // DONE
            //sscpfe /typingtest                // DONE
            if (args.Length != 0)
            {
                if((args[0].ToLower() == "/b") && args.Length == 2)
                {
                    return ARGSHandlerCommand.BINARY; 
                }
                else if((args[0].ToLower() == "/e") && args.Length == 3)
                {
                    return ARGSHandlerCommand.ENCODING;
                }
                else if((args[0].ToLower() == "/e") && args.Length == 1)
                {
                    Console.WriteLine("All available encodings");
                    Console.WriteLine("CodePage Name");
                    foreach (EncodingInfo ei in Encoding.GetEncodings())
                        Console.WriteLine("{0,-8} {1,-25} ", ei.CodePage, ei.Name);
                    return ARGSHandlerCommand.ENCODINGS;
                }
                else if((args[0].ToLower() == "/cfg"))
                {
                    return ARGSHandlerCommand.CFG;
                }
                else if((args[0].ToLower() == "/typingtest"))
                {
                    return ARGSHandlerCommand.MONKEYTYPE;
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
