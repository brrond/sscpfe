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

        public static bool operator ==(CharColor ch1, CharColor ch2) => 
            (ch1.ForegroundColor == ch2.ForegroundColor) && (ch1.BackgroundColor == ch2.BackgroundColor);

        public static bool operator !=(CharColor ch1, CharColor ch2) => !(ch1 == ch2);
    }
}
