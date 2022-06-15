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
        void Backspace();
        void CtrlBackspace();
        void Del();
        void CtrlDel();

        void Enter();

        void MoveUp();
        void MoveDown();
        void MoveLeft();
        void MoveRight();
        void Home();
        void End();

        void CtrlLeftArrow();
        void CtrlRightArrow();

        void PerformOperation(OperationInfo oi);
    }
}