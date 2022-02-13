using System;
using System.Collections.Generic;

namespace sscpfe
{
    class TypingTestBuffer : IBuffer
    {
        // TODO: rename to painteble buffer (or smth) extract base class and change
        Buffer buff;
        List<List<CharColor>> colorsBuff;

        List<string> text;

        void LoadText(string Text)
        {
            // Function to load some Text in List<string> and then loadBuff
            int w = Console.WindowWidth - 1; // width of Console
            text = new List<string>(); // create list
            for (int i = 0; i < Text.Length; i += w) // split Text by width of console blocks
            {
                if (i + w > Text.Length)
                    text.Add(Text.Substring(i));
                else
                    text.Add(Text.Substring(i, w));
            }
            LoadBuff(text);
        }

        public TypingTestBuffer(string Text)
        {
            colorsBuff = new List<List<CharColor>>();
            buff = new Buffer();
            LoadText(Text);
            _defaultCursor = ((IBuffer)buff).defaultCursor;
        }

        public TypingTestBuffer(int DefaultX, int DefaultY, string Text)
        {
            buff = new Buffer(DefaultX, DefaultY);
            colorsBuff = new List<List<CharColor>>();
            LoadText(Text);
            _defaultCursor = ((IBuffer)buff).defaultCursor;
        }

        public string this[int i] => ((IBuffer)buff)[i];

        CursorPosition _cursor, _defaultCursor;

        public CursorPosition cursor => _cursor;

        public CursorPosition defaultCursor => _defaultCursor;

        public IEnumerable<string> Buff()
        {
            return ((IBuffer)buff).Buff();
        }

        public void CtrlDel()
        {
            ((IBuffer)buff).CtrlDel(); // TODO: Fix color change
        }

        public void CtrlLeftArrow()
        {
            ((IBuffer)buff).CtrlLeftArrow();
        }

        public void CtrlRightArrow()
        {
            ((IBuffer)buff).CtrlRightArrow();
        }

        public void Del()
        {
            ((IBuffer)buff).Del(); // TODO: Fix color change
        }

        public void End()
        {
            ((IBuffer)buff).End();
        }

        public void Enter()
        {
            ((IBuffer)buff).Enter(); // TODO: Fix color change
        }

        public void Home()
        {
            ((IBuffer)buff).Home();
        }

        public int MaxYPos()
        {
            return ((IBuffer)buff).MaxYPos();
        }

        public void MoveDown()
        {
            ((IBuffer)buff).MoveDown();
        }

        public void MoveLeft()
        {
            ((IBuffer)buff).MoveLeft();
        }

        public void MoveRight()
        {
            ((IBuffer)buff).MoveRight();
        }

        public void MoveUp()
        {
            ((IBuffer)buff).MoveUp();
        }

        public void PerformOperation(OperationInfo oi)
        {
            ((IBuffer)buff).PerformOperation(oi);
        }

        public void Backspace()
        {
            if(_cursor.XPos == 0 && _cursor.YPos != 0)
            {
                _cursor.YPos--;
                _cursor.XPos = text[_cursor.YPos].Length - 1;
            } 
            else if(_cursor.XPos != 0)
            {
                _cursor.XPos--;
            }
            colorsBuff[_cursor.YPos][_cursor.XPos] = new CharColor();
            ((IBuffer)buff).Backspace();
        }

        public void CtrlBackspace()
        {
            throw new NotImplementedException();
        }

        public void Insert(string str)
        {
            if (cursor.XPos == buff[_cursor.YPos].Length) return;

            if (str == new string(' ', SSCPFEConfigurationApplication.GetTabSize()))
            {
                // impossible
            }
            else if (str.Contains("\n"))
            {
                // impossible
            }
            else if (str.Length != 1)
            {
                // impossible
            }
            else
            {
                colorsBuff[cursor.YPos][cursor.XPos] = (this[cursor.YPos][cursor.XPos] == str[0]) ? CharColor.Right : CharColor.Wrong;
                _cursor.XPos += str.Length;
            }

            if(cursor.XPos == Console.WindowWidth - 1)
            {
                _cursor.XPos = 0;
                _cursor.YPos++;
            }
        }

        public void LoadBuff(List<string> buff)
        {
            ((IBuffer)this.buff).LoadBuff(buff);
            foreach (string str in this.Buff())
            {
                List<CharColor> charColors = new List<CharColor>();
                foreach (char ch in str) charColors.Add(new CharColor());
                colorsBuff.Add(charColors);
            }
        }

        public void Print()
        {
            // TODO: Change to firstPrint and Print (another print with curr line only)
            Console.SetCursorPosition(defaultCursor.XPos, defaultCursor.YPos);
            for (int i = 0; i < text.Count; i++)
            {
                for (int j = 0; j < text[i].Length; j++) 
                {
                    char ch = text[i][j];
                    Console.ForegroundColor = colorsBuff[i][j].ForegroundColor;
                    Console.BackgroundColor = colorsBuff[i][j].BackgroundColor;
                    Console.Write(ch);
                }
                Console.SetCursorPosition(0, defaultCursor.YPos + i + 1);
            }
            Console.SetCursorPosition(defaultCursor.XPos + cursor.XPos, defaultCursor.YPos + cursor.YPos);
        }
    }

    class SSCPFETypingTestApplication : SSCPFEApplicationAbstract
    {
        public SSCPFETypingTestApplication() : base()
        {
            SetDeaultConsoleCfg();
            buff = new TypingTestBuffer("Some random text here and there, here and there, here and there... Some random text here and there, here and there, here and there...");
        }

        void HandleEsc()
        {

        }

        void HandleTab()
        {

        }

        public override void Mainloop()
        {
            while (true)
            {
                buff.Print();

                switch (kh.Handle()) // get input from user
                {
                    case KeyboardHandlerCommand.UpArrow:
                    case KeyboardHandlerCommand.DownArrow:
                    case KeyboardHandlerCommand.LeftArrow:
                    case KeyboardHandlerCommand.RightArrow:
                    case KeyboardHandlerCommand.Enter:
                    case KeyboardHandlerCommand.End:
                    case KeyboardHandlerCommand.Home:
                    case KeyboardHandlerCommand.CtrlV:
                    case KeyboardHandlerCommand.CtrlLeftArrow:
                    case KeyboardHandlerCommand.CtrlRightArrow:
                    case KeyboardHandlerCommand.CtrlDel:
                    case KeyboardHandlerCommand.Del:
                    case KeyboardHandlerCommand.CtrlZ:
                    case KeyboardHandlerCommand.CtrlY:
                        break;
                    case KeyboardHandlerCommand.Esc:
                        HandleEsc();
                        break;
                    case KeyboardHandlerCommand.Default:
                        buff.Insert("" + kh.LastKeyChar); 
                        break;
                    case KeyboardHandlerCommand.CtrlBackspace:
                        buff.CtrlBackspace();
                        break;
                    case KeyboardHandlerCommand.Backspace:
                        buff.Backspace();
                        break;
                    case KeyboardHandlerCommand.Tab:
                        HandleTab();
                        break;
                    default:
                        throw new SSCPFEHandlerException();
                }
            }
                // TODO: add
                // test will take 30 sec so it necessary to have
                // text about 250-300 words (it's impossible to rich)
                //
                // there will be only 5 commands:
                // tab (to start new test)
                // esc (to exit)
                // default
                // backspace (del prev char)
                // ctrl + backspace (del prev word)

                // After text is loaded it'll be loaded into buffer
                // (file or manually i don't know)
                // after that cursor will be place at (0; 0)
                // when you click the right char it will change color
                // if it's right char - green background and black fg
                // if not - red bg and ? fg

                // What about mistakes? (accuracy)

                // After 30 seconds (where is timer? (in title of cmd?))
                // the test'll automaticaly stop and result'll be displayed
                // with wpm (how to calc. wpm?)
                // with accuracy (?)
                // ask user to start again or exit
            }

    }
}
