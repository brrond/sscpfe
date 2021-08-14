using System;
using System.Collections.Generic;

namespace sscpfe
{
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

        public void Insert(string str)
        {
            buff[YPos] = buff[YPos].Insert(XPos++, str);
            XPos += str.Length - 1;
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
}
