using System.Collections.Generic;

namespace sscpfe
{
    interface IBuffer
    {
        CursorPosition cursor { get; }
        CursorPosition defaultCursor { get; }

        string this[int i] { get; }
        IEnumerable<string> Buff();
        void LoadBuff(List<string> buff);

        int MaxYPos();

        void Print();
        void Insert(string str);

        void Enter();

        void PerformOperation(OperationInfo oi);
    }
}