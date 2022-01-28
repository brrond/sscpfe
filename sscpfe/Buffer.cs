using System;
using System.Collections.Generic;
using System.Reflection;

namespace sscpfe
{
    class Buffer
    {
        List<string> buff;          // list of strings
        int newLineCounter = 0;

        public Buffer()
        {
            buff = new List<string>
            {
                ""               // not empty actually
            };  // empty list of strings
            cursor.XPos = 0;
            cursor.YPos = 0;
        }

        public Buffer(int DefaultX, int DefaultY) : this()
        {
            defaultCursor.XPos = DefaultX;
            defaultCursor.YPos = DefaultY;
        }

        // positions of cursor
        public CursorPosition cursor;

        // save for reset
        public CursorPosition defaultCursor;

        // get string by i
        public string this[int i]
        {
            get { return buff[i]; }
        }

        // or loop it
        public IEnumerable<string> Buff()
        {
            foreach (string str in buff)
                yield return str;
        }

        // load from list
        public void LoadBuff(List<string> buff)
        {
            // replace all tabs with
            // (4 spaces)
            for (int i = 0; i < buff.Count; i++)
                buff[i] = buff[i].Replace("\t", new string(' ', SSCPFEConfigurationApplication.GetTabSize()));
            this.buff = buff;
        }

        // for END
        public int MaxYPos()
        {
            return buff.Count;
        }
        
        // returns an empty line with size len and has '\0' in the end
        string CreateEmptyLine(int len)
        {
            string line = "";
            for (int i = 0; i < len; i++)
                line += "\0";
            return line;
        }

        // print buffer
        public void Print()
        {
            Console.SetCursorPosition(defaultCursor.XPos, defaultCursor.YPos);
            for(int i = 0; i < buff.Count; i++)
            {
                Console.WriteLine(buff[i]);
                buff[i] = buff[i].Split('\0')[0];
            }
            //if (newLineFlag)
            while(newLineCounter != 0)
            {
                //newLineFlag = false;
                buff.RemoveAt(buff.Count - 1);
                newLineCounter--;
            }
            Console.SetCursorPosition(defaultCursor.XPos + cursor.XPos, defaultCursor.YPos + cursor.YPos);
        }


        void MultilineInsert(string str)
        {
            string[] split = str.Split('\n');
            for (int i = 0; i < split.Length - 1; i++)
            {
                Insert(split[i]);
                Enter();
            }
            Insert(split[split.Length - 1]);
        }

        // insert command (str is actually char)
        public void Insert(string str)
        {
            if (str.Contains("\n"))
            {
                MultilineInsert(str);
                return;
            }
            buff[cursor.YPos] = buff[cursor.YPos].Insert(cursor.XPos++, str);    // insert at XPos
            cursor.XPos += str.Length - 1;                         // add len of str to XPos
        }

        ///////////
        public void Backspace()
        {
            if (cursor.XPos != 0)
            {
                buff[cursor.YPos] = buff[cursor.YPos].Remove(--cursor.XPos, 1);  // just remove
                buff[cursor.YPos] += (char)0;                      // and add end of line on new end
            }
            else if(cursor.YPos != 0)
            {
                // delete \n
                cursor.XPos = buff[cursor.YPos - 1].Length;   // set new XPos 
                buff[cursor.YPos - 1] += buff[cursor.YPos];   // add this line to prev
                buff.RemoveAt(cursor.YPos);            // remove line
                cursor.YPos--;                         // set new YPos

                ClearConsole();
            }
        }

        // CTRL+BACKSPACE => delete prev word
        public void CtrlBackspace()
        {
            // if there are words (or blanks)
            if (cursor.XPos != 0) 
            {
                // all possible variants:
                // "some text " => "some "
                // "some text" => "some "
                int startOfThePrevWord = cursor.XPos - 1;
                while (startOfThePrevWord != 0 && buff[cursor.YPos][startOfThePrevWord] == ' ')
                        startOfThePrevWord--;

                int deleteFromHere = 0;
                int howManyToDelete = cursor.XPos;
                int newStart = 0;

                // either we reached the begining of the line
                if(startOfThePrevWord != 0)
                {
                    // find the begining of this word
                    while (startOfThePrevWord != 0 && buff[cursor.YPos][startOfThePrevWord] != ' ')
                        startOfThePrevWord--;

                    // either it's begining of the word
                    if (startOfThePrevWord != 0)
                    {
                        startOfThePrevWord++;
                        int dif = cursor.XPos - startOfThePrevWord;
                        deleteFromHere = startOfThePrevWord;
                        howManyToDelete = dif;
                        newStart = startOfThePrevWord;
                    }
                }
                // or we in prev word
                // or it's end (begining) of the line
                buff[cursor.YPos] = buff[cursor.YPos].Remove(deleteFromHere, howManyToDelete) + CreateEmptyLine(howManyToDelete + 1);
                cursor.XPos = newStart;
            }
            // delete some inappropriate symbol
            else if (buff[cursor.YPos].Length == 0)
            {
                //***buff[YPos] = CreateEmptyLine(1);
            }
        }

        public void Del()
        {
            if (cursor.XPos != buff[cursor.YPos].Length)
            {
                buff[cursor.YPos] = buff[cursor.YPos].Remove(cursor.XPos, 1);    // just remove
                buff[cursor.YPos] += (char)0;                      // and add end of line on new end
            }
            else if (cursor.YPos != buff.Count - 1)
            {
                // delete \n
                buff[cursor.YPos] += buff[cursor.YPos + 1];   // add next line to this
                buff.RemoveAt(cursor.YPos + 1);        // remove next line

                ClearConsole();
            }
        }

        public void CtrlDel()
        {
            // if there are words (or blanks)
            if (cursor.XPos != buff[cursor.YPos].Length)
            {
                int endOfTheNextWord = cursor.XPos + 1;
                while (endOfTheNextWord != buff[cursor.YPos].Length && buff[cursor.YPos][endOfTheNextWord] == ' ')
                    endOfTheNextWord++;

                int howManyToDelete = buff[cursor.YPos].Length - cursor.XPos;

                // either we reached the end of the line
                if (endOfTheNextWord != buff[cursor.YPos].Length)
                {
                    // find the end of this word
                    while (endOfTheNextWord != buff[cursor.YPos].Length && buff[cursor.YPos][endOfTheNextWord] != ' ')
                        endOfTheNextWord++;

                    // either it's end of the word
                    if (endOfTheNextWord != buff[cursor.YPos].Length)
                    {
                        //endOfTheNextWord++;
                        int dif = endOfTheNextWord - cursor.XPos + 1;
                        howManyToDelete = dif;
                    }
                }
                // or we in prev word
                // or it's end (begining) of the line
                buff[cursor.YPos] = buff[cursor.YPos].Remove(cursor.XPos, howManyToDelete) + CreateEmptyLine(howManyToDelete + 1);
            }
            // delete some inappropriate symbol
            else if (buff[cursor.YPos].Length == 0)
            {
                //***buff[YPos] = CreateEmptyLine(1);
            }
        }

        void ClearConsole()
        {
            // TODO: Fix magical 1000
            // This works with one line
            // I DONT KNOW WHAT THE HECK
            //buff.Add(CreateEmptyLine(1000));// Create new empty line (1000?)
            //newLineFlag = true;             // ok
            // BUT what if we have multiple new lines
            buff.Add(CreateEmptyLine(1000));
            newLineCounter++;
            int tmp = cursor.YPos + 1;             // some tmp var
            while (tmp != buff.Count - 1)   // while not last line
            {
                buff[tmp] += CreateEmptyLine(1000); // set empty line
                tmp++;
            }
        }

        public void Enter()
        {
            string additionalEmptyString = "";
            if (cursor.YPos + 1 < buff.Count) // if it's last line
            {
                additionalEmptyString = CreateEmptyLine(buff[cursor.YPos + 1].Length); // create new empty line next
            }

            // it's not last line
            buff.Insert(cursor.YPos + 1, buff[cursor.YPos].Substring(cursor.XPos) + additionalEmptyString); // split lines
            buff[cursor.YPos] = buff[cursor.YPos].Substring(0, cursor.XPos) + CreateEmptyLine(buff[cursor.YPos + 1].Length); // delete garbage
            cursor.YPos++; // new line
            cursor.XPos = 0; // new position

            // there is still one problem.
            // If we splited string (or just added one)
            // we still need to remove garbage 
            // from next strings
            // Thats why:
            ClearConsole();
        }

        // Just change XPos and YPos with conditions
        public void MoveUp()
        {
            if (cursor.YPos != 0)
            {
                cursor.YPos--;
                if (cursor.XPos > buff[cursor.YPos].Length)
                    cursor.XPos = buff[cursor.YPos].Length;
            }
        }

        public void MoveDown()
        {
            if (cursor.YPos < buff.Count - 1)
            {
                cursor.YPos++;
                if (cursor.XPos > buff[cursor.YPos].Length)
                    cursor.XPos = buff[cursor.YPos].Length;
            }
        }

        public void MoveLeft()
        {
            if (cursor.XPos != 0)
            {
                cursor.XPos--;
            }
            else if(cursor.YPos != 0)
            {
                cursor.XPos = buff[--cursor.YPos].Length;
            }
        }

        public void MoveRight()
        {
            if (cursor.XPos < buff[cursor.YPos].Length)
            {
                cursor.XPos++;
            }
            else if(cursor.YPos < buff.Count - 1) // if there are some more lines
            {
                cursor.YPos++;
                cursor.XPos = 0;
            }
        }

        public void Home()
        {
            cursor.XPos = 0;
        }

        public void End()
        {
            cursor.XPos = buff[cursor.YPos].Length;
        }

        public void CtrlLeftArrow()
        {
            // place cursor before last word
            int newCursorPosition = cursor.XPos - 1;
            while(newCursorPosition > 0 && buff[cursor.YPos][newCursorPosition] == ' ')
                newCursorPosition--;
            while (newCursorPosition > 0 && buff[cursor.YPos][newCursorPosition] != ' ')
                newCursorPosition--;

            if (newCursorPosition == 0)
                cursor.XPos = 0;
            else
                cursor.XPos = newCursorPosition + 1;
        }

        public void CtrlRightArrow()
        {
            // place cursor after next word
            int newCursorPosition = cursor.XPos;
            while (newCursorPosition < buff[cursor.YPos].Length && buff[cursor.YPos][newCursorPosition] == ' ')
                newCursorPosition++;
            while (newCursorPosition < buff[cursor.YPos].Length && buff[cursor.YPos][newCursorPosition] != ' ')
                newCursorPosition++;
            cursor.XPos = newCursorPosition;
        }

        public void PerformOperation(OperationInfo oi)
        {
            int r = oi.Repeats;
            cursor.XPos = oi.XPos;
            cursor.YPos = oi.YPos;
            while(r > 0)
            {
                oi.Method.Invoke(this, oi.Parametrs);
                r--;
            }
        }

    }
}
