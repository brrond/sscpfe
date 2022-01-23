using System;
using System.Reflection;

namespace sscpfe
{
    class InsertOperation : Operation
    {
        string str = "";
        public InsertOperation(int XPosBefore, int YPosBefore, int XPosAfter, int YPosAfter, string Str) : base(XPosBefore, YPosBefore, XPosAfter, YPosAfter)
        {
            str = Str;
        }

        public override OperationInfo Redo()
        {
            Type buffType = typeof(Buffer);
            MethodInfo methodInfo;
            if (str == "\n")
            {
                methodInfo = buffType.GetMethod("Enter");
                return new OperationInfo(xPosBefore, yPosBefore, 1, methodInfo,
                    new object[] { });
            }
            else
            {
                methodInfo = buffType.GetMethod("Insert");
                return new OperationInfo(xPosBefore, yPosBefore, 1, methodInfo,
                    new object[] { str });
            }
        }

        public override OperationInfo Undo()
        {
            Type buffType = typeof(Buffer);
            MethodInfo methodInfo = buffType.GetMethod("Backspace");
            return new OperationInfo(xPosAfter, yPosAfter, str.Length, methodInfo,
                new object[] { });
        }
    }
}
