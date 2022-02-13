using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading;

namespace sscpfe
{

    class SSCPFETypingTestApplication : SSCPFEApplicationAbstract
    {
        bool stopwatchThreadState;
        Thread stopwatchTread;
        string text;

        string LoadText()
        {
            string html = new WebClient().DownloadString("https://randomtextgenerator.com/");
            string split = html.Split(new string[] { "<div id=\"randomtext_box\">" }, StringSplitOptions.None)[1].Trim();
            split = split.Split(new string[] { "<!-- RandomTextGenerator.com Top -->" }, StringSplitOptions.None)[0].Trim();
            split = split.Replace(" <br />\r\n<br />\r\n", " ");
            split = split.Replace(" \t\t\n\t\n</div>", "");
            split = split.Trim();
            text = split.Substring(0, 1500); // 1500 chars it's max
                                             // because I used info, that the fastes
                                             // typist was 216 wpm, so I used 300 chars * 5 = 1500
            return text;
        }

        void PrepareNewTest()
        {
            text = LoadText();
            buff = new TypingTestBuffer(text);
            if (stopwatchTread != null) stopwatchTread.Abort();
            stopwatchThreadState = true;
            stopwatchTread = new Thread(UpdateTimer);
        }

        public SSCPFETypingTestApplication() : base()
        {
            PrepareNewTest();
        }

        void HandleEsc()
        {
            stopwatchThreadState = false;
            if(AskUser("Do you want to exit (y/n)>"))
            {
                Environment.Exit(0);
            }
            stopwatchThreadState = true;
        }

        void HandleTab()
        {
            stopwatchThreadState = false;
            if(AskUser("Do you want to start another test? (y/n)>"))
            {
                PrepareNewTest(); // TODO: Start another test after first one is done
            }
            else
            {
                stopwatchThreadState = true;
            }
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

                if(!stopwatchThreadState) stopwatch.Stop();
                else if(stopwatchThreadState && !stopwatch.IsRunning) stopwatch.Start();
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
            while (it.MoveNext()) Console.WriteLine(new string(' ', 1000)); // TODO: Fix magic 1000
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
            double NetWPM = GrossWPM - (uncorrectedErrors / 5.0 / 0.5);

            int correctEntries = 0;
            List<List<int>> entries = (buff as TypingTestBuffer).Entries;
            for(int i = 0; i < entries.Count; i++)
                for(int j = 0; j < entries[i].Count; j++)
                    if (entries[i][j] == 1) correctEntries++;

            double Accuracy = correctEntries / (double)allTypedEntries;
            double CorrectedAccuracy = correctedErrors / (double)allTypedEntries;

             Console.WriteLine("Chars : {0}\nGrossWPM : {1}\nNetWPM : {2}\nAccuracy : {3}\nCorrected accuracy : {4}", 
                allTypedEntries, GrossWPM.ToString("F2"), NetWPM.ToString("F2"), (Accuracy * 100).ToString("F2"), 
                (CorrectedAccuracy * 100).ToString("F2"));
            Console.WriteLine("If you want to start again use TAB, or ESC to exist");
            while (true)
            {
                switch (kh.Handle()) 
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
                    case KeyboardHandlerCommand.Default:
                    case KeyboardHandlerCommand.CtrlBackspace:
                    case KeyboardHandlerCommand.Backspace:
                        break;
                    case KeyboardHandlerCommand.Esc:
                        HandleEsc();
                        break;
                    case KeyboardHandlerCommand.Tab:
                        HandleTab();
                        break;
                    default:
                        throw new SSCPFEHandlerException();
                }
            }
        }

    }
}
