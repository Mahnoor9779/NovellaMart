using System;

namespace NovellaMart.Core.BL.Data_Structures
{
    public class PriorityQueueNode<type>
    {
        public type Data;
        public int Priority;
        public PriorityQueueNode<type> Next;

        public PriorityQueueNode(type data, int priority)
        {
            Data = data;
            Priority = priority;
            Next = null;
        }
    }
}
