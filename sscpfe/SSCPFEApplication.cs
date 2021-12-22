using System;
using System.IO;
using System.Collections.Generic;

namespace sscpfe
{
    class SSCPFEApplication
    {
        // Prev console config
        int YPos;
        string title;
        int bufferWidth;
        int bufferHeight;
        ConsoleColor cF, cB;
        System.Text.Encoding inputEncoding;
        System.Text.Encoding outputEncoding;

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
            Console.OutputEncoding = System.Text.Encoding.UTF8; // UTF-8 encoding
            Console.InputEncoding = System.Text.Encoding.UTF8;
            Console.Title = "sscpfe";

            kh = new KeyboardHandler(); // init KeyboardHandler
            buff = new Buffer(0, YPos); // init Buffer
            FName = ""; // there is no file (means new file will be created)
        }

        public SSCPFEApplication(string fname) : this()
        {
            FName = fname; // we have some file to work with
            if(Read(fname))
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
                        StreamReader sR = new StreamReader(stream); // open stream reader
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
                    StreamWriter streamWriter = new StreamWriter(stream);   // open stream writer
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

        void HandleEsc()
        {
            // user pressed ESC
            Console.SetCursorPosition(buff.DefaultXPos, buff.DefaultYPos + buff.MaxYPos() + 2);
            Console.Write("Do you want to save current buffer (Y/n) > ");
            string input = Console.ReadLine();
            if(input.ToLower() == "y" || input == "") // simple handle
            {
                while (true) // loop so user WILL enter file name
                {
                    Console.Write("File name : ");
                    System.Windows.Forms.SendKeys.SendWait(FName); // if we have fname
                    FName = Console.ReadLine();
                    if(FName.Contains(".")) // if we have extension
                    {
                        string[] FName_p = FName.Split('.'); // get name and extension
                        if(FName_p[FName_p.Length - 1].Length != 0) // make sure everything is right with extension (after last .)
                        {
                            if(File.Exists(FName)) // if exist we should ask user to save
                            {
                                Console.Write(FName + " exists. Do you want to proceed (Y/n) > ");
                                input = Console.ReadLine();
                                if (input.ToLower() != "y" && input != "") // interesting if here (it's ok tho)
                                    continue;
                            }
                            Write(FName);
                            Console.WriteLine("Done");                                  // done
                            break;
                        }
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
                    case HandlerCommand.UpArrow:
                        buff.MoveUp();
                        break;
                    case HandlerCommand.DownArrow:
                        buff.MoveDown();
                        break;
                    case HandlerCommand.LeftArrow:
                        buff.MoveLeft();
                        break;
                    case HandlerCommand.RightArrow:
                        buff.MoveRight();
                        break;
                    case HandlerCommand.Backspace:
                        buff.Backspace();
                        break;
                    case HandlerCommand.Enter:
                        buff.Enter();
                        break;
                    case HandlerCommand.Esc:
                        HandleEsc();
                        break;
                    case HandlerCommand.Home:
                        buff.Home();
                        break;
                    case HandlerCommand.End:
                        buff.End();
                        break;
                    case HandlerCommand.Default:
                        buff.Insert("" + kh.LastKeyChar);
                        break;
                    case HandlerCommand.CtrlV:
                        // works really bad
                        if (System.Windows.Forms.Clipboard.ContainsText())
                            buff.Insert(System.Windows.Forms.Clipboard.GetText());
                        break;
                    case HandlerCommand.CtrlBackspace:
                        buff.CtrlBackspace();
                        break;
                    case HandlerCommand.Tab:
                        buff.Insert("    ");
                        break;
                    case HandlerCommand.CtrlLeftArrow:
                        buff.CtrlLeftArrow();
                        break;
                    case HandlerCommand.CtrlRightArrow:
                        buff.CtrlRightArrow();
                        break;
                    case HandlerCommand.CtrlDel:
                        buff.CtrlDel();
                        break;
                    case HandlerCommand.Del:
                        buff.Del();
                        break;
                    default:
                        throw new SSCPFEHandlerException();
                }

            }
        }

        public static void ShowHelp()
        {
            Console.WriteLine("Super simple command promt file editor (sscpfe)");
            Console.WriteLine("There are no commands actually. Just type 'sscpfe' as command and start entering your text.");
            Console.WriteLine("Also you can edit file by typing its name after 'sscpfe' command (sscpfe some_text.txt)");
        }
    }
}
