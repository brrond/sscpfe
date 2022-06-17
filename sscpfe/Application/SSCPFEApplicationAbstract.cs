using System;
using System.Text;

namespace sscpfe
{
    abstract class SSCPFEApplicationAbstract : IApp
    {
        // Prev console config
        protected int YPos;
        protected string title;
        protected int bufferWidth;
        protected int bufferHeight;
        protected ConsoleColor cF;
        protected ConsoleColor cB;
        protected Encoding inputEncoding;
        protected Encoding outputEncoding;

        // New console config
        protected Encoding encoding = Encoding.UTF8;

        // 
        protected IEditorBuffer buff;
        protected KeyboardHandler kh;

        protected SSCPFEApplicationAbstract()
        {
            // save cfg
            bufferWidth = Console.BufferWidth;
            bufferHeight = Console.BufferHeight;
            cF = Console.ForegroundColor;
            cB = Console.BackgroundColor;
            YPos = Console.CursorTop;
            inputEncoding = Console.InputEncoding;
            outputEncoding = Console.OutputEncoding;
            title = Console.Title;

            // when application is closed return prev console config
            AppDomain.CurrentDomain.ProcessExit += (sender, e) =>
            {
                Console.ForegroundColor = cF;
                Console.BackgroundColor = cB;
                Console.SetBufferSize(bufferWidth, bufferHeight);
                Console.SetCursorPosition(0, YPos + Console.WindowHeight + 7);
                Console.OutputEncoding = outputEncoding;
                Console.InputEncoding = inputEncoding;
                Console.Title = title;
            };

            kh = new KeyboardHandler(); // init KeyboardHandler

            // set default console cfg
            Console.SetBufferSize(Console.WindowWidth + 1, Console.BufferHeight);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;
            Console.Title = "sscpfe";
        }

        protected bool AskUser(string msg)
        {
            bool res;
            do
            {
                Console.Write(msg);
                string key = Console.ReadKey().Key.ToString().ToLower().Trim();
                if (key == "y")
                {
                    res = true;
                    break;
                }
                else if (key == "n")
                {
                    res = false;
                    break;
                }

                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write(new string(' ', msg.Length + 1));
                Console.SetCursorPosition(0, Console.CursorTop);

            } while (true);
            Console.WriteLine();
            return res;
        }

        public abstract void Mainloop();
    }
}