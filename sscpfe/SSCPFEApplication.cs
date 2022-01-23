using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
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
        OperationList operations;

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
            operations = new OperationList();
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
                        List<string> b = new List<string>();                            // tmp buffer
                        StreamReader sR = new StreamReader(stream, encoding);           // open stream reader
                        string[] arr = sR.ReadToEnd().Replace("\r", "").Split('\n');    // read everything and split by line
                        for (int i = 0; i < arr.Length; i++)                            // add every line in b
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

                // For undo/redo
                int xPosBefore = buff.XPos;
                int yPosBefore = buff.YPos;
                char currChar = '\n';
                string currStr = "";
                bool inWord = false;

                // For ctrl v
                string tmp;

                // For tab
                string tab = new string(' ', SSCPFEConfigurationApplication.GetTabSize());

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
                        if (xPosBefore != 0)                                        // if it isn't start of the line
                            currChar = buff[yPosBefore][xPosBefore - 1];            // save current char (else curr char '\n')
                        buff.Backspace();                                           // perform backspace
                        operations.Add(new DeleteOperation(xPosBefore, yPosBefore,  // add new operation
                            buff.XPos, buff.YPos, currChar, false));
                        break;
                    case KeyboardHandlerCommand.Enter:
                        buff.Enter();                                               // perform enter
                        operations.Add(new InsertOperation(xPosBefore, yPosBefore, buff.XPos, buff.YPos, "\n")); // add new insert operation
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
                        buff.Insert("" + kh.LastKeyChar);                           // perform insertion
                        operations.Add(new InsertOperation(xPosBefore, yPosBefore, buff.XPos, buff.YPos, "" + kh.LastKeyChar)); // add new operation
                        break;
                    case KeyboardHandlerCommand.CtrlV:
                        // TODO: Fix ctrl v (problems with enter)
                        if (System.Windows.Forms.Clipboard.ContainsText())
                        {
                            tmp = System.Windows.Forms.Clipboard.GetText().Replace("\r", "").Replace("\t", "    ");
                            foreach (string el in tmp.Split('\n')) {
                                buff.Insert(el);
                                buff.Enter();
                            }
                            operations.Add(new InsertOperation(xPosBefore, yPosBefore, buff.XPos, buff.YPos,
                                tmp));
                        }
                        break;
                    case KeyboardHandlerCommand.CtrlBackspace:
                        if(xPosBefore != 0)
                        {
                            if(buff[yPosBefore][xPosBefore - 1] != ' ')
                                inWord = true;
                            int currPos = xPosBefore - 1;
                            while(currPos != 0 && inWord)
                                if (buff[yPosBefore][currPos--] == ' ')
                                    inWord = false;
                            while (currPos != 0 && buff[yPosBefore][currPos] != ' ')
                                currPos--;
                            currStr = buff[yPosBefore].Substring(currPos, xPosBefore - currPos);
                        }
                        buff.CtrlBackspace();
                        operations.Add(new DeleteOperation(xPosBefore, yPosBefore, buff.XPos, buff.YPos, currStr, false));
                        break;
                    case KeyboardHandlerCommand.Tab:
                        buff.Insert(tab);
                        operations.Add(new InsertOperation(xPosBefore, yPosBefore, buff.XPos, buff.YPos, tab));
                        break;
                    case KeyboardHandlerCommand.CtrlLeftArrow:
                        buff.CtrlLeftArrow();
                        break;
                    case KeyboardHandlerCommand.CtrlRightArrow:
                        buff.CtrlRightArrow();
                        break;
                    case KeyboardHandlerCommand.CtrlDel:
                        if (xPosBefore != buff[yPosBefore].Length)
                        {
                            if (buff[yPosBefore][xPosBefore] != ' ')
                                inWord = true;
                            int currPos = xPosBefore;
                            while (currPos != buff[yPosBefore].Length && inWord)
                                if (buff[yPosBefore][currPos++] == ' ')
                                    inWord = false;
                            while (currPos != buff[yPosBefore].Length && buff[yPosBefore][currPos] != ' ')
                                currPos++;
                            currStr = buff[yPosBefore].Substring(xPosBefore, currPos - xPosBefore);
                        }
                        buff.CtrlDel();
                        operations.Add(new DeleteOperation(xPosBefore, yPosBefore, buff.XPos, buff.YPos, currStr, true));
                        break;
                    case KeyboardHandlerCommand.Del:
                        if (xPosBefore != buff[yPosBefore].Length)                  // if it isn't end of the line
                            currChar = buff[yPosBefore][xPosBefore];                // save curr char (else curr char '\n')
                        buff.Del();                                                 // perform del
                        operations.Add(new DeleteOperation(xPosBefore, yPosBefore,  // add new operation
                            buff.XPos, buff.YPos, currChar, true));
                        break;
                    case KeyboardHandlerCommand.CtrlZ:
                        if (operations.Curr != null)                                // if we have operations
                        {               
                            buff.PerformOperation(operations.Curr.Value.Undo());    // perform undo
                            operations.Prev();                                      // return to prev operation
                        }
                        break;
                    case KeyboardHandlerCommand.CtrlY:
                        if(operations.Curr != null && operations.Curr.Next != null)
                        {
                            buff.PerformOperation(operations.Curr.Next.Value.Redo());
                            operations.Next();
                        }
                        break;
                    default:
                        throw new SSCPFEHandlerException();
                }

            }
        }

    }
}
