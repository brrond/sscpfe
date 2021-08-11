using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

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
                for (int i = 0; i < buff[YPos + 1].Length; i++)
                    additionalEmptyString += '\0';
            }
            buff.Insert(YPos + 1, buff[YPos].Substring(XPos) + additionalEmptyString); ////// YPos || YPos + 1
            string emptyString = "";
            for (int i = 0; i < buff[YPos + 1].Length; i++)
                emptyString += '\0';
            buff[YPos] = buff[YPos].Substring(0, XPos) + emptyString;
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
                case "Esc": return HandlerCommand.Esc;
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
        public SSCPFEApplication()
        {
            Console.SetBufferSize(16000, 16000);
            kh = new KeyboardHandler();
            buff = new Buffer();
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
                        ////////////////
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

                // handle out of borders

            }
        }
    }
    class MainClass
    {
        static void Main(string[] args)
        {
            SSCPFEApplication app = new SSCPFEApplication();
            app.Mainloop();
        }
    }
}
