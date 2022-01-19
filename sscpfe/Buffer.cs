using System;
using System.Collections.Generic;

namespace sscpfe
{
    class Buffer
    {
        List<string> buff;          // list of strings
        bool newLineFlag = false;   // 

        // default const
        public Buffer()
        {
            buff = new List<string>();  // empty list of strings
            buff.Add("");               // not empty actually
            XPos = 0;
            YPos = 0;
        }

        public Buffer(int DefaultX, int DefaultY) : this()
        {
            DefaultXPos = DefaultX;
            DefaultYPos = DefaultY;
        }

        // positions of cursor
        public int XPos { get; private set; }
        public int YPos { get; private set; }

        // save for reset (I think)
        public int DefaultXPos { get; private set; }
        public int DefaultYPos { get; private set; }

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
            // replace all tabs with "    " (4 spaces)
            for (int i = 0; i < buff.Count; i++)
                buff[i] = buff[i].Replace("\t", new string(' ', 4));
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

        // insert command (str is actually char)
        public void Insert(string str)
        {
            buff[YPos] = buff[YPos].Insert(XPos++, str);    // insert at XPos
            XPos += str.Length - 1;                         // add len of str to XPos
        }

        ///////////
        public void Backspace()
        {
            if (XPos != 0)
            {
                buff[YPos] = buff[YPos].Remove(--XPos, 1);  // just remove
                buff[YPos] += (char)0;                      // and add end of line on new end
            }
            else if(YPos != 0)
            {
                // delete \n
                XPos = buff[YPos - 1].Length;   // set new XPos 
                buff[YPos - 1] += buff[YPos];   // add this line to prev
                buff.RemoveAt(YPos);            // remove line
                YPos--;                         // set new YPos

                ClearConsole();
            }
        }

        // CTRL+BACKSPACE => delete prev word
        public void CtrlBackspace()
        {
            // if there are words (or blanks)
            if (XPos != 0) 
            {
                // all possible variants:
                // "some text " => "some "
                // "some text" => "some "
                int startOfThePrevWord = XPos - 1;
                while (startOfThePrevWord != 0 && buff[YPos][startOfThePrevWord] == ' ')
                        startOfThePrevWord--;

                int deleteFromHere = 0;
                int howManyToDelete = XPos;
                int newStart = 0;

                // either we reached the begining of the line
                if(startOfThePrevWord != 0)
                {
                    // find the begining of this word
                    while (startOfThePrevWord != 0 && buff[YPos][startOfThePrevWord] != ' ')
                        startOfThePrevWord--;

                    // either it's begining of the word
                    if (startOfThePrevWord != 0)
                    {
                        startOfThePrevWord++;
                        int dif = XPos - startOfThePrevWord;
                        deleteFromHere = startOfThePrevWord;
                        howManyToDelete = dif;
                        newStart = startOfThePrevWord;
                    }
                }
                // or we in prev word
                // or it's end (begining) of the line
                buff[YPos] = buff[YPos].Remove(deleteFromHere, howManyToDelete) + CreateEmptyLine(howManyToDelete + 1);
                XPos = newStart;
            }
            // delete some inappropriate symbol
            else if (buff[YPos].Length == 0)
            {
                buff[YPos] = CreateEmptyLine(1);
            }
        }

        public void Del()
        {
            if (XPos != buff[YPos].Length)
            {
                buff[YPos] = buff[YPos].Remove(XPos, 1);    // just remove
                buff[YPos] += (char)0;                      // and add end of line on new end
            }
            else if (YPos != buff.Count - 1)
            {
                // delete \n
                buff[YPos] += buff[YPos + 1];   // add next line to this
                buff.RemoveAt(YPos + 1);        // remove next line

                ClearConsole();
            }
        }

        public void CtrlDel()
        {
            // if there are words (or blanks)
            if (XPos != buff[YPos].Length)
            {
                int endOfTheNextWord = XPos + 1;
                while (endOfTheNextWord != buff[YPos].Length && buff[YPos][endOfTheNextWord] == ' ')
                    endOfTheNextWord++;

                int howManyToDelete = buff[YPos].Length - XPos;

                // either we reached the end of the line
                if (endOfTheNextWord != buff[YPos].Length)
                {
                    // find the end of this word
                    while (endOfTheNextWord != buff[YPos].Length && buff[YPos][endOfTheNextWord] != ' ')
                        endOfTheNextWord++;

                    // either it's end of the word
                    if (endOfTheNextWord != buff[YPos].Length)
                    {
                        //endOfTheNextWord++;
                        int dif = endOfTheNextWord - XPos + 1;
                        howManyToDelete = dif;
                    }
                }
                // or we in prev word
                // or it's end (begining) of the line
                buff[YPos] = buff[YPos].Remove(XPos, howManyToDelete) + CreateEmptyLine(howManyToDelete + 1);
            }
            // delete some inappropriate symbol
            else if (buff[YPos].Length == 0)
            {
                buff[YPos] = CreateEmptyLine(1);
            }
        }

        void ClearConsole()
        {
            // I DONT KNOW WHAT THE HECK
            buff.Add(CreateEmptyLine(1000));// Create new empty line (1000?)
            newLineFlag = true;             // ok
            int tmp = YPos + 1;             // some tmp var
            while (tmp != buff.Count - 1)   // while not last line
            {
                buff[tmp] += CreateEmptyLine(1000); // set empty line
                tmp++;
            }
        }

        public void Enter()
        {
            string additionalEmptyString = "";
            if (YPos + 1 < buff.Count) // if it's last line
            {
                additionalEmptyString = CreateEmptyLine(buff[YPos + 1].Length); // create new empty line next
            }

            // it's not last line
            buff.Insert(YPos + 1, buff[YPos].Substring(XPos) + additionalEmptyString); // split lines
            buff[YPos] = buff[YPos].Substring(0, XPos) + CreateEmptyLine(buff[YPos + 1].Length); // delete garbage
            YPos++; // new line
            XPos = 0; // new position

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
            else if(YPos < buff.Count - 1) // if there are some more lines
            {
                YPos++;
                XPos = 0;
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

        public void CtrlLeftArrow()
        {
            // place cursor before last word
            int newCursorPosition = XPos - 1;
            while(newCursorPosition > 0 && buff[YPos][newCursorPosition] == ' ')
                newCursorPosition--;
            while (newCursorPosition > 0 && buff[YPos][newCursorPosition] != ' ')
                newCursorPosition--;

            if (newCursorPosition == 0)
                XPos = 0;
            else
                XPos = newCursorPosition + 1;
        }

        public void CtrlRightArrow()
        {
            // place cursor after next word
            int newCursorPosition = XPos;
            while (newCursorPosition < buff[YPos].Length && buff[YPos][newCursorPosition] == ' ')
                newCursorPosition++;
            while (newCursorPosition < buff[YPos].Length && buff[YPos][newCursorPosition] != ' ')
                newCursorPosition++;
            XPos = newCursorPosition;
        }
    }
}
