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
    }
}
