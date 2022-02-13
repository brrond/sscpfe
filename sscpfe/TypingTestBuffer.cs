using System;
using System.Collections.Generic;

namespace sscpfe
{
    class TypingTestBuffer : IBuffer
    {
        Buffer buff;
        List<List<CharColor>> colorsBuff;

        public List<List<CharColor>> ColorsBuff => colorsBuff;

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

        
        bool firstPrintFlag = true;
        void FirstPrint()
        {
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

        public void Print()
        {
            if(firstPrintFlag)
            {
                FirstPrint();
                firstPrintFlag = false;
                return;
            }
            // only one line
            Console.SetCursorPosition(0, cursor.YPos);
            for (int j = 0; j < text[_cursor.YPos].Length; j++)
            {
                char ch = text[_cursor.YPos][j];
                Console.ForegroundColor = colorsBuff[_cursor.YPos][j].ForegroundColor;
                Console.BackgroundColor = colorsBuff[_cursor.YPos][j].BackgroundColor;
                Console.Write(ch);
            }
            Console.SetCursorPosition(defaultCursor.XPos + cursor.XPos, defaultCursor.YPos + cursor.YPos);
        }
    }
}
