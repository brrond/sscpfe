using System;
using System.Collections.Generic;
using System.Linq;

namespace sscpfe
{
    class TypingTestBuffer : IEditorBuffer
    {
        // Fields
        readonly Buffer buff;
        List<List<CharColor>> colorsBuff;
        List<List<int>> entries; // 0 - not found
                                 // 1 - true
                                 // -1 - false

        public List<List<int>> Entries => entries;
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

        void init(string Text)
        {
            colorsBuff = new List<List<CharColor>>();
            LoadText(Text);
            _defaultCursor = ((IBuffer)buff).defaultCursor;
            entries = new List<List<int>>();
            for (int i = 0; i < text.Count; i++)
            {
                entries.Add(new List<int>());
                for (int j = 0; j < text[i].Length; j++) entries[i].Add(0);
            }
        }

        // Constructors
        public TypingTestBuffer(string Text)
        {
            buff = new Buffer();
            init(Text);
        }

        public TypingTestBuffer(int DefaultX, int DefaultY, string Text)
        {
            buff = new Buffer(DefaultX, DefaultY);
            init(Text);
        }

        // IBuffer fields
        public string this[int i] => ((IBuffer)buff)[i];

        CursorPosition _cursor, _defaultCursor;

        public CursorPosition cursor => _cursor;

        public CursorPosition defaultCursor => _defaultCursor;


        // Not used methods
        public IEnumerable<string> Buff()
        {
            return ((IBuffer)buff).Buff();
        }

        public void CtrlDel()
        {
            ((IDeletable)buff).CtrlDel();
        }

        public void CtrlLeftArrow()
        {
            ((IMovable)buff).CtrlLeftArrow();
        }

        public void CtrlRightArrow()
        {
            ((IMovable)buff).CtrlRightArrow();
        }

        public void Del()
        {
            ((IDeletable)buff).Del(); 
        }

        public void End()
        {
            ((IMovable)buff).End();
        }

        public void Enter()
        {
            ((IBuffer)buff).Enter();
        }

        public void Home()
        {
            ((IMovable)buff).Home();
        }

        public int MaxYPos()
        {
            return ((IBuffer)buff).MaxYPos();
        }

        public void MoveDown()
        {
            ((IMovable)buff).MoveDown();
        }

        public void MoveLeft()
        {
            ((IMovable)buff).MoveLeft();
        }

        public void MoveRight()
        {
            ((IMovable)buff).MoveRight();
        }

        public void MoveUp()
        {
            ((IMovable)buff).MoveUp();
        }

        public void PerformOperation(OperationInfo oi)
        {
            ((IBuffer)buff).PerformOperation(oi);
        }
        // Not used methods


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
            ((IDeletable)buff).Backspace();
        }

        public void CtrlBackspace()
        {
            // TODO: Insert ctrl+backspace
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
                if (this[cursor.YPos][cursor.XPos] == str[0]) {
                    colorsBuff[cursor.YPos][cursor.XPos] = CharColor.Right;
                    if (entries[cursor.YPos][cursor.XPos] == 0) entries[cursor.YPos][cursor.XPos] = 1;
                }
                else
                {
                    colorsBuff[cursor.YPos][cursor.XPos] = CharColor.Wrong;
                    entries[cursor.YPos][cursor.XPos] = -1;
                }
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
                List<CharColor> charColors = (from char ch in str select new CharColor()).ToList();
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
