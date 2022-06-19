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
            //sscpfe                            // DONE - open editor
            //sscpfe file_name                  // DONE - open file `file_name` in editor
            //sscpfe /help                      // DONE - show help
            //sscpfe /b file_name               // DONE - read file `file_name` in binary format
            //sscpfe /e encoding file_name      // DONE - open file `file_name` with encoding `encoding` in editor
            //sscpfe /e                         // DONE - show all available encodings
            //sscpfe jkfajfkejfeifjeiffjeifj    // DONE - open this file (create new)
            //sscpfe /cfg                       // DONE - open cfg app
            //sscpfe /typingtest                // DONE - open typingtest app
            if (args.Length != 0)
            {
                if((args[0].ToLower() == "/b" || 
                    args[0].ToLower() == "-b" ||
                    args[0].ToLower() == "--binary") && args.Length == 2) 
                {
                    return ARGSHandlerCommand.BINARY; 
                }
                else if((args[0].ToLower() == "/e" ||
                    args[0].ToLower() == "-e" ||
                    args[0].ToLower() == "--encoding") && args.Length == 3)
                {
                    return ARGSHandlerCommand.ENCODING;
                }
                else if((args[0].ToLower() == "/e" ||
                    args[0].ToLower() == "-e" ||
                    args[0].ToLower() == "--encoding") && args.Length == 1)
                {
                    Console.WriteLine("All available encodings");
                    Console.WriteLine("CodePage Name");
                    foreach (EncodingInfo ei in Encoding.GetEncodings())
                        Console.WriteLine("{0,-8} {1,-25} ", ei.CodePage, ei.Name);
                    return ARGSHandlerCommand.ENCODINGS;
                }
                else if(args[0].ToLower() == "/cfg" ||
                    args[0].ToLower() == "-cfg" || 
                    args[0].ToLower() == "--configuration")
                {
                    return ARGSHandlerCommand.CFG;
                }
                else if(args[0].ToLower() == "--typingtest" ||
                    args[0].ToLower() == "/tt" ||
                    args[0].ToLower() == "-tt")
                {
                    return ARGSHandlerCommand.MONKEYTYPE;
                }
                if (args[0].ToLower()[0] == '/' ||
                    args[0].ToLower() == "-h" ||
                    args[0].ToLower() == "--help" ||
                    args[0].ToLower() == "/?")
                    return ARGSHandlerCommand.HELP;
                return ARGSHandlerCommand.FILE;
            }
            return ARGSHandlerCommand.DEFAULT;
        }

        public static void ShowHelp()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Super simple command promt file editor (sscpfe)");
            stringBuilder.AppendLine("https://github.com/greentech72/sscpfe.git");
            stringBuilder.AppendLine("");
            stringBuilder.AppendLine("sscpfe [filename] [-b filename] [-e] [-e filename] [-cfg] [-tt] [-h]");
            stringBuilder.AppendLine("");
            stringBuilder.AppendLine("filename                      - specify file to open or to create");
            stringBuilder.AppendLine("-b, --binary, /b              - read file as binary");
            stringBuilder.AppendLine("-e, --encoding, /e            - open file in specific encoding OR print all available encodings");
            stringBuilder.AppendLine("-cfg, --configuration, /cfg   - open configuration app. Allows to change options of editor");
            stringBuilder.AppendLine("-tt, --typingtest, /tt        - try it yourself (you need internet for this one)");
            stringBuilder.AppendLine("-h, --help, /?                - show help");
            Console.WriteLine(stringBuilder.ToString());
        }
    }
}
