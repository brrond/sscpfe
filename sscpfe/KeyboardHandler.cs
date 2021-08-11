using System;

namespace sscpfe
{
    class KeyboardHandler
    {
        public KeyboardHandler()
        {

        }

        public HandlerCommand Handle()
        {
            ConsoleKeyInfo info = Console.ReadKey();
            switch(info.Key.ToString())
            {
                case "UpArrow": return HandlerCommand.UpArrow;
                case "DownArrow": return HandlerCommand.DownArrow;
                case "LeftArrow": return HandlerCommand.LeftArrow;
                case "RightArrow": return HandlerCommand.RightArrow;
                case "Backspace": return HandlerCommand.Backspace;
                case "Enter": return HandlerCommand.Enter;
                case "Escape": return HandlerCommand.Esc;
                case "Home": return HandlerCommand.Home;
                case "End": return HandlerCommand.End;
                default:
                    LastKey = info.Key;
                    LastKeyChar = info.KeyChar;
                    break;
            }
            return HandlerCommand.Default;
        }

        public ConsoleKey LastKey { get; private set; }
        public char LastKeyChar { get; private set; }
    }
}
