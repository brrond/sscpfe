namespace sscpfe
{
    abstract class Operation
    {
        protected int xPosBefore, yPosBefore;
        protected int xPosAfter, yPosAfter;

        public Operation(int XPosBefore, int YPosBefore, int XPosAfter, int YPosAfter)
        {
            xPosBefore = XPosBefore;
            yPosBefore = YPosBefore;
            xPosAfter = XPosAfter;
            yPosAfter = YPosAfter;
        }

        abstract public OperationInfo Undo();
        abstract public OperationInfo Redo();
    }
}
