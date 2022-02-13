using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading;

namespace sscpfe
{

    class SSCPFETypingTestApplication : SSCPFEApplicationAbstract
    {
        Thread stopwatchTread;
        string text;
        public SSCPFETypingTestApplication() : base()
        {
            // TODO: A lot of text
            string html = new WebClient().DownloadString("https://randomtextgenerator.com/");
            string split = html.Split(new string[] { "<div id=\"randomtext_box\">" }, StringSplitOptions.None)[1].Trim();
            split = split.Split(new string[] { "<!-- RandomTextGenerator.com Top -->" }, StringSplitOptions.None)[0].Trim();
            split = split.Replace(" <br />\r\n<br />\r\n", " ");
            split = split.Replace(" \t\t\n\t\n</div>", "");
            split = split.Trim();
            text = split;

            buff = new TypingTestBuffer(text);
            stopwatchTread = new Thread(UpdateTimer);
        }

        void HandleEsc()
        {
            // TODO: Add esc
        }

        void HandleTab()
        {
            // TODO: Add tab
        }

        void UpdateTimer()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            do
            {
                double elapsed = stopwatch.Elapsed.TotalSeconds;
                Console.Title = elapsed.ToString();
                if(elapsed >= 30)
                {
                    stopwatch.Stop();
                    Console.Title = "Done";
                    break;
                }
            } while (true);
        }

        public override void Mainloop()
        {
            while (true)
            {
                buff.Print();                
                if(stopwatchTread.ThreadState == System.Threading.ThreadState.Stopped) break;

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
                        if (stopwatchTread.ThreadState == System.Threading.ThreadState.Unstarted) stopwatchTread.Start();
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

            // clear console from text
            Console.SetCursorPosition(buff.defaultCursor.XPos, buff.defaultCursor.YPos);
            var it = buff.Buff().GetEnumerator();
            while (it.MoveNext()) Console.WriteLine(new string(' ', 1000));
            Console.WriteLine(new string(' ', 1000));
            Console.SetCursorPosition(buff.defaultCursor.XPos, buff.defaultCursor.YPos); // + 2?

            // calculate wpm
            int allTypedEntries = 0;
            int uncorrectedErrors = 0;
            int correctedErrors = 0;
            List<List<CharColor>> colors = (buff as TypingTestBuffer).ColorsBuff;
            for (int i = 0; i < colors.Count; i++)
            {
                for (int j = 0; j < colors[i].Count; j++)
                {
                    if (colors[i][j] == CharColor.Wrong) uncorrectedErrors++;
                    else if (colors[i][j] == CharColor.Right) correctedErrors++;
                    else break;
                    allTypedEntries++;
                }
            }

            double GrossWPM = (allTypedEntries / 5.0) / 0.5;
            double NetWPM = GrossWPM - (uncorrectedErrors / 5 / 0.5);
            double Accuracy = correctedErrors / allTypedEntries; // TODO: Should use correct entries only

            Console.WriteLine("Chars : {0}\nGrossWPM : {1}\nNetWPM : {2}\nAccuracy : {3}", 
                allTypedEntries, GrossWPM, NetWPM, Accuracy * 100);
            Console.Read();
            // TODO: Ask user to start again
        }

    }
}
