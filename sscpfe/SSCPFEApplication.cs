using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;

namespace sscpfe
{

    class OperationInfo
    {
        public int XPos { get; private set; }
        public int YPos { get; private set; }
        public int Repeats { get; private set; }
        public MethodInfo Method { get; private set; }
        public object[] Parametrs { get; private set; }

        public OperationInfo(int xPos, int yPos, int repeats, MethodInfo method, object[] parametrs)
        {
            XPos = xPos;
            YPos = yPos;
            Repeats = repeats;
            Method = method;
            Parametrs = parametrs;
        }
    }

    abstract class Operation
    {
        protected int xPosBefore, yPosBefore;
        protected int xPosAfter, yPosAfter;

        public Operation(int XPosBefore, int YPosBefore, int XPosAfter, int YPosAfter)
        {
            xPosBefore = XPosBefore;
            yPosBefore = YPosBefore;
            xPosAfter = XPosAfter;
            yPosAfter = YPosAfter;
        }

        abstract public OperationInfo Undo();

        //abstract public void Redo();
    }

    class DeleteOperation : Operation
    {
        char ch;
        public DeleteOperation(int XPosBefore, int YPosBefore, int XPosAfter, int YPosAfter, char Ch) :
            base(XPosBefore, YPosBefore, XPosAfter, YPosAfter)
        {
            ch = Ch;
        }

        public override OperationInfo Undo()
        {
            Type buffType = typeof(Buffer);
            if (ch == '\n')
            {
                MethodInfo methodInfo = buffType.GetMethod("Enter");
                return new OperationInfo(xPosAfter, yPosAfter, 1, methodInfo,
                    new object[] { });
            }
            else
            {
                MethodInfo methodInfo = buffType.GetMethod("Insert");
                return new OperationInfo(xPosAfter, yPosAfter, 1, methodInfo,
                    new object[] { ch.ToString() });
            }
        }
    }

    class InsertOperation : Operation
    {
        public InsertOperation(int XPosBefore, int YPosBefore, int XPosAfter, int YPosAfter) : base(XPosBefore, YPosBefore, XPosAfter, YPosAfter)
        {
        }

        public override OperationInfo Undo()
        {
            Type buffType = typeof(Buffer);
            MethodInfo methodInfo = buffType.GetMethod("Backspace");
            return new OperationInfo(xPosAfter, yPosAfter, 1, methodInfo,
                new object[] { });
        }
    }

    class OperationList
    {
        LinkedList<Operation> operations;
        public LinkedListNode<Operation> Curr { get; private set; } = null;
        public OperationList()
        {
            operations = new LinkedList<Operation>();
        }
        public void Prev()
        {
            if(Curr != null)
                Curr = Curr.Previous;
        }

        public void Next()
        {
            if(Curr != null)
                Curr = Curr.Next;
        }
        public void Add(Operation operation)
        {
           if(Curr != null && Curr != operations.Last) // for redo
            {
                // delete after curr
                while (Curr.Next != null)
                {
                    operations.Remove(Curr.Next);
                }

                if (operations.Count == 0)
                    Curr = null;
            }
            operations.AddLast(operation);
            Curr = operations.Last;
        }
    }

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

                // For undo/redo
                int xPosBefore = buff.XPos;
                int yPosBefore = buff.YPos;
                char currChar = '\n';        // TODO: Fix no element in redo del op
                                             // if there's no element that's problem

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
                        if (xPosBefore != 0)
                            currChar = buff[yPosBefore][xPosBefore - 1];
                        buff.Backspace();
                        operations.Add(new DeleteOperation(xPosBefore, yPosBefore,
                            buff.XPos, buff.YPos, currChar));
                        break;
                    case KeyboardHandlerCommand.Enter:
                        buff.Enter();
                        operations.Add(new InsertOperation(xPosBefore, yPosBefore, buff.XPos, buff.YPos));
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
                        operations.Add(new InsertOperation(xPosBefore, yPosBefore, buff.XPos, buff.YPos));
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
                        if (xPosBefore != buff[yPosBefore].Length)
                            currChar = buff[yPosBefore][xPosBefore];
                        buff.Del();
                        operations.Add(new DeleteOperation(xPosBefore, yPosBefore,
                            buff.XPos, buff.YPos, currChar));
                        break;
                    case KeyboardHandlerCommand.CtrlZ:
                        if (operations.Curr != null)
                        {
                            buff.PerformOperation(operations.Curr.Value.Undo());
                            operations.Prev();
                        }
                        break;
                    case KeyboardHandlerCommand.CtrlY:
                        //buff.Redo(currOperation);
                        break;
                    default:
                        throw new SSCPFEHandlerException();
                }

            }
        }

    }
}
