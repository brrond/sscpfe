namespace sscpfe
{
    struct CursorPosition
    {
        public int XPos { set; get; }
        public int YPos { set; get; }

        public CursorPosition(int x, int y)
        {
            XPos = x;
            YPos = y;
        }

        public static bool operator ==(CursorPosition p1, CursorPosition p2) => (p1.XPos == p2.XPos) && (p1.YPos == p2.YPos);
        public static bool operator !=(CursorPosition p1, CursorPosition p2) => !(p1 == p2);
        public static bool operator >(CursorPosition p1, CursorPosition p2)
        {
            if (p1.YPos > p2.YPos) return true;
            else if (p1.YPos < p2.YPos) return false;
            return p1.XPos > p2.XPos;
        }
        public static bool operator <(CursorPosition p1, CursorPosition p2)
        {
            if (p1.YPos > p2.YPos) return true;
            else if (p1.YPos < p2.YPos) return false;
            return p1.XPos < p2.XPos;
        }
        public static bool operator >=(CursorPosition p1, CursorPosition p2) => (p1 > p2) || (p1 == p2);
        public static bool operator <=(CursorPosition p1, CursorPosition p2) => (p1 < p2) || (p1 == p2);

        public override bool Equals(object obj)
        {
            return obj is CursorPosition position &&
                   XPos == position.XPos &&
                   YPos == position.YPos;
        }
        public override int GetHashCode()
        {
            int hashCode = -1416500419;
            hashCode = hashCode * -1521134295 + XPos.GetHashCode();
            hashCode = hashCode * -1521134295 + YPos.GetHashCode();
            return hashCode;
        }
    }
}
