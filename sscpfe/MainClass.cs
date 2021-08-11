using System;
using System.IO;
using System.Collections.Generic;

namespace sscpfe
{

    enum HandlerCommand
    {
        UpArrow,
        DownArrow,
        LeftArrow,
        RightArrow,
        Backspace,
        Enter,
        Home,
        End,
        Esc,
        Default
    }

    class SSCPFEHandlerException : Exception
    {
        public SSCPFEHandlerException() : base("Expection thrown by handler in mainloop")
        {

        }
    }

    class Buffer
    {
        List<string> buff;
        bool newLineFlag = false;

        public Buffer()
        {
            buff = new List<string>();
            buff.Add("");
            XPos = 0;
            YPos = 0;
        }

        public Buffer(int DefaultX, int DefaultY) : this()
        {
            DefaultXPos = DefaultX;
            DefaultYPos = DefaultY;
        }

        public int XPos { get; private set; }
        public int YPos { get; private set; }
        public int DefaultXPos { get; private set; }
        public int DefaultYPos { get; private set; }

        public string this[int i]
        {
            get { return buff[i]; }
        }

        public IEnumerable<string> Buff()
        {
            foreach (string str in buff)
                yield return str;
        }

        public void LoadBuff(List<string> buff)
        {
            this.buff = buff;
        }

        public int MaxYPos()
        {
            return buff.Count;
        }
        
        string CreateEmptyLine(int len)
        {
            string line = "";
            for (int i = 0; i < len; i++)
                line += "\0";
            return line;
        }

        public void Print()
        {
            Console.SetCursorPosition(DefaultXPos, DefaultYPos);
            for(int i = 0; i < buff.Count; i++)
            {
                Console.WriteLine(buff[i]);
                buff[i] = buff[i].Split('\0')[0];
            }
            if (newLineFlag)
            {
                newLineFlag = false;
                buff.RemoveAt(buff.Count - 1);
            }
            Console.SetCursorPosition(DefaultXPos + XPos, DefaultYPos + YPos);
        }

        public void Insert(string character)
        {
            buff[YPos] = buff[YPos].Insert(XPos++, character);
        }

        public void Backspace()
        {
            if (XPos != 0)
            {
                buff[YPos] = buff[YPos].Remove(--XPos, 1);
                buff[YPos] += (char)0;
            }
            else if(YPos != 0)
            {
                // delete \n
                XPos = buff[YPos - 1].Length;
                int len = buff[YPos].Length;
                buff[YPos - 1] += buff[YPos];
                buff.RemoveAt(YPos);
                YPos--;

                buff.Add(CreateEmptyLine(1000));
                newLineFlag = true;
                int tmp = YPos + 1;
                while (tmp != buff.Count - 1)
                {
                    buff[tmp] += CreateEmptyLine(1000);
                    tmp++;
                }
            }
        }

        public void Enter()
        {
            string additionalEmptyString = "";
            if (YPos + 1 < buff.Count)
            {
                additionalEmptyString = CreateEmptyLine(buff[YPos + 1].Length);
            }
            buff.Insert(YPos + 1, buff[YPos].Substring(XPos) + additionalEmptyString);
            buff[YPos] = buff[YPos].Substring(0, XPos) + CreateEmptyLine(buff[YPos + 1].Length);
            YPos++;
            XPos = 0;
        }

        public void MoveUp()
        {
            if (YPos != 0)
            {
                YPos--;
                if (XPos > buff[YPos].Length)
                    XPos = buff[YPos].Length;
            }
        }

        public void MoveDown()
        {
            if (YPos < buff.Count - 1)
            {
                YPos++;
                if (XPos > buff[YPos].Length)
                    XPos = buff[YPos].Length;
            }
        }

        public void MoveLeft()
        {
            if (XPos != 0)
            {
                XPos--;
            }
            else if(YPos != 0)
            {
                XPos = buff[--YPos].Length;
            }
        }

        public void MoveRight()
        {
            if (XPos < buff[YPos].Length)
            {
                XPos++;
            }
        }

        public void Home()
        {
            XPos = 0;
        }

        public void End()
        {
            XPos = buff[YPos].Length;
        }

    }

    class KeyboardHandler
    {
        public KeyboardHandler()
        {

        }

        public HandlerCommand Handle()
        {
            ConsoleKeyInfo info = Console.ReadKey();
            switch(info.Key.ToString())
            {
                case "UpArrow": return HandlerCommand.UpArrow;
                case "DownArrow": return HandlerCommand.DownArrow;
                case "LeftArrow": return HandlerCommand.LeftArrow;
                case "RightArrow": return HandlerCommand.RightArrow;
                case "Backspace": return HandlerCommand.Backspace;
                case "Enter": return HandlerCommand.Enter;
                case "Escape": return HandlerCommand.Esc;
                case "Home": return HandlerCommand.Home;
                case "End": return HandlerCommand.End;
                default:
                    LastKey = info.Key;
                    LastKeyChar = info.KeyChar;
                    break;
            }
            return HandlerCommand.Default;
        }

        public ConsoleKey LastKey { get; private set; }
        public char LastKeyChar { get; private set; }
    }

    class SSCPFEApplication
    {
        KeyboardHandler kh;
        Buffer buff;

        public string FName { get; private set; }

        public SSCPFEApplication()
        {
            Console.SetBufferSize(16000, 16000);
            Console.ForegroundColor = ConsoleColor.Green;
            kh = new KeyboardHandler();
            buff = new Buffer();
            FName = "";
        }

        public SSCPFEApplication(string fname) : this()
        {
            FName = fname;
            if (File.Exists(fname))
            {
                using (FileStream stream = new FileStream(fname, FileMode.Open))
                {
                    List<string> b = new List<string>();
                    StreamReader sR = new StreamReader(stream);
                    string[] arr = sR.ReadToEnd().Split('\n');
                    for(int i = 0; i < arr.Length; i++)
                        b.Add(arr[i]);
                    sR.Close();
                    Console.WriteLine("Open");
                    buff.LoadBuff(b);
                }
            }
        }

        void HandleEsc()
        {
            Console.SetCursorPosition(buff.DefaultXPos, buff.DefaultYPos + buff.MaxYPos() + 2);
            Console.Write("Do you want to save current buffer (Y/n) > ");
            string input = Console.ReadLine();
            if(input.ToLower() == "y" || input == "")
            {
                while (true)
                {
                    Console.Write("File name : ");
                    System.Windows.Forms.SendKeys.SendWait(FName);
                    FName = Console.ReadLine();
                    if(FName.Contains("."))
                    {
                        string[] FName_p = FName.Split('.');
                        if(FName_p[FName_p.Length - 1].Length != 0)
                        {
                            using (FileStream stream = new FileStream(FName, FileMode.Create))
                            {
                                StreamWriter streamWriter = new StreamWriter(stream);
                                foreach (string str in buff.Buff())
                                    streamWriter.WriteLine(str);
                                streamWriter.Close();
                            }
                            Console.WriteLine("Done");
                            break;
                        }
                    }
                }
                Environment.Exit(0);
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
                buff.Print();

                switch (kh.Handle())
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
                    default:
                        throw new SSCPFEHandlerException();
                }

            }
        }
    }
    class MainClass
    {
        static void Main(string[] args)
        {
            SSCPFEApplication app;
            if (args.Length != 0)
                app = new SSCPFEApplication(args[0]);
            else
                app = new SSCPFEApplication();

            app.Mainloop();
        }
    }
}
