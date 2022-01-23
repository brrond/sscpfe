using System;
using System.Reflection;

namespace sscpfe
{
    class DeleteOperation : Operation
    {
        char ch = '\0';
        string str = "";
        bool isDel;
        public DeleteOperation(int XPosBefore, int YPosBefore, int XPosAfter, int YPosAfter, char Ch, bool isDel) :
            base(XPosBefore, YPosBefore, XPosAfter, YPosAfter)
        {
            ch = Ch;
            this.isDel = isDel;
        }

        public DeleteOperation(int XPosBefore, int YPosBefore, int XPosAfter, int YPosAfter, string Str, bool isDel) :
            base(XPosBefore, YPosBefore, XPosAfter, YPosAfter)
        {
            str = Str;
            this.isDel = isDel;
        }

        public override OperationInfo Redo()
        {
            Type buffType = typeof(Buffer);
            MethodInfo methodInfo;
            if (ch != '\0')
            {
                if (isDel)
                    methodInfo = buffType.GetMethod("Del");
                else
                    methodInfo = buffType.GetMethod("Backspace");
            }
            else
            {
                if (isDel)
                    methodInfo = buffType.GetMethod("CtrlDel");
                else
                    methodInfo = buffType.GetMethod("CtrlBackspace");
            }

            return new OperationInfo(xPosBefore, yPosBefore, 1, methodInfo,
                    new object[] { });
        }

        public override OperationInfo Undo()
        {
            Type buffType = typeof(Buffer);
            if(ch == '\0')
            {
                MethodInfo methodInfo = buffType.GetMethod("Insert");
                return new OperationInfo(xPosAfter, yPosAfter, 1, methodInfo,
                    new object[] { str });
            }
            else if (ch == '\n')
            {
                MethodInfo methodInfo = buffType.GetMethod("Enter");
                return new OperationInfo(xPosAfter, yPosAfter, 1, methodInfo,
                    new object[] { });
            }
            else
            {
                MethodInfo methodInfo = buffType.GetMethod("Insert");
                return new OperationInfo(xPosAfter, yPosAfter, 1, methodInfo,
                    new object[] { ch.ToString() });
            }
        }
    }
}
