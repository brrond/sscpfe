using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace sscpfe
{
    class SSCPFEApplication : IApp
    {
        // Prev console config
        int YPos;
        string title;
        int bufferWidth;
        int bufferHeight;
        ConsoleColor cF, cB;
        Encoding inputEncoding;
        Encoding outputEncoding;

        // New console config
        Encoding encoding = Encoding.UTF8;

        // 
        Buffer buff;
        KeyboardHandler kh;

        // curr file name
        public string FName { get; private set; }

        // default constructor
        public SSCPFEApplication()
        {
            // save cfg
            bufferWidth = Console.BufferWidth;
            bufferHeight = Console.BufferHeight;
            cF = Console.ForegroundColor;
            cB = Console.BackgroundColor;
            YPos = Console.CursorTop;
            inputEncoding = Console.InputEncoding;
            outputEncoding = Console.OutputEncoding;
            title = Console.Title;

            // when application is closed return prev console config
            AppDomain.CurrentDomain.ProcessExit += (sender, e) =>
            {
                Console.ForegroundColor = cF;
                Console.BackgroundColor = cB;
                Console.SetBufferSize(bufferWidth, bufferHeight);
                Console.SetCursorPosition(0, YPos + buff.MaxYPos() + 5);
                Console.OutputEncoding = outputEncoding;
                Console.InputEncoding = inputEncoding;
                Console.Title = title;
            };

            // set new config
            Console.SetBufferSize(16000, 16000); // I don't know why value so "high"
            Console.ForegroundColor = ConsoleColor.Green; // I watched Matrix recently
            Console.OutputEncoding = Encoding.UTF8; // UTF-8 encoding
            Console.InputEncoding = Encoding.UTF8;
            Console.Title = "sscpfe";

            kh = new KeyboardHandler(); // init KeyboardHandler
            buff = new Buffer(0, YPos); // init Buffer
            FName = ""; // there is no file (means new file will be created)
        }

        public SSCPFEApplication(string fname) : this(fname, new UTF8Encoding(false))
        {
            
        }

        public SSCPFEApplication(string fname, Encoding encoding) : this()
        {
            this.encoding = encoding;
            Console.OutputEncoding = encoding;
            Console.InputEncoding = encoding;
            FName = fname; // we have some file to work with
            if (Read(fname))
                Console.Title = "sscpfe - " + FName;
        }

        bool Read(string fname)
        {
            try
            {
                if (File.Exists(fname))
                {
                    using (FileStream stream = new FileStream(fname, FileMode.Open)) // try to open it
                    {
                        List<string> b = new List<string>();        // tmp buffer
                        StreamReader sR = new StreamReader(stream, encoding); // open stream reader
                        string[] arr = sR.ReadToEnd().Split('\n');  // read everything and split by line
                        for (int i = 0; i < arr.Length; i++)         // add every line in b
                            b.Add(arr[i]);
                        sR.Close();                                 // close stream
                                                                    //Console.WriteLine("Open");                // debug info (actually bad idea)
                        buff.LoadBuff(b);                           // load file into buffer
                    }
                    return true;
                }
            }
            catch { }
            return false;
        }

        bool Write(string fname)
        {
            try
            {
                using (FileStream stream = new FileStream(fname, FileMode.Create)) // open file stream
                {
                    StreamWriter streamWriter = new StreamWriter(stream, encoding);   // open stream writer
                    bool first_line = true;                                 // first_line ? (we should some how '\n')
                    foreach (string str in buff.Buff())
                    {
                        if (!first_line)
                            streamWriter.WriteLine();
                        else
                            first_line = false;
                        streamWriter.Write(str);
                    }
                    streamWriter.Close();                                   // close stream writer
                }
                return true;
            }
            catch { }
            return false;
        }

        bool AskUser(string msg)
        {
            bool res = true;
            do
            {
                Console.Write(msg);
                string key = Console.ReadKey().Key.ToString().ToLower().Trim();
                if (key == "y")
                {
                    res = true;
                    break;
                }
                else if (key == "n")
                {
                    res = false;
                    break;
                }

                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write(new string(' ', msg.Length + 1));
                Console.SetCursorPosition(0, Console.CursorTop);

            } while (true);
            Console.WriteLine();
            return res;
        }

        void HandleEsc()
        {
            // user pressed ESC
            Console.SetCursorPosition(buff.DefaultXPos, buff.DefaultYPos + buff.MaxYPos() + 2);
            if (AskUser("Do you want to save current buffer (Y/n) > ")) // simple handle
            {
                Regex fileNameRegex = new Regex("[a-zA-Z0-9\\-. _]*\\.[a-zA-Z0-9\\-. _]+");
                while (true) // loop so user WILL enter file name
                {
                    Console.Write("File name : ");
                    System.Windows.Forms.SendKeys.SendWait(FName); // if we have fname
                    FName = Console.ReadLine();
                    if (fileNameRegex.IsMatch(FName)) // if we have right extension
                    {
                        if(File.Exists(FName)) // if exist we should ask user to save
                        {
                            if (!AskUser(FName + " exists. Do you want to proceed (Y/n) > "))
                                continue;
                        }
                        Write(FName);
                        Console.WriteLine("Done");  // done
                        break;
                    }
                }
                Environment.Exit(0);                                                    // hard di... exit
            }
            else
            {
                Environment.Exit(0);
            }
        }

        public void Mainloop()
        {
            while (true)
            {
                // print buffer (bad but)
                buff.Print();

                switch (kh.Handle()) // get input from user
                {
                    case KeyboardHandlerCommand.UpArrow:
                        buff.MoveUp();
                        break;
                    case KeyboardHandlerCommand.DownArrow:
                        buff.MoveDown();
                        break;
                    case KeyboardHandlerCommand.LeftArrow:
                        buff.MoveLeft();
                        break;
                    case KeyboardHandlerCommand.RightArrow:
                        buff.MoveRight();
                        break;
                    case KeyboardHandlerCommand.Backspace:
                        buff.Backspace();
                        break;
                    case KeyboardHandlerCommand.Enter:
                        buff.Enter();
                        break;
                    case KeyboardHandlerCommand.Esc:
                        HandleEsc();
                        break;
                    case KeyboardHandlerCommand.Home:
                        buff.Home();
                        break;
                    case KeyboardHandlerCommand.End:
                        buff.End();
                        break;
                    case KeyboardHandlerCommand.Default:
                        buff.Insert("" + kh.LastKeyChar);
                        break;
                    case KeyboardHandlerCommand.CtrlV:
                        // works really bad
                        if (System.Windows.Forms.Clipboard.ContainsText())
                            buff.Insert(System.Windows.Forms.Clipboard.GetText());
                        break;
                    case KeyboardHandlerCommand.CtrlBackspace:
                        buff.CtrlBackspace();
                        break;
                    case KeyboardHandlerCommand.Tab:
                        buff.Insert("    ");
                        break;
                    case KeyboardHandlerCommand.CtrlLeftArrow:
                        buff.CtrlLeftArrow();
                        break;
                    case KeyboardHandlerCommand.CtrlRightArrow:
                        buff.CtrlRightArrow();
                        break;
                    case KeyboardHandlerCommand.CtrlDel:
                        buff.CtrlDel();
                        break;
                    case KeyboardHandlerCommand.Del:
                        buff.Del();
                        break;
                    default:
                        throw new SSCPFEHandlerException();
                }

            }
        }

    }
}
