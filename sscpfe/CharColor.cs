using System;

namespace sscpfe
{
    class CharColor : Tuple<ConsoleColor, ConsoleColor>
    {
        public CharColor() : this(ConsoleColor.Green, ConsoleColor.Black)
        {

        }

        public CharColor(ConsoleColor fg, ConsoleColor bg) : base(fg, bg)
        {

        }

        public ConsoleColor ForegroundColor => Item1;
        public ConsoleColor BackgroundColor => Item2;

        public static CharColor Wrong => new CharColor(ConsoleColor.Black, ConsoleColor.Red);

        public static CharColor Right => new CharColor(ConsoleColor.Black, ConsoleColor.Green);
    }
}
