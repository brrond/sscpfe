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

        public override bool Equals(object obj)
        {
            return obj is CharColor color &&
                   base.Equals(obj) &&
                   Item1 == color.Item1 &&
                   Item2 == color.Item2 &&
                   ForegroundColor == color.ForegroundColor &&
                   BackgroundColor == color.BackgroundColor;
        }

        public override int GetHashCode()
        {
            int hashCode = 1789844046;
            hashCode = hashCode * -1521134295 + base.GetHashCode();
            hashCode = hashCode * -1521134295 + Item1.GetHashCode();
            hashCode = hashCode * -1521134295 + Item2.GetHashCode();
            hashCode = hashCode * -1521134295 + ForegroundColor.GetHashCode();
            hashCode = hashCode * -1521134295 + BackgroundColor.GetHashCode();
            return hashCode;
        }
    }
}
