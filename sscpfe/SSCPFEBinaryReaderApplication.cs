using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace sscpfe
{
    class SSCPFEBinaryReaderApplication : IApp
    {
        string title;
        ConsoleColor cF, cB;

        public string FName { private set; get; }

        public SSCPFEBinaryReaderApplication(string fname)
        {
            FName = fname;
            cF = Console.ForegroundColor;
            cB = Console.BackgroundColor;
            title = Console.Title;

            AppDomain.CurrentDomain.ProcessExit += (sender, e) =>
            {
                Console.ForegroundColor = cF;
                Console.BackgroundColor = cB;
                Console.Title = title;
            };

            Console.ForegroundColor = ConsoleColor.Green; // I watched Matrix recently
            Console.Title = "sscpfe_binary_reader - " + FName;
        }

        public void Mainloop()
        {
            if(File.Exists(FName))
            {
                //-------------------------------------------------------------------
                //|addr    |values                                         |char    |
                //|-----------------------------------------------------------------|
                //|00000000|00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00|00000000|
                //-------------------------------------------------------------------
                // TODO: Fix binary reader
                string hLine = new string('-', 16 * 3 - 1 + 2 + 16 + 8);
                Console.WriteLine("-{0}-", hLine);
                Console.WriteLine("|{0,8}|{1,47}|{2,16}|", "addr", "values", "char");
                Console.WriteLine("|{0}|", hLine);
                using (FileStream fileStream = new FileStream(FName, FileMode.Open))
                using (StreamReader reader = new StreamReader(fileStream))
                {
                    int position = 0;
                    string str = "";
                    while(reader.Peek() != -1)
                    {
                        int currChar = reader.Read();
                        str += (char)currChar;
                        if (position % 16 == 0)
                            Console.Write("|{0,8}|{1,2}", position.ToString("X8"), currChar.ToString("X2"));
                        else
                            Console.Write(" {0,2}", currChar.ToString("X2"));

                        if((position + 1) % 16 == 0)
                        {
                            str = Regex.Replace(str, @"[^\u0000-\u007F]+", ".").
                                Replace('\n', '.').Replace('\r', '.').Replace('\t', '.');
                            Console.Write("|{0}|\n", str);
                            str = "";
                        }

                        position++;
                    }
                    if(str != "")
                    {
                        str = Regex.Replace(str, @"[^\u0000-\u007F]+", ".").
                                Replace('\n', '.').Replace('\r', '.').Replace('\t', '.');
                        Console.Write("{0," + ((16 * 3 - 1) - (str.Length * 3 - 1) + 1) + "}{1}", "|", str);
                        Console.Write("{0}|\n", new string(' ', 16 - str.Length));
                    }
                    Console.WriteLine("-{0}-", hLine);
                }
                Console.WriteLine();

            }
            else
            {
                Console.WriteLine("File doesn't exist");
            }
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}
