using System.Collections.Generic;

namespace sscpfe
{
    class OperationList
    {
        LinkedList<Operation> operations;
        public LinkedListNode<Operation> Curr { get; private set; } = null;
        public OperationList()
        {
            operations = new LinkedList<Operation>();
        }
        public void Prev()
        {
            if(Curr != null)
                Curr = Curr.Previous;
        }

        public void Next()
        {
            if(Curr != null)
                Curr = Curr.Next;
        }
        public void Add(Operation operation)
        {
           if(Curr != null && Curr != operations.Last) // for redo
            {
                // delete after curr
                while (Curr.Next != null)
                {
                    operations.Remove(Curr.Next);
                }

                if (operations.Count == 0)
                    Curr = null;
            }
            operations.AddLast(operation);
            Curr = operations.Last;
        }
    }
}
