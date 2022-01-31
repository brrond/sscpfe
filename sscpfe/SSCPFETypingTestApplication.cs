using System;
using System.Collections.Generic;

namespace sscpfe
{
    class TypingTestBuffer : IBuffer
    {
        // TODO: rename to painteble buffer (or smth)
        Buffer buff;
        public TypingTestBuffer()
        {
            buff = new Buffer();
        }

        public TypingTestBuffer(int DefaultX, int DefaultY)
        {
            buff = new Buffer(DefaultX, DefaultY);
        }

        public string this[int i] => ((IBuffer)buff)[i];

        public CursorPosition cursor => ((IBuffer)buff).cursor;

        public CursorPosition defaultCursor => ((IBuffer)buff).defaultCursor;

        public void Backspace()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> Buff()
        {
            return ((IBuffer)buff).Buff();
        }

        public void CtrlBackspace()
        {
            throw new NotImplementedException();
        }

        public void CtrlDel()
        {
            ((IBuffer)buff).CtrlDel();
        }

        public void CtrlLeftArrow()
        {
            ((IBuffer)buff).CtrlLeftArrow();
        }

        public void CtrlRightArrow()
        {
            ((IBuffer)buff).CtrlRightArrow();
        }

        public void Del()
        {
            ((IBuffer)buff).Del();
        }

        public void End()
        {
            ((IBuffer)buff).End();
        }

        public void Enter()
        {
            ((IBuffer)buff).Enter();
        }

        public void Home()
        {
            ((IBuffer)buff).Home();
        }

        public void Insert(string str)
        {
            throw new NotImplementedException();
        }

        public void LoadBuff(List<string> buff)
        {
            ((IBuffer)this.buff).LoadBuff(buff);
        }

        public int MaxYPos()
        {
            return ((IBuffer)buff).MaxYPos();
        }

        public void MoveDown()
        {
            ((IBuffer)buff).MoveDown();
        }

        public void MoveLeft()
        {
            ((IBuffer)buff).MoveLeft();
        }

        public void MoveRight()
        {
            ((IBuffer)buff).MoveRight();
        }

        public void MoveUp()
        {
            ((IBuffer)buff).MoveUp();
        }

        public void PerformOperation(OperationInfo oi)
        {
            ((IBuffer)buff).PerformOperation(oi);
        }

        public void Print()
        {
            throw new NotImplementedException();
        }
    }

    class SSCPFETypingTestApplication : SSCPFEApplicationAbstract
    {
        public SSCPFETypingTestApplication()
        {
            buff = new TypingTestBuffer();
        }

        public override void Mainloop()
        {
            while (true)
            {
                buff.Print();
            }
                // TODO: add
                // test will take 30 sec so it necessary to have
                // text about 250-300 words (it's impossible to rich)
                // there will be only 5 commands:
                // tab (to start new test)
                // esc (to exit)
                // default
                // backspace (del prev char)
                // ctrl + backspace (del prev word)

                // After text is loaded it'll be loaded into buffer
                // (file or manually i don't know)
                // after that cursor will be place at (0; 0)
                // when you click the right char it will change color
                // if it's right char - green background and black fg
                // if not - red bg and ? fg

                // What about mistakes? (accuracy)

                // After 30 seconds (where is timer? (in title of cmd?))
                // the test'll automaticaly stop and result'll be displayed
                // with wpm (how to calc. wpm?)
                // with accuracy (?)
                // ask user to start again or exit
            }

    }
}
