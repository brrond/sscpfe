using System;
using System.IO;
using System.Collections.Generic;

namespace sscpfe
{
    class SSCPFEApplication
    {
        // Prev console settings
        int bufferWidth, bufferHeight;
        ConsoleColor cF, cB;
        int YPos;

        KeyboardHandler kh;
        Buffer buff;

        public string FName { get; private set; }

        public SSCPFEApplication()
        {
            bufferWidth = Console.BufferWidth;
            bufferHeight = Console.BufferHeight;
            cF = Console.ForegroundColor;
            cB = Console.BackgroundColor;
            YPos = Console.CursorTop;
            
            AppDomain.CurrentDomain.ProcessExit += (sender, e) =>
            {
                Console.ForegroundColor = cF;
                Console.BackgroundColor = cB;
                Console.SetBufferSize(bufferWidth, bufferHeight);
                Console.SetCursorPosition(0, YPos + buff.MaxYPos() + 5);
            };

            Console.SetBufferSize(16000, 16000);
            Console.ForegroundColor = ConsoleColor.Green;
            kh = new KeyboardHandler();
            buff = new Buffer(0, YPos);
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
                            if(File.Exists(FName))
                            {
                                Console.Write(FName + " exists. Do you want to proceed (Y/n) > ");
                                input = Console.ReadLine();
                                if (input.ToLower() != "y" && input != "")
                                    continue;
                            }
                            using (FileStream stream = new FileStream(FName, FileMode.Create))
                            {
                                StreamWriter streamWriter = new StreamWriter(stream);
                                bool first_line = true;
                                foreach (string str in buff.Buff())
                                {
                                    if(!first_line)
                                        streamWriter.WriteLine();
                                    else
                                        first_line = false;
                                    streamWriter.Write(str);
                                }
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
                    case HandlerCommand.Ctrl_V:
                        if (System.Windows.Forms.Clipboard.ContainsText())
                            buff.Insert(System.Windows.Forms.Clipboard.GetText());
                        break;
                    default:
                        throw new SSCPFEHandlerException();
                }

            }
        }
    }
}
