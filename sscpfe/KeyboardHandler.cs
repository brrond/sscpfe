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
        public KeyboardHandlerCommand Handle()
        {
            ConsoleKeyInfo info = Console.ReadKey(true);                        // get user key
            switch(info.Key.ToString())                                     // if it's
            {
                case "UpArrow": return KeyboardHandlerCommand.UpArrow;      // move up
                case "DownArrow": return KeyboardHandlerCommand.DownArrow;  // 
                case "LeftArrow":
                    if (info.Modifiers.HasFlag(ConsoleModifiers.Control))   // 
                        return KeyboardHandlerCommand.CtrlLeftArrow;        // move left one word
                    return KeyboardHandlerCommand.LeftArrow;                // move left one char
                case "RightArrow":
                    if (info.Modifiers.HasFlag(ConsoleModifiers.Control))   
                        return KeyboardHandlerCommand.CtrlRightArrow;       // move right one word
                    return KeyboardHandlerCommand.RightArrow;               // move left one char
                case "Backspace":                                           // backspace
                    if (info.Modifiers.HasFlag(ConsoleModifiers.Control))   // with control (ctrl + backspace)
                        return KeyboardHandlerCommand.CtrlBackspace;               
                    return KeyboardHandlerCommand.Backspace;                // regular backspace
                case "Delete":                                              // delete
                    if (info.Modifiers.HasFlag(ConsoleModifiers.Control))   // with control (del + backspace)
                        return KeyboardHandlerCommand.CtrlDel;
                    return KeyboardHandlerCommand.Del;                      // regular del
                case "Enter": return KeyboardHandlerCommand.Enter;          // new line
                case "Escape": return KeyboardHandlerCommand.Esc;           // exit
                case "Home": return KeyboardHandlerCommand.Home;            // move
                case "End": return KeyboardHandlerCommand.End;              // move
                case "Tab": return KeyboardHandlerCommand.Tab;              // tab command
                case "V":                                                   //
                case "v":                                                   // 
                    if (info.Modifiers.HasFlag(ConsoleModifiers.Control))   // has ctrl flag
                        return KeyboardHandlerCommand.CtrlV;                // ctrl + V
                                                                            // TODO: actually it is shift ctrl v
                                                                            // doesn't work
                    goto default;
                case "Z":
                case "z":
                    if (info.Modifiers.HasFlag(ConsoleModifiers.Control))   // has ctrl flag
                        return KeyboardHandlerCommand.CtrlZ;                // ctrl + Z
                    goto default;
                default:
                    LastKey = info.Key;                                     // insert
                    LastKeyChar = info.KeyChar;                             // this char
                    break;
            }
            return KeyboardHandlerCommand.Default;                          // default status
        }

        public ConsoleKey LastKey { get; private set; }
        public char LastKeyChar { get; private set; }
    }
}
