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
                int len = buff[YPos].Length;    // get len of del string
                buff[YPos - 1] += buff[YPos];   // add this line to prev
                buff.RemoveAt(YPos);            // remove line
                YPos--;                         // set new YPos

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
        }

        ///////////
        public void Ctrl_Backspace()
        {
            if(XPos != 0)
            {
                // "some text " => "some "
                int end_of_the_last_word = -1;
                for(int i = buff[YPos].Length - 1; i >= 0; i--)
                {
                    if(end_of_the_last_word == -1 && buff[YPos][i] != ' ')
                    {
                        end_of_the_last_word = i;
                    }
                    else if(end_of_the_last_word != -1 && buff[YPos][i] == ' ')
                    {
                        int dif = XPos - i;
                        buff[YPos] = buff[YPos].Remove(i, dif) + CreateEmptyLine(dif + 1);
                        XPos -= dif;
                        return; //break;
                    }
                }
                buff[YPos] = CreateEmptyLine(buff[YPos].Length + 1);
                XPos = 0;
            }
        }

        public void Enter()
        {
            string additionalEmptyString = "";
            if (YPos + 1 < buff.Count) // if it's last line
            {
                additionalEmptyString = CreateEmptyLine(buff[YPos + 1].Length); // create new empty line next
            }

            // IFJEKFJEKFEJEKFJEKJFKEWJFKEJK
            buff.Insert(YPos + 1, buff[YPos].Substring(XPos) + additionalEmptyString);
            buff[YPos] = buff[YPos].Substring(0, XPos) + CreateEmptyLine(buff[YPos + 1].Length);
            YPos++;
            XPos = 0;
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
}
