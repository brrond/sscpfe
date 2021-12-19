using System;

namespace sscpfe
{
    /// <summary>
    /// Class create to handle user keyboard input
    /// </summary>
    class KeyboardHandler
    {
        // default constructor
        public KeyboardHandler()
        {

        }

        // return some of HandlerCommand enum element
        public HandlerCommand Handle()
        {
            ConsoleKeyInfo info = Console.ReadKey();                        // get user key
            switch(info.Key.ToString())                                     // if it's
            {
                case "UpArrow": return HandlerCommand.UpArrow;              // move up
                case "DownArrow": return HandlerCommand.DownArrow;          // 
                case "LeftArrow":
                    if (info.Modifiers.HasFlag(ConsoleModifiers.Control))
                        return HandlerCommand.CtrlLeftArrow;
                    return HandlerCommand.LeftArrow;                        //
                case "RightArrow":
                    if (info.Modifiers.HasFlag(ConsoleModifiers.Control))
                        return HandlerCommand.CtrlRightArrow;
                    return HandlerCommand.RightArrow;                       //
                case "Backspace":                                           // backspace
                    if (info.Modifiers.HasFlag(ConsoleModifiers.Control))   // with control (ctrl + backspace)
                        return HandlerCommand.CtrlBackspace;               
                    return HandlerCommand.Backspace;                        // regular backspace
                case "Delete":                                              // delete
                    if (info.Modifiers.HasFlag(ConsoleModifiers.Control))   // with control (del + backspace)
                        return HandlerCommand.CtrlDel;
                    return HandlerCommand.Del;                              // regular del
                case "Enter": return HandlerCommand.Enter;                  // new line
                case "Escape": return HandlerCommand.Esc;                   // exit
                case "Home": return HandlerCommand.Home;                    // move
                case "End": return HandlerCommand.End;                      // move
                case "Tab": return HandlerCommand.Tab;                      // tab command
                case "V":                                                   //
                case "v":                                                   // 
                    if (info.Modifiers.HasFlag(ConsoleModifiers.Control))   // has ctrl flag
                        return HandlerCommand.CtrlV;                       // ctrl + V
                    goto default;
                default:
                    LastKey = info.Key;                                     // insert
                    LastKeyChar = info.KeyChar;                             // this char
                    break;
            }
            return HandlerCommand.Default;                                  // default status
        }

        public ConsoleKey LastKey { get; private set; }
        public char LastKeyChar { get; private set; }
    }
}
